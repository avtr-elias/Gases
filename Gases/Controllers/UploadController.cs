using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        ApplicationContext _context;
        IHostingEnvironment _appEnvironment;

        public UploadController(ApplicationContext context, IHostingEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Index()
        {
            return View(_context.Files.ToList());
            //return View();
        }
        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> AddFile(IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                // путь к папке Files
                string path = "/Uploaded/" + uploadedFile.FileName;
                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                UploadModel file = new UploadModel { Name = uploadedFile.FileName, Path = path };
                //_context.Files.Add(file);
                //_context.SaveChanges();
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
                }
            }

            return RedirectToAction("Index");
        }
    }
}