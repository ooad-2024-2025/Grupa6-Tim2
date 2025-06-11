using AspNetCoreGeneratedDocument;
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
        private readonly IWebHostEnvironment _env;
        private readonly IQRCodeService _qrService;

        public ArtikalsController(ApplicationDbContext context, IWebHostEnvironment env, IQRCodeService qrService)
        {
            _context = context;
            _env = env;
            _qrService = qrService;
        }
        // ******************************************

        // DUPLIRANJE KODA, TREBALO BI IZDVOJITI U ODVOJEN INTERFEJS KASNIJE

        // ******************************************
        private async Task<Korpa?> GetOrCreateKorpaAsync()
        {
            string? userId;
            if(User.Identity?.IsAuthenticated ?? false)
            {
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            else
            {
                userId = HttpContext.Session.GetString("GuestUserId");
                if(userId == null)
                {
                    userId = Guid.NewGuid().ToString();
                    HttpContext.Session.SetString("GuestUserId", userId);
                }
            }

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
            ViewData["Kategorija"] = new SelectList(_context.TipoviOdjece.ToList(), "Id", "Naziv");
            ViewData["Velicine"] = new SelectList(Enum.GetValues(typeof(Velicina)).Cast<Velicina>());
            ViewData["Spolovi"] = new SelectList(Enum.GetValues(typeof(Spol)).Cast<Spol>());

            return View();
        }

        // GET: Artikals
        /*
        public async Task<IActionResult> Index()
        {
            return View(await _context.Artikli.Include(a => a.Kategorija).ToListAsync());
        }*/

        // GET: Artikals
        public async Task<IActionResult> Index(
            string sortOrder,
            int? kategorijaFilter,
            Spol? spolFilter,
            Velicina? velicinaFilter,
            string materijal)
        {
            // Čuvanje trenutnih filter vrijednosti za view
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentKategorija"] = kategorijaFilter;
            ViewData["CurrentSpol"] = spolFilter;
            ViewData["CurrentVelicina"] = velicinaFilter;
            ViewData["CurrentMaterijal"] = materijal;

            // Sortiranje parametri
            ViewData["CijenaSortParm"] = String.IsNullOrEmpty(sortOrder) ? "cijena_desc" : "";
            ViewData["CijenaAscSortParm"] = sortOrder == "cijena_desc" ? "cijena_asc" : "cijena_desc";

            // Dohvatiti sve artikle sa kategorijama
            var artikli = _context.Artikli.Include(a => a.Kategorija).AsQueryable();

            // Filtriranje
            if (kategorijaFilter.HasValue)
            {
                artikli = artikli.Where(a => a.KategorijaId == kategorijaFilter);
            }

            if (spolFilter.HasValue)
            {
                artikli = artikli.Where(a => a.Spol == spolFilter);
            }

            if (velicinaFilter.HasValue)
            {
                artikli = artikli.Where(a => a.Velicina == velicinaFilter);
            }

            if (!String.IsNullOrEmpty(materijal))
            {
                artikli = artikli.Where(a => a.Materijal.Contains(materijal));
            }

            // Sortiranje
            switch (sortOrder)
            {
                case "cijena_desc":
                    artikli = artikli.OrderByDescending(a => a.Cijena);
                    break;
                case "cijena_asc":
                    artikli = artikli.OrderBy(a => a.Cijena);
                    break;
                default:
                    artikli = artikli.OrderBy(a => a.Id);
                    break;
            }

            // Priprema podataka za dropdown liste
            ViewData["Kategorije"] = new SelectList(
                await _context.TipoviOdjece.ToListAsync(),
                "Id",
                "Naziv",
                kategorijaFilter);
            ViewData["Spolovi"] = new SelectList(
                Enum.GetValues(typeof(Spol)).Cast<Spol>(),
                spolFilter);
            ViewData["Velicine"] = new SelectList(
                Enum.GetValues(typeof(Velicina)).Cast<Velicina>(),
                velicinaFilter);

            return View(await artikli.ToListAsync());
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("KategorijaId,Cijena,Materijal,Velicina,Spol,Opis, Kolicina")] Artikal artikal, IFormFile? Slika)
        {

            foreach (var modelError in ModelState)
            {
                Console.WriteLine($"Key: {modelError.Key}, Errors: {string.Join(", ", modelError.Value.Errors.Select(e => e.ErrorMessage))}");
            }

            if (ModelState.IsValid)
            {
                artikal.Kategorija = await _context.TipoviOdjece.FirstOrDefaultAsync(t => t.Id == artikal.KategorijaId);
                artikal.SlikaUrl = "/images/ArtikalDefault.png";

                if(Slika != null && Slika.Length > 0)
                {
                    var uploads = Path.Combine(_env.WebRootPath, "images", "artikli");
                    Directory.CreateDirectory(uploads);
                    var filename = $"{Guid.NewGuid()}{Path.GetExtension(Slika.FileName)}";
                    var filepath = Path.Combine(uploads, filename);

                    using var stream = new FileStream(filepath, FileMode.Create);
                    await Slika.CopyToAsync(stream);

                    artikal.SlikaUrl = $"/images/artikli/{filename}";
                }
                
                _context.Add(artikal);
                await _context.SaveChangesAsync();          

                var url = Url.Action("Details", "Artikals", new { id = artikal.Id }, Request.Scheme);
                string dataUri = _qrService.GenerateQrCodeBase64(url);
                var q = new QRKod
                {
                    ArtikalId = artikal.Id,
                    TipKoda = QRKodTip.OPISARTIKLA,
                    DatumKreiranja = DateTime.UtcNow,
                    DatumIsteka = DateTime.UtcNow.AddYears(1),
                    IsAktivan = true,
                    DataPayload = dataUri
                };

                _context.QRKodovi.Add(q);
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
        /*public async Task<IActionResult> Edit(int id, [Bind("Id,KategorijaId,Cijena,Materijal,Velicina,Spol,Opis, Kolicina")] Artikal artikal, int KategorijaId, IFormFile? Slika)
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
        }*/

        public async Task<IActionResult> Edit(int id, [Bind("Id,KategorijaId,Cijena,Materijal,Velicina,Spol,Opis,Kolicina")] Artikal artikal, int KategorijaId, IFormFile? Slika)
        {
            if (id != artikal.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var existing = await _context.Artikli.FindAsync(id);
                if (existing == null)
                    return NotFound();

                existing.KategorijaId = artikal.KategorijaId;
                existing.Cijena = artikal.Cijena;
                existing.Materijal = artikal.Materijal;
                existing.Velicina = artikal.Velicina;
                existing.Spol = artikal.Spol;
                existing.Opis = artikal.Opis;
                existing.Kolicina = artikal.Kolicina;

                if (Slika != null && Slika.Length > 0)
                {
                    var uploads = Path.Combine(_env.WebRootPath, "images", "artikli");
                    Directory.CreateDirectory(uploads);
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(Slika.FileName)}";
                    var filePath = Path.Combine(uploads, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await Slika.CopyToAsync(stream);

                    existing.SlikaUrl = $"/images/artikli/{fileName}";
                }

                _context.Update(existing);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

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

            TempData["Dodano"] = "Artikal je dodan u korpu!";
            return RedirectToAction(nameof(Index));
        }
    }
}