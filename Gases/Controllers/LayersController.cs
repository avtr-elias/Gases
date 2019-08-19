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
    public class LayersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LayersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Layers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Layer.Include(l => l.GDataType).Include(l => l.Gase);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Layers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var layer = await _context.Layer
                .Include(l => l.GDataType)
                .Include(l => l.Gase)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (layer == null)
            {
                return NotFound();
            }

            return View(layer);
        }

        // GET: Layers/Create
        //public IActionResult Create()
        //{
        //    ViewData["GDataTypeId"] = new SelectList(_context.GDataType, "Id", "Name");
        //    ViewData["GaseId"] = new SelectList(_context.Gase, "Id", "Name");
        //    return View();
        //}

        // POST: Layers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,GeoServerName,FileNameWithPath,GeoServerStyle,NameKK,NameRU,NameEN,GDataTypeId,GaseId,VerticalSlice,Year")] Layer layer)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(layer);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["GDataTypeId"] = new SelectList(_context.GDataType, "Id", "Name", layer.GDataTypeId);
        //    ViewData["GaseId"] = new SelectList(_context.Gase, "Id", "Name", layer.GaseId);
        //    return View(layer);
        //}

        // GET: Layers/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var layer = await _context.Layer.SingleOrDefaultAsync(m => m.Id == id);
        //    if (layer == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["GDataTypeId"] = new SelectList(_context.GDataType, "Id", "Name", layer.GDataTypeId);
        //    ViewData["GaseId"] = new SelectList(_context.Gase, "Id", "Name", layer.GaseId);
        //    return View(layer);
        //}

        // POST: Layers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,GeoServerName,FileNameWithPath,GeoServerStyle,NameKK,NameRU,NameEN,GDataTypeId,GaseId,VerticalSlice,Year")] Layer layer)
        //{
        //    if (id != layer.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(layer);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!LayerExists(layer.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["GDataTypeId"] = new SelectList(_context.GDataType, "Id", "Name", layer.GDataTypeId);
        //    ViewData["GaseId"] = new SelectList(_context.Gase, "Id", "Name", layer.GaseId);
        //    return View(layer);
        //}

        //// GET: Layers/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var layer = await _context.Layer
        //        .Include(l => l.GDataType)
        //        .Include(l => l.Gase)
        //        .SingleOrDefaultAsync(m => m.Id == id);
        //    if (layer == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(layer);
        //}

        //// POST: Layers/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var layer = await _context.Layer.SingleOrDefaultAsync(m => m.Id == id);
        //    _context.Layer.Remove(layer);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool LayerExists(int id)
        {
            return _context.Layer.Any(e => e.Id == id);
        }
    }
}
