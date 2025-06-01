using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DressCode.Data;
using DressCode.Models;

namespace DressCode.Controllers
{
    public class StavkaKorpeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StavkaKorpeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StavkaKorpe
        public async Task<IActionResult> Index()
        {
            return View(await _context.StavkeKorpe.ToListAsync());
        }

        // GET: StavkaKorpe/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stavkaKorpe = await _context.StavkeKorpe
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stavkaKorpe == null)
            {
                return NotFound();
            }

            return View(stavkaKorpe);
        }

        // GET: StavkaKorpe/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: StavkaKorpe/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Velicina,Kolicina,CijenaPoKomadu")] StavkaKorpe stavkaKorpe)
        {
            if (ModelState.IsValid)
            {
                _context.Add(stavkaKorpe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(stavkaKorpe);
        }

        // GET: StavkaKorpe/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stavkaKorpe = await _context.StavkeKorpe.FindAsync(id);
            if (stavkaKorpe == null)
            {
                return NotFound();
            }
            return View(stavkaKorpe);
        }

        // POST: StavkaKorpe/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Velicina,Kolicina,CijenaPoKomadu")] StavkaKorpe stavkaKorpe)
        {
            if (id != stavkaKorpe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stavkaKorpe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StavkaKorpeExists(stavkaKorpe.Id))
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
            return View(stavkaKorpe);
        }

        // GET: StavkaKorpe/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stavkaKorpe = await _context.StavkeKorpe
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stavkaKorpe == null)
            {
                return NotFound();
            }

            return View(stavkaKorpe);
        }

        // POST: StavkaKorpe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var stavkaKorpe = await _context.StavkeKorpe.FindAsync(id);
            if (stavkaKorpe != null)
            {
                _context.StavkeKorpe.Remove(stavkaKorpe);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StavkaKorpeExists(string id)
        {
            return _context.StavkeKorpe.Any(e => e.Id == id);
        }
    }
}
