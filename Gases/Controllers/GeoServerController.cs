using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gases.Data;
using Gases.Models;
using HtmlAgilityPack;
using Ionic.Zip;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

namespace Gases.Controllers
{
    public class GeoServerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public GeoServerController(ApplicationDbContext context,
            IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _context = context;
            _sharedLocalizer = sharedLocalizer;
        }

        public void GeoTiffFile(int GaseId,
            string Year,
            string Month,
            string Name)
        {
            GeoTiffFile geoTiffFile = new GeoTiffFile
            {
                GaseId = GaseId,
                Year = Year,
                Month = Month,
                Name = Name
            };

            _context.GeoTiffFile.Add(geoTiffFile);
            _context.SaveChanges();
        }

        /// <summary>
        /// Запуск CURL с параметрами
        /// </summary>
        /// <param name="Arguments"></param>
        /// <returns></returns>
        private Process CurlExecute(string Arguments)
        {
            Process process = new Process();
            try
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.FileName = Startup.Configuration["GeoServer:CurlFullPath"];
                process.StartInfo.Arguments = Arguments;
                process.Start();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
            return process;
        }

        private Process CurlExecuteFalse(string Arguments)
        {
            Process process = new Process();
            try
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = false;
                process.StartInfo.RedirectStandardError = false;
                process.StartInfo.FileName = Startup.Configuration["GeoServer:CurlFullPath"];
                process.StartInfo.Arguments = Arguments;
                process.Start();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
            return process;
        }

        /// <summary>
        /// Возвращает список всех рабочих областей GeoServer
        /// </summary>
        /// <returns></returns>
        private string[] GetWorkspaces()
        {
            try
            {
                Process process = CurlExecute($" -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -XGET" +
                    $" http://{Startup.Configuration["GeoServer:Address"]}:" +
                    $"{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces.html");
                string html = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
                HtmlNode root = htmlDocument.DocumentNode;
                List<string> workspaces = new List<string>();
                foreach (HtmlNode node in root.Descendants())
                {
                    if (node.Name == "title" && node.InnerText.ToLower().Contains("error"))
                    {
                        throw new Exception(node.InnerText);
                    }
                    if (node.Name == "a")
                    {
                        workspaces.Add(node.InnerText);
                    }
                }
                return workspaces.ToArray();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Возвращает список всех хранилищ рабочей области GeoServer
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <returns></returns>
        private string[] GetWorkspaceStores(string WorkspaceName)
        {
            try
            {
                if (!GetWorkspaces().Contains(WorkspaceName))
                {
                    throw new Exception($"No workspace {WorkspaceName}!");
                }
                if (!string.IsNullOrEmpty(WorkspaceName))
                {
                    Process process = CurlExecute($" -u " +
                        $"{Startup.Configuration["GeoServer:User"]}:" +
                        $"{Startup.Configuration["GeoServer:Password"]}" +
                        $" -XGET" +
                        $" http://{Startup.Configuration["GeoServer:Address"]}:" +
                        $"{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces/{WorkspaceName}");
                    string html = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    HtmlDocument htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(html);
                    HtmlNode root = htmlDocument.DocumentNode;
                    List<string> stores = new List<string>();
                    foreach (HtmlNode node in root.Descendants())
                    {
                        if (node.Name == "a")
                        {
                            stores.Add(node.InnerText);
                        }
                    }
                    return stores.ToArray();
                }
                else
                {
                    throw new Exception("WorkspaceName must be non-empty!");
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Возвращает тип хранилища GeoServer
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <param name="StoreName"></param>
        /// <returns>
        /// "datastores" или "coveragestores"
        /// </returns>
        private string GetStoreType(string WorkspaceName, string StoreName)
        {
            try
            {
                Process process = CurlExecute($" -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -XGET" +
                    $" http://{Startup.Configuration["GeoServer:Address"]}:" +
                    $"{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces/{WorkspaceName}");
                string html = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
                HtmlNode root = htmlDocument.DocumentNode;
                string storeType = "";
                foreach (HtmlNode node in root.Descendants())
                {
                    if (node.Name == "a")
                    {
                        if (node.InnerText == StoreName)
                        {
                            storeType = node.GetAttributeValue("href", "").Split(WorkspaceName)[1].Split(StoreName)[0].Replace("/", "");
                            break;
                        }
                    }
                }
                return storeType;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Возвращает список всех слоев хранилища GeoServer
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <param name="StoreName"></param>
        /// <returns></returns>
        private string[] GetStoreLayers(string WorkspaceName, string StoreName)
        {
            try
            {
                if (!GetWorkspaces().Contains(WorkspaceName))
                {
                    throw new Exception($"No workspace {WorkspaceName}!");
                }
                if (!GetWorkspaceStores(WorkspaceName).Contains(StoreName))
                {
                    throw new Exception($"No store {WorkspaceName} in workspace {WorkspaceName}!");
                }
                if (string.IsNullOrEmpty(WorkspaceName))
                {
                    throw new Exception("WorkspaceName must be non-empty!");
                }
                if (string.IsNullOrEmpty(StoreName))
                {
                    throw new Exception("StoreName must be non-empty!");
                }
                string storeType = GetStoreType(WorkspaceName, StoreName);
                Process process = CurlExecute($" -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -XGET" +
                    $" http://{Startup.Configuration["GeoServer:Address"]}:" +
                    $"{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces/{WorkspaceName}/{storeType}/{StoreName}");
                string html = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
                HtmlNode root = htmlDocument.DocumentNode;
                List<string> layers = new List<string>();
                foreach (HtmlNode node in root.Descendants())
                {
                    if (node.Name == "a")
                    {
                        layers.Add(node.InnerText);
                    }
                }
                return layers.ToArray();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Возвращает список всех слоев рабочей области GeoServer
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <returns></returns>
        private string[] GetWorkspaceLayers(string WorkspaceName)
        {
            try
            {
                List<string> layers = new List<string>();
                foreach (string store in GetWorkspaceStores(WorkspaceName))
                {
                    layers.AddRange(GetStoreLayers(WorkspaceName, store));
                }
                return layers.ToArray();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Возвращает список всех GeoTIFF слоев рабочей области GeoServer
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <returns></returns>
        private string[] GetWorkspaceGeoTIFFLayers(string WorkspaceName)
        {
            try
            {
                List<string> layers = new List<string>();
                foreach (string store in GetWorkspaceStores(WorkspaceName))
                {
                    if (GetStoreType(WorkspaceName, store) == "coveragestores")
                    {
                        layers.AddRange(GetStoreLayers(WorkspaceName, store));
                    }
                }
                return layers.ToArray();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Возвращает стили GeoServer рабочей области
        /// </summary>
        /// <param name="WorkspaceName"></param>
        public string[] GetWorkspaceStyles(string WorkspaceName)
        {
            try
            {
                if (!GetWorkspaces().Contains(WorkspaceName))
                {
                    throw new Exception($"No workspace {WorkspaceName}!");
                }
                Process process = CurlExecute($" -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -XGET" +
                    $" http://{Startup.Configuration["GeoServer:Address"]}:" +
                    $"{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces/{WorkspaceName}/styles.html");
                string html = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
                HtmlNode root = htmlDocument.DocumentNode;
                List<string> stores = new List<string>();
                foreach (HtmlNode node in root.Descendants())
                {
                    if (node.Name == "a")
                    {
                        stores.Add(node.InnerText);
                    }
                }
                return stores.ToArray();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Возвращает путь к папке рабочей области
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <returns></returns>
        private string GetWorkspaceDirectoryPath(string WorkspaceName)
        {
            try
            {
                return Path.Combine(Path.Combine(Startup.Configuration["GeoServer:DataDir"], "data"), WorkspaceName);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Публикация GeoTIFF-файла в GeoServer
        /// </summary>
        /// <remarks>
        /// Будет опубликован слой в GeoServer с именем, совпадающим с именем файла (без расширения)
        /// Будет создано новое хранилище в GeoServer с именем, совпадающим с именем файла (без расширения)
        /// Если существует опубликованный слой с именем, совпадающим с именем файла (без расширения), в рабочей области с именем, совпадающим с именем файла (без расширения), будет выдано исключение
        /// </remarks>
        /// <param name="WorkspaceName">
        /// Рабочая область GeoServer, в которой публикуется слой
        /// </param>
        /// <param name="FileName">
        /// Имя публикуемого GeoTIFF-файла с расширением без пути в папке данных GeoServer
        /// </param>
        /// <param name="Style">
        /// Стиль GeoServer, с которым будет опубликован слой. Должен быть в рабочей области "WorkspaceName"
        /// </param>
        private void PublishGeoTIFF(string WorkspaceName, string FileName, string Style)
        {
            try
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(FileName);

                if (GetWorkspaceLayers(WorkspaceName).Contains(fileNameWithoutExtension))
                {
                    throw new Exception($"Layer {fileNameWithoutExtension} is already exist in {WorkspaceName} workspace!");
                }

                if (Path.GetExtension(FileName).ToLower() != ".tif" || Path.GetExtension(FileName).ToLower() != ".tif")
                {
                    throw new Exception("File extension must be \"tif\" or \"tiff\"!");
                }

                string s1 = $" -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -v -XPOST" +
                    $" -H \"Content-type: text/xml\"" +
                    $" \\ -d \"<coverageStore><name>{fileNameWithoutExtension}</name>" +
                    $"<workspace>{WorkspaceName}</workspace>" +
                    $"<enabled>true</enabled>" +
                    $"<type>GeoTIFF</type>" +
                    $"<url>/data/{WorkspaceName}/{FileName}</url></coverageStore>\"" +
                    $" \\ http://{Startup.Configuration["GeoServer:Address"]}:" +
                    $"{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces/{WorkspaceName}/coveragestores?configure=all",
                    s2 = $" -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -v -XPOST" +
                    $" -H \"Content-type: text/xml\"" +
                    $" -d \"<coverage><name>{fileNameWithoutExtension}</name>" +
                    $"<title>{fileNameWithoutExtension}</title>" +
                    //$"<nativeCRS>EPSG:3857</nativeCRS>" +
                    //$"<srs>EPSG:3857</srs>" +
                    $"<nativeCRS>EPSG:4326</nativeCRS>" +
                    $"<srs>EPSG:4326</srs>" +
                    $"<projectionPolicy>FORCE_DECLARED</projectionPolicy>" +
                    $"<defaultInterpolationMethod><name>nearest neighbor</name></defaultInterpolationMethod></coverage>\"" +
                    $" \\ \"http://{Startup.Configuration["GeoServer:Address"]}" +
                    $":{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces/{WorkspaceName}/coveragestores/{fileNameWithoutExtension}/coverages?recalculate=nativebbox\"",
                    s3 = $" -u " +
                     $"{Startup.Configuration["GeoServer:User"]}:" +
                     $"{Startup.Configuration["GeoServer:Password"]}" +
                     $" -XPUT" +
                     $" -H \"Content-type: text/xml\"" +
                     $" -d \"<layer><defaultStyle><name>{WorkspaceName}:{Style}</name></defaultStyle></layer>\"" +
                     $" http://{Startup.Configuration["GeoServer:Address"]}" +
                     $":{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/layers/{WorkspaceName}:{fileNameWithoutExtension}";

                Process process1 = CurlExecuteFalse($" -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -v -XPOST" +
                    $" -H \"Content-type: text/xml\"" +
                    $" \\ -d \"<coverageStore><name>{fileNameWithoutExtension}</name>" +
                    $"<workspace>{WorkspaceName}</workspace>" +
                    $"<enabled>true</enabled>" +
                    $"<type>GeoTIFF</type>" +
                    $"<url>/data/{WorkspaceName}/{FileName}</url></coverageStore>\"" +
                    $" \\ http://{Startup.Configuration["GeoServer:Address"]}:" +
                    $"{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces/{WorkspaceName}/coveragestores?configure=all");
                process1.WaitForExit();
                Process process2 = CurlExecuteFalse($" -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -v -XPOST" +
                    $" -H \"Content-type: text/xml\"" +
                    $" -d \"<coverage><name>{fileNameWithoutExtension}</name>" +
                    $"<title>{fileNameWithoutExtension}</title>" +
                    //$"<nativeCRS>EPSG:3857</nativeCRS>" +
                    //$"<srs>EPSG:3857</srs>" +
                    $"<nativeCRS>EPSG:4326</nativeCRS>" +
                    $"<srs>EPSG:4326</srs>" +
                    $"<projectionPolicy>FORCE_DECLARED</projectionPolicy>" +
                    $"<defaultInterpolationMethod><name>nearest neighbor</name></defaultInterpolationMethod></coverage>\"" +
                    $" \\ \"http://{Startup.Configuration["GeoServer:Address"]}" +
                    $":{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces/{WorkspaceName}/coveragestores/{fileNameWithoutExtension}/coverages?recalculate=nativebbox\"");
                process2.WaitForExit();
                Process process3 = CurlExecuteFalse($" -u " +
                     $"{Startup.Configuration["GeoServer:User"]}:" +
                     $"{Startup.Configuration["GeoServer:Password"]}" +
                     $" -XPUT" +
                     $" -H \"Content-type: text/xml\"" +
                     $" -d \"<layer><defaultStyle><name>{WorkspaceName}:{Style}</name></defaultStyle></layer>\"" +
                     $" http://{Startup.Configuration["GeoServer:Address"]}" +
                     $":{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/layers/{WorkspaceName}:{fileNameWithoutExtension}");
                process3.WaitForExit();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Отмена публикации GeoTIFF-файла в GeoServer
        /// </summary>
        /// <remarks>
        /// Имя хранилища слоя GeoServer должно совпадать с именем слоя GeoServer
        /// </remarks>
        /// <param name="WorkspaceName">
        /// Рабочая область GeoServer, в которой отменяется публикация слоя
        /// </param>
        /// <param name="LayerName">
        /// Имя слоя GeoServer, отмена публикации которого происходит
        /// </param>
        private void UnpublishGeoTIFF(string WorkspaceName, string LayerName)
        {
            try
            {
                if (!GetWorkspaceLayers(WorkspaceName).Contains(LayerName))
                {
                    throw new Exception($"Layer {LayerName} isn't exist in {WorkspaceName} workspace!");
                }

                Process process1 = CurlExecute($" -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -v -XDELETE" +
                    $" http://{Startup.Configuration["GeoServer:Address"]}:" +
                    $"{Startup.Configuration["GeoServer:Port"]}/" +
                    $"geoserver/rest/layers/{LayerName}");
                process1.WaitForExit();
                Process process2 = CurlExecute($" -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -v -XDELETE" +
                    $" http://{Startup.Configuration["GeoServer:Address"]}" +
                    $":{Startup.Configuration["GeoServer:Port"]}/" +
                    $"geoserver/rest/workspaces/{WorkspaceName}/coveragestores/{LayerName}/coverages/{LayerName}");
                process2.WaitForExit();
                Process process3 = CurlExecute($" -v -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -XDELETE" +
                    $" http://{Startup.Configuration["GeoServer:Address"]}" +
                    $":{Startup.Configuration["GeoServer:Port"]}/" +
                    $"geoserver/rest/workspaces/{WorkspaceName}/coveragestores/{LayerName}");
                process3.WaitForExit();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Возвращает все GeoTIFF-файлы (без пути, с расширением) с папки рабочей области GeoServer
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <returns></returns>
        public string[] GetGeoTIFFFiles(string WorkspaceName)
        {
            IConfigurationSection geoTIFFFileExtentions = Startup.Configuration.GetSection("GeoServer:GeoTIFFFileExtentions");
            List<string> geoTIFFFiles = Directory.GetFiles(GetWorkspaceDirectoryPath(WorkspaceName), "*.*", SearchOption.TopDirectoryOnly).OrderBy(l => l).ToList();
            geoTIFFFiles.RemoveAll(l => !geoTIFFFileExtentions.AsEnumerable().Select(e => e.Value).Contains(Path.GetExtension(l)));
            geoTIFFFiles = geoTIFFFiles.Select(l => { return Path.GetFileName(l); }).ToList();
            return geoTIFFFiles.ToArray();
        }

        /// <summary>
        /// Загрузка GeoTIFF-файлов в папку рабочей области GeoServer
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <param name="Files"></param>
        private string[] UploadGeoTIFFFiles(string WorkspaceName, List<IFormFile> Files)
        {
            List<string> filesreport = new List<string>(),
            report = new List<string>();
            try
            {
                //foreach (IFormFile file in Files)
                //{
                //    var filePath = Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), Path.GetFileName(file.FileName));
                //    using (var fileStream = new FileStream(filePath, FileMode.Create))
                //    {
                //        file.CopyTo(fileStream);
                //    }
                //}

                //foreach (IFormFile file in Files)
                //{
                //    var filePath = Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", Path.GetFileName(file.FileName));
                //    using (var fileStream = new FileStream(filePath, FileMode.Create))
                //    {
                //        file.CopyTo(fileStream);
                //    }
                //}
                //====================================================================================================================
                
                //List<string> unzipfiles = new List<string>();
                //List<string> zipfiles = new List<string>();
                //foreach (string file in Directory.GetFiles(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload"), "*.zip", SearchOption.TopDirectoryOnly))
                //{
                //    if (Path.GetExtension(file) == ".zip")
                //    {
                //        try
                //        {
                //            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                //            using (ZipFile zip = ZipFile.Read(Path.Combine(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload"), Path.GetFileName(file))))
                //            {
                //                foreach (ZipEntry filefromzip in zip)
                //                {
                //                    filefromzip.Extract(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload"), ExtractExistingFileAction.OverwriteSilently);
                //                    unzipfiles.Add(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", filefromzip.FileName));
                //                }
                //            }
                //            //zipfiles.Add(file);
                //            zipfiles.AddRange(Directory.GetFiles(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload"), Path.GetFileNameWithoutExtension(file) + ".z*", SearchOption.TopDirectoryOnly));
                //        }
                //        catch
                //        {

                //        }
                //    }
                //}
                IConfigurationSection geoTIFFFileExtentions = Startup.Configuration.GetSection("GeoServer:GeoTIFFFileExtentions");
                int filesCounter = 0;
                //foreach (string file in zipfiles)
                //{
                //    if (!geoTIFFFileExtentions.AsEnumerable().Select(l => l.Value).Contains(Path.GetExtension(file)))
                //    {
                //        System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", file));
                //    }
                //}
                //================================================================================================================================

                foreach (string file in Files.Select(f => f.FileName))
                {
                    var fileName = Path.GetFileName(file);
                    if (geoTIFFFileExtentions.AsEnumerable().Select(l => l.Value).Contains(Path.GetExtension(fileName)))
                    {
                        if (System.IO.File.Exists(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), fileName)))
                        {
                            report.Add($"{fileName}: {_sharedLocalizer["exist"]}!");
                            filesCounter = filesCounter + 1;
                            //System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), fileName));
                        }
                        else
                        {
                            //System.IO.File.Move(file, Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), fileName));
                            //foreach (IFormFile fileAdd in Files)
                            //{
                            IFormFile fileAdd = Files[filesCounter];
                            var filePath = Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), Path.GetFileName(fileAdd.FileName));
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                                {
                                    fileAdd.CopyTo(fileStream);
                                }
                            //}
                            report.Add($"{fileName}: {_sharedLocalizer["uploaded"]}!");
                            filesCounter = filesCounter + 1;

                            string str, date, year, month, gase;
                            int indexPointOneStr, indexPointTwoStr, indexPointOneName;
                            //DateTime dateTime = new DateTime();
                            //int[] dataArray = new int[2];

                            str = file;
                            indexPointOneStr = str.IndexOf('.');
                            indexPointTwoStr = str.IndexOf('.', indexPointOneStr + 1);
                            date = str.Remove(indexPointTwoStr, str.Length - indexPointTwoStr);
                            gase = str.Remove(0, indexPointTwoStr + 1);
                            indexPointOneName = gase.IndexOf('.');
                            gase = gase.Remove(indexPointOneName, gase.Length - indexPointOneName);
                            string[] words = date.Split(new char[] { '.' });
                            year = words[0];
                            month = words[1];

                            //dateTime = new DateTime(dataArray[0], dataArray[1], 01);

                            gase = gase.ToUpper();
                            int idGase = _context.Gase.Where(d => d.Formula == gase).First().Id;
                            //idGase = idGase == null ? 0 : idGase;

                            GeoTiffFile(idGase, year, month, file);
                        }
                    }
                    else if (Path.GetExtension(file)[1] != 'z')
                    {
                        report.Add($"{fileName}: {_sharedLocalizer["notGeoTIFF"]}!");
                        filesCounter = filesCounter + 1;
                        //System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), fileName));
                    }
                }

                //================================================================================================================================
                //foreach (string file in unzipfiles)
                //{
                //    var fileName = Path.GetFileName(file);
                //    if (geoTIFFFileExtentions.AsEnumerable().Select(l => l.Value).Contains(Path.GetExtension(fileName)))
                //    {
                //        if (System.IO.File.Exists(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), fileName)))
                //        {
                //            report.Add($"{fileName}: {_sharedLocalizer["exist"]}!");
                //            System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName));
                //        }
                //        else
                //        {
                //            System.IO.File.Move(file, Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), fileName));
                //            report.Add($"{fileName}: {_sharedLocalizer["uploaded"]}!");
                //        }
                //    }
                //    else
                //    {
                //        report.Add($"{fileName}: {_sharedLocalizer["notGeoTIFF"]}!");
                //        System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName));
                //    }
                //}
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
            return report.ToArray();
        }

        /// <summary>
        /// Возвращает файл (путь, расширение) хранилища типа "coveragestores"
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <param name="StoreName"></param>
        private string GetCoveragestoreGeoTIFFFile(string WorkspaceName, string StoreName)
        {
            try
            {
                string GeoTIFFFile = "";

                Process process = CurlExecute($" -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -XGET" +
                    $" http://{Startup.Configuration["GeoServer:Address"]}:" +
                    $"{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces/{WorkspaceName}/coveragestores/{StoreName}.xml");
                string html = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
                HtmlNode root = htmlDocument.DocumentNode;
                foreach (HtmlNode node in root.Descendants())
                {
                    if (node.Name == "url")
                    {
                        GeoTIFFFile = Startup.Configuration["GeoServer:DataDir"] + node.InnerText.Replace("/", "\\");
                        break;
                    }
                }
                return GeoTIFFFile;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Возвращает список слоев, в которых используется GeoTIFF-файл
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <param name="FileName">
        /// Полный путь к файлу с расширением
        /// </param>
        private string[] GetGeoTIFFFileLayers(string WorkspaceName, string FileName)
        {
            try
            {
                Process process = CurlExecute($" -u " +
                $"{Startup.Configuration["GeoServer:User"]}:" +
                $"{Startup.Configuration["GeoServer:Password"]}" +
                $" -XGET" +
                $" http://{Startup.Configuration["GeoServer:Address"]}:" +
                $"{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces/{WorkspaceName}");
                string html = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
                HtmlNode root = htmlDocument.DocumentNode;
                List<string> layers = new List<string>();
                foreach (HtmlNode node in root.Descendants())
                {
                    if (node.Name == "a")
                    {
                        if (node.GetAttributeValue("href", "").Contains("coveragestores"))
                        {
                            if (GetCoveragestoreGeoTIFFFile(WorkspaceName, node.InnerText) == Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), FileName))
                            {
                                layers.AddRange(GetStoreLayers(WorkspaceName, node.InnerText));
                            }
                        }
                    }
                }
                return layers.ToArray();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Удаление GeoTIFF-файла
        /// </summary>
        /// <remarks>
        /// Если GeoTIFF-файл используется в слоях GeoServer, будет выдано исключение
        /// </remarks>
        /// <param name="WorkspaceName"></param>
        /// <param name="FileName">
        /// Полный путь к файлу с расширением
        /// </param>
        private void DeleteGeoTIFFFile(string WorkspaceName, string FileName)
        {
            try
            {
                string[] geoTIFFFileLayers = GetGeoTIFFFileLayers(WorkspaceName, FileName);
                if (geoTIFFFileLayers.Count() > 0)
                {
                    //throw new Exception($"{FileName} can't be deleted because it is used in current layers: {string.Join(", ", geoTIFFFileLayers)}!");
                    throw new Exception(string.Format(_sharedLocalizer["MessageCantBeDeleted"], FileName, string.Join(", ", geoTIFFFileLayers)));
                }
                System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), FileName));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        //===========================================================================================================================================================================
        /// <summary>
        /// Загрузка GeoTIFF-файлов в папку рабочей области GeoServer (Get)
        /// </summary>
        /// <remarks>
        /// Рабочая область всегда "Gases"
        /// Файлы могут быть разделены на zip-архивы
        /// </remarks>
        [DisableRequestSizeLimit]
        //[RequestSizeLimit(long.MaxValue)]
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult UploadGeoTIFFFiles()
        {
            return View();
        }

        /// <summary>
        /// Загрузка GeoTIFF-файлов в папку рабочей области GeoServer (Post)
        /// </summary>
        /// <remarks>
        /// Рабочая область всегда "Gases"
        /// Файлы могут быть разделены на zip-архивы
        /// </remarks>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DisableRequestSizeLimit]
        //[RequestSizeLimit(long.MaxValue)]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<string> UploadGeoTIFFFiles(List<IFormFile> Files)
        {
            string message = _sharedLocalizer["FilesUploaded"];
            try
            {
                message = string.Join("<br/>", UploadGeoTIFFFiles(Startup.Configuration["GeoServer:Workspace"], Files));
            }
            catch (Exception exception)
            {
                message = $"{exception.ToString()}. {exception.InnerException?.Message}";
            }
            return message;
        }

        /// <summary>
        /// Удаление GeoTIFF-файлов из рабочей области GeoServer (Get)
        /// </summary>
        /// <remarks>
        /// Рабочая область всегда "Gases"
        /// </remarks>
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult DeleteGeoTIFFFile()
        {
            ViewBag.GeoTIFFFiles = new SelectList(GetGeoTIFFFiles(Startup.Configuration["GeoServer:Workspace"]));
            return View();
        }

        /// <summary>
        /// Удаление GeoTIFF-файлов из рабочей области GeoServer (Post)
        /// </summary>
        /// <remarks>
        /// Рабочая область всегда "Gases"
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteGeoTIFFFile(string GeoTIFFFile)
        {
            ViewData["Message"] = "";
            try
            {
                DeleteGeoTIFFFile(Startup.Configuration["GeoServer:Workspace"], GeoTIFFFile);
                //ViewData["Message"] = $"File {GeoTIFFFile} was deleted!";
                ViewData["Message"] = string.Format(_sharedLocalizer["MessageFileWasDeleted"], GeoTIFFFile);
            }
            catch (Exception exception)
            {
                ViewData["Message"] = $"{exception.ToString()}. {exception.InnerException?.Message}";
            }
            ViewBag.GeoTIFFFiles = new SelectList(GetGeoTIFFFiles(Startup.Configuration["GeoServer:Workspace"]));
            return View();
        }

        /// <summary>
        /// Публикация GeoTIFF-файла в рабочей области Gases (Get)
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult PublishGeoTIFF()
        {
            var publicshedLayers = GetWorkspaceLayers(Startup.Configuration["GeoServer:Workspace"]);
            ViewBag.GeoTIFFFiles = new SelectList(GetGeoTIFFFiles(Startup.Configuration["GeoServer:Workspace"])
                .Where(l => !publicshedLayers.Contains(Path.GetFileNameWithoutExtension(l))));
            ViewBag.Styles = new SelectList(GetWorkspaceStyles(Startup.Configuration["GeoServer:Workspace"]));
            return View();
        }

        /// <summary>
        /// Публикация GeoTIFF-файла в рабочей области Gases (Post)
        /// </summary>
        /// <param name="GeoTIFFFile">
        /// Имя GeoTIFF-файла с расширенем, без пути
        /// </param>
        /// <param name="Style"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> PublishGeoTIFF(string GeoTIFFFile, string Style, string NameKK, string NameRU, string NameEN)
        {
            string message = "";
            try
            {
                PublishGeoTIFF(Startup.Configuration["GeoServer:Workspace"], GeoTIFFFile, Style);
                Layer layer = new Layer()
                {
                    NameKK = NameKK,
                    NameRU = NameRU,
                    NameEN = NameEN,
                    GeoServerStyle = Style,
                    GeoServerName = Path.GetFileNameWithoutExtension(GeoTIFFFile),
                    FileNameWithPath = Path.Combine(GetWorkspaceDirectoryPath(Startup.Configuration["GeoServer:Workspace"]), GeoTIFFFile)
                };
                _context.Add(layer);
                await _context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                message = $"{exception.ToString()}. {exception.InnerException?.Message}";
            }
            var publicshedLayers = GetWorkspaceLayers(Startup.Configuration["GeoServer:Workspace"]);
            ViewBag.GeoTIFFFiles = new SelectList(GetGeoTIFFFiles(Startup.Configuration["GeoServer:Workspace"])
                .Where(l => !publicshedLayers.Contains(Path.GetFileNameWithoutExtension(l))));
            ViewBag.Styles = new SelectList(GetWorkspaceStyles(Startup.Configuration["GeoServer:Workspace"]));
            ViewBag.Message = message;
            return View();
        }

        /// <summary>
        /// Отмена публикации GeoTIFF-файла в рабочей области Gases (Get)
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult UnpublishGeoTIFF()
        {
            ViewBag.GeoTIFFLayers = new SelectList(GetWorkspaceGeoTIFFLayers(Startup.Configuration["GeoServer:Workspace"]));
            return View();
        }

        /// <summary>
        /// Отмена публикации GeoTIFF-файла в рабочей области Gases (Get)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult UnpublishGeoTIFF(string LayerName)
        {
            ViewData["Message"] = "";
            try
            {
                UnpublishGeoTIFF(Startup.Configuration["GeoServer:Workspace"], LayerName);
                var layer = _context.Layer.SingleOrDefault(m => m.GeoServerName == LayerName);
                if (layer != null)
                {
                    _context.Layer.Remove(layer);
                    _context.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                ViewData["Message"] = $"{exception.ToString()}. {exception.InnerException?.Message}";
            }
            ViewBag.GeoTIFFLayers = new SelectList(GetWorkspaceGeoTIFFLayers(Startup.Configuration["GeoServer:Workspace"]));
            return View();
        }
    }
}