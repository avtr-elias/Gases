using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Gases.Data;
using Gases.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gases.Controllers
{
    public class UploadController : Controller
    {
        IHostingEnvironment _appEnvironment;
        private readonly ApplicationDbContext _context;

        public UploadController(IHostingEnvironment appEnvironment, ApplicationDbContext context)
        {
            _appEnvironment = appEnvironment;
            _context = context;
        }

        public void NetCDF(int Gase,
            DateTime DateTime,
            string Name,
            string Unit,
            decimal Longtitude,
            decimal Latitude,
            decimal Value)
        {
            NetCDF netCDF = new NetCDF
            {
                Gase = Gase,
                DateTime = DateTime,
                Name = Name,
                Unit = Unit,
                Longtitude = Longtitude,
                Latitude = Latitude,
                Value = Value
            };

            _context.NetCDF.Add(netCDF);
            _context.SaveChanges();
        }

        //[Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        //[Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> AddFile(IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                //-----------Проверяем, существует ли уже файл с таким именем----------
                //string[] dirs = Directory.GetFiles((Path.Combine(_appEnvironment.WebRootPath, "Uploaded")), "*.nc");
                //for (int i = 0; i < dirs.Length; i++)
                //{
                //    dirs[i] = Path.GetFileName(dirs[i]);
                //}
                //foreach (string nameFile in dirs)
                //{
                //    if (uploadedFile.FileName == nameFile)
                //    {
                //        ViewBag.Message = "A file with this name already exists!";
                //        return RedirectToAction("Index");
                //    }
                //}
                //---------------------------------------------------------------------------

                // путь к папке Uploaded
                //string path = "/Uploaded/" + uploadedFile.FileName;
                string path = Path.Combine(_appEnvironment.WebRootPath, "Uploaded", Path.GetFileName(uploadedFile.FileName));
                // сохраняем файл в папку Uploaded в каталоге wwwroot
                //using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                if (Path.GetExtension(path) == ".nc")
                {
                    string folder = Path.Combine(_appEnvironment.WebRootPath, "Uploaded");
                    string batfile = Path.Combine(folder, "bat.bat");
                    string filename = Path.GetFileNameWithoutExtension(uploadedFile.FileName);

                    using (var sw = new StreamWriter(batfile))
                    {
                        sw.WriteLine("ncdump -f c " + filename + ".nc > " + filename + ".txt");
                    }

                    Process process = new Process();
                    try
                    {
                        process.StartInfo.WorkingDirectory = folder;
                        process.StartInfo.FileName = batfile;
                        process.Start();
                        process.WaitForExit();
                        System.IO.File.Delete(batfile);
                    }
                    catch (Exception exception)
                    {
                        throw new Exception(exception.ToString(), exception.InnerException);
                    }
                    //Получение необходимой информации из созданного файла *.txt
                    path = Path.Combine(_appEnvironment.WebRootPath, "Uploaded", filename + ".txt");

                    string name, gase, unit, date;
                    List<decimal> value = new List<decimal>();
                    List<decimal> lon = new List<decimal>();
                    List<decimal> lat = new List<decimal>();
                    name = gase = unit = null;
                    DateTime dateTime = new DateTime();
                    int indexOfSubstring;
                    string subString;

                    using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != "data:")
                        {
                            if (name == null)
                            {
                                subString = "long_name = \"";
                                indexOfSubstring = line.IndexOf(subString);
                                if (indexOfSubstring != -1)
                                {
                                    indexOfSubstring = indexOfSubstring + subString.Length;
                                    name = line.Substring(indexOfSubstring);
                                    name = name.Remove(name.IndexOf('"'));
                                }
                            }
                            if (gase == null)
                            {
                                subString = "quantity_type = \"";
                                indexOfSubstring = line.IndexOf(subString);
                                if (indexOfSubstring != -1)
                                {
                                    indexOfSubstring = indexOfSubstring + subString.Length;
                                    gase = line.Substring(indexOfSubstring);
                                    gase = gase.Remove(gase.IndexOf('"'));
                                }
                            }
                            if (unit == null)
                            {
                                subString = "units = \"";
                                indexOfSubstring = line.IndexOf(subString);
                                if (indexOfSubstring != -1)
                                {
                                    indexOfSubstring = indexOfSubstring + subString.Length;
                                    unit = line.Substring(indexOfSubstring);
                                    unit = unit.Remove(unit.IndexOf('"'));
                                }
                            }
                            if (dateTime == new DateTime())
                            {
                                subString = "\"over ";
                                indexOfSubstring = line.IndexOf(subString);
                                if (indexOfSubstring != -1)
                                {
                                    indexOfSubstring = indexOfSubstring + subString.Length;
                                    date = line.Substring(indexOfSubstring);
                                    date = date.Remove(date.IndexOf(','));
                                    indexOfSubstring = date.IndexOf(' ');
                                    if (indexOfSubstring != -1)
                                    {
                                        date = date.Remove(date.IndexOf(' '));
                                    }
                                    int dateYear, dateMonth = 0, dateDay;
                                    string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "June",
                                        "July", "Aug", "Sept", "Oct", "Nov", "Dec" };
                                    string[]  words = date.Split(new char[] { '-' });
                                    dateYear = Convert.ToInt32(words[0]);
                                    for (int i = 0; i < months.Length; i++)
                                    {
                                        if (String.Compare(words[1], months[i]) == 0)
                                        {
                                            dateMonth = i + 1;
                                            break;
                                        }
                                    }
                                    if (dateMonth == 0)
                                    {
                                        dateMonth = Convert.ToInt32(words[1]);
                                    }
                                    if (words.Length == 2)
                                    {
                                        dateTime = new DateTime(dateYear, dateMonth, 01);
                                    }
                                    else
                                    {
                                        dateDay = Convert.ToInt32(words[2]);
                                        dateTime = new DateTime(dateYear, dateMonth, dateDay);
                                    }
                                }
                            }
                        }
                        line = sr.ReadLine() + Environment.NewLine;
                        line = sr.ReadLine();
                        List<int> x = new List<int>();
                        List<int> y = new List<int>();
                        while ((line = sr.ReadLine().Trim(' ')) != "")
                        {
                            subString = "_";
                            indexOfSubstring = line.IndexOf(subString);
                            if (indexOfSubstring > 10)
                            {
                                line = line.Replace(" ", "").Replace("\t", "");
                                indexOfSubstring = line.IndexOf("(");
                                subString = line.Remove(0, line.IndexOf("(") + 1);

                                x.Add(Convert.ToInt32(subString.Remove(subString.IndexOf(","), subString.Length - subString.IndexOf(","))));
                                y.Add(Convert.ToInt32(subString.Remove(subString.IndexOf(")"), subString.Length - subString.IndexOf(")")).Remove(0, subString.IndexOf(",") + 1)));

                                value.Add(Decimal.Parse(line.Remove(line.IndexOf(',')), CultureInfo.InvariantCulture));
                            }
                        }
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.IndexOf("lat =") != -1 || line.IndexOf("lon =") != -1)
                            {
                                do
                                {
                                    string numb = null;
                                    if (line.IndexOf(",") != -1)
                                    {
                                        line = line.Remove(line.IndexOf(","), line.Length - line.IndexOf(","));
                                    }
                                    else
                                    {
                                        line = line.Remove(line.IndexOf(";"), line.Length - line.IndexOf(";"));
                                    }
                                    foreach (char s in line)
                                    {
                                        if (char.IsDigit(s))
                                        {
                                            numb = numb + s;
                                        }
                                    }
                                    lat.Add(Decimal.Parse(numb, CultureInfo.InvariantCulture));
                                } while ((line = sr.ReadLine().Trim(' ')) != "");
                                break;
                            }
                        }

                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.IndexOf("lon =") != -1 || line.IndexOf("lat =") != -1)
                            {
                                do
                                {
                                    string numb = null;
                                    if (line.IndexOf(",") != -1)
                                    {
                                        line = line.Remove(line.IndexOf(","), line.Length - line.IndexOf(","));
                                    }
                                    else
                                    {
                                        line = line.Remove(line.IndexOf(";"), line.Length - line.IndexOf(";"));
                                    }
                                    foreach (char s in line)
                                    {
                                        if (char.IsDigit(s) || char.IsPunctuation(s))
                                        {
                                            numb = numb + s;
                                        }
                                    }
                                    lon.Add(Decimal.Parse(numb, CultureInfo.InvariantCulture));
                                } while ((line = sr.ReadLine().Trim(' ')) != "");
                                break;
                            }
                        }
                        List<decimal> latValue = new List<decimal>();
                        List<decimal> lonValue = new List<decimal>();
                        for (int i = 0; i < x.Count; i++)
                        {
                            for (int j = 0; j < lat.Count; j++)
                            {
                                if (x[i] == j)
                                {
                                    latValue.Add(lat[j]);
                                }
                            }
                        }
                        for (int i = 0; i < y.Count; i++)
                        {
                            for (int j = 0; j < lon.Count; j++)
                            {
                                if (y[i] == j)
                                {
                                    lonValue.Add(lon[j]);
                                }
                            }
                        }

                        for (int i = 0; i < value.Count; i++)
                        {
                            NetCDF("?", dateTime, name, unit, lonValue[i], latValue[i], value[i]);
                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }
    }
}