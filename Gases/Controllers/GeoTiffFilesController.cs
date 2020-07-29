using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gases.Data;
using Gases.Models;

namespace Gases.Controllers
{
    public class GeoTiffFilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GeoTiffFilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: GeoTiffFiles
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.GeoTiffFile.Include(g => g.Gase);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: GeoTiffFiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var geoTiffFile = await _context.GeoTiffFile
                .Include(g => g.Gase)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (geoTiffFile == null)
            {
                return NotFound();
            }

            return View(geoTiffFile);
        }

        //// GET: GeoTiffFiles/Create
        //public IActionResult Create()
        //{
        //    ViewData["GaseId"] = new SelectList(_context.Gase, "Id", "Id");
        //    return View();
        //}

        //// POST: GeoTiffFiles/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,GaseId,Year,VerticalSlice,Name")] GeoTiffFile geoTiffFile)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(geoTiffFile);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["GaseId"] = new SelectList(_context.Gase, "Id", "Id", geoTiffFile.GaseId);
        //    return View(geoTiffFile);
        //}

        // GET: GeoTiffFiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var geoTiffFile = await _context.GeoTiffFile.SingleOrDefaultAsync(m => m.Id == id);
            if (geoTiffFile == null)
            {
                return NotFound();
            }
            ViewData["GaseId"] = new SelectList(_context.Gase.Where(g => g.Id != 4), "Id", "Formula", geoTiffFile.GaseId); //not show NO2
            return View(geoTiffFile);
        }

        // POST: GeoTiffFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GaseId,Year,VerticalSlice,Name")] GeoTiffFile geoTiffFile)
        {
            if (id != geoTiffFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(geoTiffFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GeoTiffFileExists(geoTiffFile.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GaseId"] = new SelectList(_context.Gase.Where(g => g.Id != 4), "Id", "Formula", geoTiffFile.GaseId); //not show NO2
            return View(geoTiffFile);
        }

        // GET: GeoTiffFiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var geoTiffFile = await _context.GeoTiffFile
                .Include(g => g.Gase)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (geoTiffFile == null)
            {
                return NotFound();
            }

            return View(geoTiffFile);
        }

        // POST: GeoTiffFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var geoTiffFile = await _context.GeoTiffFile.SingleOrDefaultAsync(m => m.Id == id);
            _context.GeoTiffFile.Remove(geoTiffFile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GeoTiffFileExists(int id)
        {
            return _context.GeoTiffFile.Any(e => e.Id == id);
        }
    }
}
