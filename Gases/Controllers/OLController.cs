using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Gases.Data;
using Gases.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gases.Controllers
{
    public class OLController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OLController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult ViewGases()
        {
            var gaseFormula = _context.Gase.OrderBy(m => m.Formula);
            ViewBag.GaseFormula = new SelectList(gaseFormula, "Formula", "Formula");
            //var year = _context.GeoTiffFile.Where(m => m.GaseId == _context.Gase.OrderBy(ms => ms.Id).FirstOrDefault().Id).OrderBy(m => m.Year);
            //ViewBag.Year = new SelectList(year, "Year", "Year");
            //var month = _context.GeoTiffFile.Where(m => m.GaseId == _context.Gase.OrderBy(ms => ms.Id).FirstOrDefault().Id).OrderBy(m => m.Month);
            //ViewBag.Month = new SelectList(month, "Month", "Month");
            string formula = gaseFormula.FirstOrDefault().Formula;
            var year = _context.GeoTiffFile.Where(m => m.Gase.Formula == formula).GroupBy(m => m.Year).Select(m => new { Year = m.Key }).OrderBy(m => m.Year);
            ViewBag.Year = new SelectList(year, "Year", "Year");
            var month = _context.GeoTiffFile.Where(m => m.Gase.Formula == formula).GroupBy(m => m.Month).Select(m => new { Month = m.Key }).OrderBy(m => m.Month);
            ViewBag.Month = new SelectList(month, "Month", "Month");
            //var year = _context.GeoTiffFile.OrderBy(m => m.Year);
            //ViewBag.Year = new SelectList(year, "Year", "Year");
            //var month = _context.GeoTiffFile.OrderBy(m => m.Month);
            //ViewBag.Month = new SelectList(month, "Month", "Month");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public JsonResult GetYearByGaseFormula(string GaseFormula)
        {
            var year = _context.GeoTiffFile
                .Where(m => m.Gase.Formula == GaseFormula);
            JsonResult result = new JsonResult(year);
            return result;
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public JsonResult GetMonthByYear(string Year)
        {
            var month = _context.GeoTiffFile
                .Where(m => m.Year == Year);
            JsonResult result = new JsonResult(month);
            return result;
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public JsonResult GetName(string Year,
            string Month,
            string GaseFormula)
        {
            string gase = GaseFormula.ToUpper();
            int idGase = _context.Gase.Where(d => d.Formula == gase).First().Id;
            string name = _context.GeoTiffFile.Where(m => m.Year == Year).Where(m => m.Month == Month).Where(m => m.GaseId == idGase).First().Name;
            name = name.Remove(name.Length - 4, 4);
            JsonResult result = new JsonResult(name);
            return result;
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public JsonResult GetValue(string Year,
            string Month,
            string GaseFormula)
        {
            string gase = GaseFormula.ToUpper();
            int idGase = _context.Gase.Where(d => d.Formula == gase).First().Id;
            int dateYear = Convert.ToInt32(Year);
            int dateMonth = Convert.ToInt32(Month);
            DateTime dateTime = new DateTime(dateYear, dateMonth, 01);

            var coordinats = _context.NetCDF.Where(m => m.Date == dateTime).Where(m => m.GaseId == idGase);
            JsonResult result = new JsonResult(coordinats);
            return result;
        }
    }
}