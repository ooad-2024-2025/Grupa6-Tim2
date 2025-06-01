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
    public class AdresaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdresaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Adresa
        public async Task<IActionResult> Index()
        {
            return View(await _context.Adrese.ToListAsync());
        }

        // GET: Adresa/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adresa = await _context.Adrese
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adresa == null)
            {
                return NotFound();
            }

            return View(adresa);
        }

        // GET: Adresa/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Adresa/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ulica,Broj,Grad,Drzava")] Adresa adresa)
        {
            if (ModelState.IsValid)
            {
                _context.Add(adresa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(adresa);
        }

        // GET: Adresa/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adresa = await _context.Adrese.FindAsync(id);
            if (adresa == null)
            {
                return NotFound();
            }
            return View(adresa);
        }

        // POST: Adresa/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ulica,Broj,Grad,Drzava")] Adresa adresa)
        {
            if (id != adresa.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adresa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdresaExists(adresa.Id))
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
            return View(adresa);
        }

        // GET: Adresa/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adresa = await _context.Adrese
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adresa == null)
            {
                return NotFound();
            }

            return View(adresa);
        }

        // POST: Adresa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var adresa = await _context.Adrese.FindAsync(id);
            if (adresa != null)
            {
                _context.Adrese.Remove(adresa);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdresaExists(int id)
        {
            return _context.Adrese.Any(e => e.Id == id);
        }
    }
}
