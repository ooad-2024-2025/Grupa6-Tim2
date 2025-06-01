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
    public class KarticaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KarticaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Kartica
        public async Task<IActionResult> Index()
        {
            return View(await _context.Kartice.ToListAsync());
        }

        // GET: Kartica/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kartica = await _context.Kartice
                .FirstOrDefaultAsync(m => m.Id == id);
            if (kartica == null)
            {
                return NotFound();
            }

            return View(kartica);
        }

        // GET: Kartica/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Kartica/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,KorisnikId,BrojKartice,DatumIsteka,ImeVlasnika")] Kartica kartica)
        {
            if (ModelState.IsValid)
            {
                _context.Add(kartica);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(kartica);
        }

        // GET: Kartica/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kartica = await _context.Kartice.FindAsync(id);
            if (kartica == null)
            {
                return NotFound();
            }
            return View(kartica);
        }

        // POST: Kartica/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,KorisnikId,BrojKartice,DatumIsteka,ImeVlasnika")] Kartica kartica)
        {
            if (id != kartica.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kartica);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KarticaExists(kartica.Id))
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
            return View(kartica);
        }

        // GET: Kartica/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kartica = await _context.Kartice
                .FirstOrDefaultAsync(m => m.Id == id);
            if (kartica == null)
            {
                return NotFound();
            }

            return View(kartica);
        }

        // POST: Kartica/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kartica = await _context.Kartice.FindAsync(id);
            if (kartica != null)
            {
                _context.Kartice.Remove(kartica);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KarticaExists(int id)
        {
            return _context.Kartice.Any(e => e.Id == id);
        }
    }
}
