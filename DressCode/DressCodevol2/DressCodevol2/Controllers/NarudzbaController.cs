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
    public class NarudzbaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NarudzbaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Narudzba
        public async Task<IActionResult> Index()
        {
           var narudzbe = await _context.Narudzbe.AsNoTracking().ToListAsync();

            var model = new List<NarudzbaIndexViewModel>();

            foreach (var n in narudzbe)
            {
                var korisnik = await _context.Korisnik
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == n.KorisnikId);

                var adresa = await _context.Adrese
                    .FirstOrDefaultAsync(a => a.Id == n.AdresaId);

                var narudzbaStavke = await _context.NarudzbaStavka
                        .Where(os => os.NarudzbaId == n.Id)
                        .ToListAsync();

                var artikli = new List<string>();
                foreach (var stavka in narudzbaStavke)
                {
                    artikli.Add($"{stavka.GrupaId}, Velicina: {stavka.Velicina} x {stavka.Kolicina}");
                    
                }

                model.Add(new NarudzbaIndexViewModel
                {
                    NarudzbaId = n.Id,
                    ImePrezime = $"{korisnik.Ime} {korisnik.Prezime}",
                    DatumKreiranja = n.DatumKreiranja.ToString("dd-MM-yyyy"),
                    Adresa = $"{adresa.Ulica}, {adresa.Grad} ({adresa.Drzava})",
                    Artikli = artikli
                });
                
            }

            return View(model);
        }

        // GET: Narudzba/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var narudzba = await _context.Narudzbe
                .FirstOrDefaultAsync(m => m.Id == id);
            if (narudzba == null)
            {
                return NotFound();
            }

            return View(narudzba);
        }

        // POST: Narudzba/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var narudzba = await _context.Narudzbe.FindAsync(id);
            if (narudzba != null)
            {
                _context.Narudzbe.Remove(narudzba);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NarudzbaExists(int id)
        {
            return _context.Narudzbe.Any(e => e.Id == id);
        }
    }
}
