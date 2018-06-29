using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gases.Data;
using Gases.Models;
using Microsoft.AspNetCore.Authorization;

namespace Gases.Controllers
{
    public class GasesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GasesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Gases
        public async Task<IActionResult> Index()
        {
            return View(await _context.Gase.ToListAsync());
        }

        // GET: Gases/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gase = await _context.Gase
                .SingleOrDefaultAsync(m => m.Id == id);
            if (gase == null)
            {
                return NotFound();
            }

            return View(gase);
        }

        // GET: Gases/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Gases/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([Bind("Id,Name,Formula")] Gase gase)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gase);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gase);
        }

        // GET: Gases/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gase = await _context.Gase.SingleOrDefaultAsync(m => m.Id == id);
            if (gase == null)
            {
                return NotFound();
            }
            return View(gase);
        }

        // POST: Gases/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Formula")] Gase gase)
        {
            if (id != gase.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gase);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GaseExists(gase.Id))
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
            return View(gase);
        }

        // GET: Gases/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gase = await _context.Gase
                .SingleOrDefaultAsync(m => m.Id == id);
            if (gase == null)
            {
                return NotFound();
            }

            return View(gase);
        }

        // POST: Gases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gase = await _context.Gase.SingleOrDefaultAsync(m => m.Id == id);
            _context.Gase.Remove(gase);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GaseExists(int id)
        {
            return _context.Gase.Any(e => e.Id == id);
        }
    }
}
