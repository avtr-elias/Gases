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
    public class NetCDFsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NetCDFsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: NetCDFs
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.NetCDF.Include(n => n.Gase);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: NetCDFs/Details/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var netCDF = await _context.NetCDF
                .Include(n => n.Gase)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (netCDF == null)
            {
                return NotFound();
            }

            return View(netCDF);
        }

        // GET: NetCDFs/Create
        //public IActionResult Create()
        //{
        //    ViewData["GaseId"] = new SelectList(_context.Gase, "Id", "Id");
        //    return View();
        //}

        // POST: NetCDFs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,GaseId,Date,Name,Unit,Longtitude,Latitude,Value")] NetCDF netCDF)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(netCDF);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["GaseId"] = new SelectList(_context.Gase, "Id", "Id", netCDF.GaseId);
        //    return View(netCDF);
        //}

        // GET: NetCDFs/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var netCDF = await _context.NetCDF.SingleOrDefaultAsync(m => m.Id == id);
            if (netCDF == null)
            {
                return NotFound();
            }
            ViewData["GaseId"] = new SelectList(_context.Gase, "Id", "Id", netCDF.GaseId);
            return View(netCDF);
        }

        // POST: NetCDFs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GaseId,Date,Name,Unit,Longtitude,Latitude,Value")] NetCDF netCDF)
        {
            if (id != netCDF.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(netCDF);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NetCDFExists(netCDF.Id))
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
            ViewData["GaseId"] = new SelectList(_context.Gase, "Id", "Id", netCDF.GaseId);
            return View(netCDF);
        }

        // GET: NetCDFs/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var netCDF = await _context.NetCDF
                .Include(n => n.Gase)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (netCDF == null)
            {
                return NotFound();
            }

            return View(netCDF);
        }

        // POST: NetCDFs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var netCDF = await _context.NetCDF.SingleOrDefaultAsync(m => m.Id == id);
            _context.NetCDF.Remove(netCDF);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NetCDFExists(int id)
        {
            return _context.NetCDF.Any(e => e.Id == id);
        }
    }
}
