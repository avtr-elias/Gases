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
    public class GDataTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GDataTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: GDataTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.GDataType.ToListAsync());
        }

        // GET: GDataTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gDataType = await _context.GDataType
                .SingleOrDefaultAsync(m => m.Id == id);
            if (gDataType == null)
            {
                return NotFound();
            }

            return View(gDataType);
        }

        // GET: GDataTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GDataTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] GDataType gDataType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gDataType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gDataType);
        }

        // GET: GDataTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gDataType = await _context.GDataType.SingleOrDefaultAsync(m => m.Id == id);
            if (gDataType == null)
            {
                return NotFound();
            }
            return View(gDataType);
        }

        // POST: GDataTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] GDataType gDataType)
        {
            if (id != gDataType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gDataType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GDataTypeExists(gDataType.Id))
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
            return View(gDataType);
        }

        // GET: GDataTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gDataType = await _context.GDataType
                .SingleOrDefaultAsync(m => m.Id == id);
            if (gDataType == null)
            {
                return NotFound();
            }

            return View(gDataType);
        }

        // POST: GDataTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gDataType = await _context.GDataType.SingleOrDefaultAsync(m => m.Id == id);
            _context.GDataType.Remove(gDataType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GDataTypeExists(int id)
        {
            return _context.GDataType.Any(e => e.Id == id);
        }
    }
}
