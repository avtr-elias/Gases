using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Gases.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Gases.Data;
using System.IO;

namespace Gases.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.Gases = _context.Gase.Where(g => g.Id != 4).ToList(); //not show NO2
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            // load layers
            string path = "E:\\Documents\\Google Drive\\Notes\\Work\\Gases\\TAM\\";
            string[] files = Directory.GetFiles(path, "*.tif", SearchOption.AllDirectories);
            List<Layer> layers = new List<Layer>();

            foreach(string file in files)
            {
                DirectoryInfo di = new DirectoryInfo(file);
                string s_name = Path.GetFileNameWithoutExtension(di.Name),
                    s_year = di.Parent.Name,
                    s_gas = di.Parent.Parent.Name,
                    s_vertical = di.Parent.Parent.Parent.Name;
                Layer layer = new Layer()
                {
                    GaseId = _context.Gase.FirstOrDefault(g => g.Formula == s_gas).Id,
                    GDataTypeId = 2,
                    GeoServerName = s_name,
                    VerticalSlice = Convert.ToDecimal(s_vertical),
                    Year = Convert.ToInt32(s_year)
                };
                layers.Add(layer);
            }

            //string test = "";
            //foreach(Layer layer in layers)
            //{
            //    test += layer.GaseId + "\t" + layer.GeoServerName + "\t" + layer.VerticalSlice + "\t" + layer.Year + Environment.NewLine;
            //}

            _context.Layer.AddRange(layers);
            _context.SaveChanges();

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Administrator()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }
    }
}
