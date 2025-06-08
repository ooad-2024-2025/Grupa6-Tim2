using DressCode.Data;
using DressCode.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DressCode.Controllers
{
    public class ArtikalsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ArtikalsController(ApplicationDbContext context)
        {
            _context = context;
        }
        // ******************************************

        // DUPLIRANJE KODA, TREBALO BI IZDVOJITI U ODVOJEN INTERFEJS KASNIJE

        // ******************************************
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

        // GET: Artikals/Create
        public IActionResult Create()
        {
            // Dohvaćanje svih tipova odjeće iz baze
            Console.WriteLine("GET Create metoda pozvana");
            ViewData["Kategorija"] = new SelectList(_context.TipoviOdjece.ToList(), "Id", "Naziv");
            //ViewBag.Kategorija = new SelectList(_context.TipoviOdjece.ToList(), "Id", "Naziv");
            ViewData["Velicine"] = new SelectList(Enum.GetValues(typeof(Velicina)).Cast<Velicina>());
            ViewData["Spolovi"] = new SelectList(Enum.GetValues(typeof(Spol)).Cast<Spol>());
            Console.WriteLine("GET Create metoda zavrsena");
            return View();
        }

        // GET: Artikals
        public async Task<IActionResult> Index()
        {
            return View(await _context.Artikli.Include(a => a.Kategorija).ToListAsync());
        }

        // GET: Artikals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artikal = await _context.Artikli
                .Include(a => a.Kategorija)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (artikal == null)
            {
                return NotFound();
            }

            return View(artikal);
        }

        /*
        // POST: Artikals/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,KategorijaId,Cijena,Materijal,Velicina,Spol,Opis")] Artikal artikal)
        {
            if (ModelState.IsValid)
            {
                _context.Add(artikal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Kategorija"] = new SelectList(_context.TipoviOdjece.ToList(), "Id", "Naziv", artikal.KategorijaId);
            //ViewBag.Kategorija = new SelectList(_context.TipoviOdjece.ToList(), "Id", "Naziv", artikal.KategorijaId);
            Console.WriteLine("KategorijaId: " + artikal.KategorijaId);
            ViewData["Velicine"] = new SelectList(Enum.GetValues(typeof(Velicina)).Cast<Velicina>(), artikal.Velicina);
            ViewData["Spolovi"] = new SelectList(Enum.GetValues(typeof(Spol)).Cast<Spol>(), artikal.Spol);
            return View(artikal);
        }*/

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("KategorijaId,Cijena,Materijal,Velicina,Spol,Opis")] Artikal artikal)
        {
            // DEBUGGING - dodajte ove linije
            Console.WriteLine($"KategorijaId vrednost: {artikal.KategorijaId}");
            Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

            foreach (var modelError in ModelState)
            {
                Console.WriteLine($"Key: {modelError.Key}, Errors: {string.Join(", ", modelError.Value.Errors.Select(e => e.ErrorMessage))}");
            }

            if (ModelState.IsValid)
            {
                artikal.Kategorija = await _context.TipoviOdjece.FirstOrDefaultAsync(t => t.Id == artikal.KategorijaId);
                _context.Add(artikal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Kategorija"] = new SelectList(_context.TipoviOdjece.ToList(), "Id", "Naziv", artikal.KategorijaId);
            //ViewData["Kategorija"] = new SelectList(_context.TipoviOdjece.ToList(), "Id", "Naziv");
            ViewData["Velicine"] = new SelectList(Enum.GetValues(typeof(Velicina)).Cast<Velicina>(), artikal.Velicina);
            ViewData["Spolovi"] = new SelectList(Enum.GetValues(typeof(Spol)).Cast<Spol>(), artikal.Spol);
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
            ViewData["Kategorija"] = new SelectList(_context.TipoviOdjece.ToList(), "Id", "Naziv", artikal.KategorijaId);
            ViewData["Velicine"] = new SelectList(Enum.GetValues(typeof(Velicina)).Cast<Velicina>(), artikal.Velicina);
            ViewData["Spolovi"] = new SelectList(Enum.GetValues(typeof(Spol)).Cast<Spol>(), artikal.Spol);
            return View(artikal);
        }

        // POST: Artikals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,KategorijaId,Cijena,Materijal,Velicina,Spol,Opis")] Artikal artikal, int KategorijaId)
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
            //ViewBag.TipoviOdjece = _context.TipoviOdjece.ToList();
            ViewData["Kategorija"] = new SelectList(_context.TipoviOdjece.ToList(), "Id", "Naziv", artikal.KategorijaId);
            ViewData["Velicine"] = new SelectList(Enum.GetValues(typeof(Velicina)).Cast<Velicina>(), artikal.Velicina);
            ViewData["Spolovi"] = new SelectList(Enum.GetValues(typeof(Spol)).Cast<Spol>(), artikal.Spol);
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

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DodajUKorpu(int id)
        {
            var artikal = await _context.Artikli.FindAsync(id);
            if (artikal == null) return NotFound();

            var korpa = await GetOrCreateKorpaAsync();

            var stavka = new StavkaKorpe
            {
                ArtikalId = artikal.Id,
                Velicina = artikal.Velicina,
                Kolicina = 1,
                CijenaPoKomadu = artikal.Cijena
            };
            _context.StavkeKorpe.Add(stavka);
            await _context.SaveChangesAsync();
            var link = new KorpaStavkaKorpe
            {
                KorpaId = korpa.Id,
                StavkaKorpeId = stavka.Id
            };
            _context.KorpaStavkeKorpe.Add(link);
            korpa.UkupnaCijena += stavka.Kolicina * stavka.CijenaPoKomadu;
            _context.Korpe.Update(korpa);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
