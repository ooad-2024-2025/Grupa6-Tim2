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
    public class ArtikalsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ArtikalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Artikals
        public async Task<IActionResult> Index()
        {
            return View(await _context.Artikli.ToListAsync());
        }

        // GET: Artikals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artikal = await _context.Artikli
                .FirstOrDefaultAsync(m => m.Id == id);
            if (artikal == null)
            {
                return NotFound();
            }

            return View(artikal);
        }

        // GET: Artikals/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Artikals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Cijena,Materijal,Velicina,Spol,Opis")] Artikal artikal)
        {
            if (ModelState.IsValid)
            {
                _context.Add(artikal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(artikal);
        }

        // GET: Artikals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artikal = await _context.Artikli.FindAsync(id);
            if (artikal == null)
            {
                return NotFound();
            }
            return View(artikal);
        }

        // POST: Artikals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Cijena,Materijal,Velicina,Spol,Opis")] Artikal artikal)
        {
            if (id != artikal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(artikal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArtikalExists(artikal.Id))
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
            return View(artikal);
        }

        // GET: Artikals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artikal = await _context.Artikli
                .FirstOrDefaultAsync(m => m.Id == id);
            if (artikal == null)
            {
                return NotFound();
            }

            return View(artikal);
        }

        // POST: Artikals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var artikal = await _context.Artikli.FindAsync(id);
            if (artikal != null)
            {
                _context.Artikli.Remove(artikal);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArtikalExists(int id)
        {
            return _context.Artikli.Any(e => e.Id == id);
        }
    }
}
