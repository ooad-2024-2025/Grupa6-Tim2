using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DressCode.Data;
using DressCode.Models;
using System.Security.Claims;

namespace DressCode.Controllers
{
    public class KorpaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KorpaController(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task<Korpa?> GetOrCreateKorpaAsync()
        {
            if (!User.Identity.IsAuthenticated)
                return null;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var korpa = await _context.Korpe
                .FirstOrDefaultAsync(c => c.KorisnikID == userId && c.IsAktivna);

            if (korpa == null)
            {
                korpa = new Korpa
                {
                    KorisnikID = userId,
                    IsAktivna = true,
                    UkupnaCijena = 0
                };
                _context.Korpe.Add(korpa);
                await _context.SaveChangesAsync();
            }
            return korpa;
        }


        // GET: Korpa
        public async Task<IActionResult> Index()
        {
            var korpa = await GetOrCreateKorpaAsync();
            var links = await _context.KorpaStavkeKorpe
                .Where(x => x.KorpaId == korpa.Id)
                .ToListAsync();
            var stavkeDto = new List<KorpaStavkaDto>();
            foreach (var link in links)
            {
                var stavka = await _context.StavkeKorpe.FindAsync(link.StavkaKorpeId);
                var artikal = await _context.Artikli.FindAsync(stavka.ArtikalId);
                stavkeDto.Add(new KorpaStavkaDto
                {
                    StavkaKorpeId = stavka.Id,
                    ArtikalNaziv = artikal?.Opis ?? "(nepoznato)",
                    Kolicina = stavka.Kolicina,
                    CijenaPoKomadu = stavka.CijenaPoKomadu
                });
            }
            var vm = new KorpaViewModel
            {
                KorpaId = korpa.Id,
                UkupnaCijena = korpa.UkupnaCijena,
                IsAktivna = korpa.IsAktivna,
                Stavke = stavkeDto
            };

            return View(vm);
        }

        // GET: Korpa/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var korpa = await _context.Korpe
                .FirstOrDefaultAsync(m => m.Id == id);
            if (korpa == null)
            {
                return NotFound();
            }

            return View(korpa);
        }

        // GET: Korpa/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Korpa/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,KorisnikID,UkupnaCijena,IsAktivna")] Korpa korpa)
        {
            if (ModelState.IsValid)
            {
                _context.Add(korpa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(korpa);
        }

        // GET: Korpa/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var korpa = await _context.Korpe.FindAsync(id);
            if (korpa == null)
            {
                return NotFound();
            }
            return View(korpa);
        }

        // POST: Korpa/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,KorisnikID,UkupnaCijena,IsAktivna")] Korpa korpa)
        {
            if (id != korpa.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(korpa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KorpaExists(korpa.Id))
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
            return View(korpa);
        }

        // GET: Korpa/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var korpa = await _context.Korpe
                .FirstOrDefaultAsync(m => m.Id == id);
            if (korpa == null)
            {
                return NotFound();
            }

            return View(korpa);
        }

        // POST: Korpa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var korpa = await _context.Korpe.FindAsync(id);
            if (korpa != null)
            {
                _context.Korpe.Remove(korpa);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KorpaExists(int id)
        {
            return _context.Korpe.Any(e => e.Id == id);
        }

        // GET: Korpa/Edit/5
        public async Task<IActionResult> IzbaciElement(int? id)
        {
            if (id == null)
                return NotFound();

            var korpa = await _context.Korpe.FindAsync(id);
            if (korpa == null)
                return NotFound();

            var links = await _context.KorpaStavkeKorpe
                .Where(x => x.KorpaId == korpa.Id)
                .ToListAsync();

            var stavkeDto = new List<KorpaStavkaDto>();
            foreach (var link in links)
            {
                var stavka = await _context.StavkeKorpe.FindAsync(link.StavkaKorpeId);
                var artikal = await _context.Artikli.FindAsync(stavka.ArtikalId);
                stavkeDto.Add(new KorpaStavkaDto
                {
                    StavkaKorpeId = stavka.Id,
                    ArtikalNaziv = artikal?.Opis ?? "(nepoznato)",
                    Kolicina = stavka.Kolicina,
                    CijenaPoKomadu = stavka.CijenaPoKomadu
                });
            }

            var vm = new KorpaViewModel
            {
                KorpaId = korpa.Id,
                UkupnaCijena = korpa.UkupnaCijena,
                IsAktivna = korpa.IsAktivna,
                Stavke = stavkeDto
            };

            return View(vm);
        }

    }
}
