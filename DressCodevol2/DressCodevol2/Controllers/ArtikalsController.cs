using AspNetCoreGeneratedDocument;
using DressCode.Data;
using DressCode.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            ViewData["GrupaId"] = "";

            return View();
        }

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

            var grupirani = await artikli
            .GroupBy(a => a.GrupaId)
            .Select(g => g.OrderBy(a => a.Id).First()) 
            .ToListAsync();

            // Sortiranje
            switch (sortOrder)
            {
                case "cijena_desc":
                    grupirani = grupirani.OrderByDescending(a => a.Cijena).ToList();
                    break;
                case "cijena_asc":
                    grupirani = grupirani.OrderBy(a => a.Cijena).ToList();
                    break;
                default:
                    grupirani = grupirani.OrderBy(a => a.Id).ToList();
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

            return View(grupirani);
        }

        // GET: Artikals/Details/5
        public async Task<IActionResult> Details(string grupaId)
        {
            if (string.IsNullOrEmpty(grupaId))
            {
                return NotFound();
            }

            var artikli = await _context.Artikli
            .Include(a => a.Kategorija)
            .Where(a => a.GrupaId == grupaId)
            .ToListAsync();

                if (!artikli.Any())
                {
                    return NotFound();
                }

            var glavniArtikal = artikli.First();

            ViewBag.DostupneVelicine = artikli
            .Select(a => a.Velicina)
            .Distinct()
            .OrderBy(v => v) // Ovo će sortirati po redoslijedu u enumu
            .ToList();
            ViewBag.SviArtikli = artikli; 

            return View(glavniArtikal);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("KategorijaId,Cijena,Materijal,Velicina,Spol,Opis, Kolicina, GrupaId")] Artikal artikal, IFormFile? Slika)
        {

            foreach (var modelError in ModelState)
            {
                Debug.WriteLine($"Key: {modelError.Key}, Errors: {string.Join(", ", modelError.Value.Errors.Select(e => e.ErrorMessage))}");
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

                //var url = Url.Action("Details", "Artikals", new { grupaId = artikal.GrupaId }, Request.Scheme);
                var url = Url.Action("Details", "Artikals", new { grupaId = artikal.GrupaId }, Request.Scheme);

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

               // return RedirectToAction(nameof(Index));
            }

            ViewData["Kategorija"] = new SelectList(_context.TipoviOdjece.ToList(), "Id", "Naziv", artikal.KategorijaId);
            //ViewData["Kategorija"] = new SelectList(_context.TipoviOdjece.ToList(), "Id", "Naziv");
            ViewData["Velicine"] = new SelectList(Enum.GetValues(typeof(Velicina)).Cast<Velicina>(), artikal.Velicina);
            ViewData["Spolovi"] = new SelectList(Enum.GetValues(typeof(Spol)).Cast<Spol>(), artikal.Spol);
            return RedirectToAction(nameof(Index));
        }
        

       /* [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("KategorijaId,Cijena,Materijal,Velicina,Spol,Opis,Kolicina,GrupaId")] Artikal artikal, IFormFile? Slika)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Postavljanje kategorije
                    artikal.Kategorija = await _context.TipoviOdjece.FirstOrDefaultAsync(t => t.Id == artikal.KategorijaId);

                    // Postavljanje default slike
                    artikal.SlikaUrl = "/images/ArtikalDefault.png";

                    // Upload slike
                    if (Slika != null && Slika.Length > 0)
                    {
                        var uploads = Path.Combine(_env.WebRootPath, "images", "artikli");
                        Directory.CreateDirectory(uploads);
                        var filename = $"{Guid.NewGuid()}{Path.GetExtension(Slika.FileName)}";
                        var filepath = Path.Combine(uploads, filename);

                        using var stream = new FileStream(filepath, FileMode.Create);
                        await Slika.CopyToAsync(stream);
                        artikal.SlikaUrl = $"/images/artikli/{filename}";
                    }

                    // Spremanje artikla
                    _context.Add(artikal);
                    await _context.SaveChangesAsync();

                    // Generiranje QR koda
                    try
                    {
                        var url = Url.Action("Details", "Artikals", new { grupaId = artikal.GrupaId }, "https");
                        string dataUri = _qrService.GenerateQrCodeBase64(url);

                        var qrKod = new QRKod
                        {
                            ArtikalId = artikal.Id,
                            TipKoda = QRKodTip.OPISARTIKLA,
                            DatumKreiranja = DateTime.UtcNow,
                            DatumIsteka = DateTime.UtcNow.AddYears(1),
                            IsAktivan = true,
                            DataPayload = dataUri
                        };

                        _context.QRKodovi.Add(qrKod);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception qrEx)
                    {
                        Console.WriteLine($"QR Code generation failed: {qrEx.Message}");
                        // QR kod nije kritičan, možete nastaviti bez njega
                    }

                    return RedirectToAction(nameof(Index));
                }

                // Ako ModelState nije valjan, prikažite greške
                foreach (var modelError in ModelState)
                {
                    Console.WriteLine($"Key: {modelError.Key}, Errors: {string.Join(", ", modelError.Value.Errors.Select(e => e.ErrorMessage))}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error in Create: {ex.Message}");
                ModelState.AddModelError("", "Došlo je do greške prilikom kreiranja artikla.");
            }

            // Ponovno učitavanje ViewData za dropdown liste
            ViewData["Kategorija"] = new SelectList(_context.TipoviOdjece.ToList(), "Id", "Naziv", artikal.KategorijaId);
            ViewData["Velicine"] = new SelectList(Enum.GetValues(typeof(Velicina)).Cast<Velicina>(), artikal.Velicina);
            ViewData["Spolovi"] = new SelectList(Enum.GetValues(typeof(Spol)).Cast<Spol>(), artikal.Spol);

            return View(artikal);
        }*/

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,KategorijaId,Cijena,Materijal,Velicina,Spol,Opis,Kolicina, GrupaId")] Artikal artikal, int KategorijaId, IFormFile? Slika)
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
                existing.GrupaId = artikal.GrupaId;

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
                var qrKod = await _context.QRKodovi.FirstOrDefaultAsync(q => q.ArtikalId == id);
                if (qrKod != null)
                {
                    _context.QRKodovi.Remove(qrKod);
                }
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
        public async Task<IActionResult> DodajUKorpu(string grupaId, Velicina velicina)
        {
            var artikal = await _context.Artikli.FirstOrDefaultAsync(a => a.GrupaId == grupaId && a.Velicina == velicina);
            if (artikal == null) return NotFound();

            var korpa = await GetOrCreateKorpaAsync();

            var stavka = new StavkaKorpe
            {
                ArtikalId = artikal.Id,
                Velicina = artikal.Velicina,
                Kolicina = 1,
                CijenaPoKomadu = artikal.Cijena,
                GrupaId = artikal.GrupaId
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
            return RedirectToAction("Details", new { grupaId = grupaId }); //RedirectToAction(nameof(Index));
    }

        // GET: Artikals/EditGroup/BK123

        [HttpGet]
        public async Task<IActionResult> EditGroup(string grupaId)
        {
            if (string.IsNullOrEmpty(grupaId)) return NotFound();

            var artikli = await _context.Artikli
                .Where(a => a.GrupaId == grupaId)
                .ToListAsync();

            if(!artikli.Any()) return NotFound();

           
            var vm = new EditGroupViewModel
            {
                GrupaId = grupaId,
                ZajednickaCijena = artikli.First().Cijena,
                ZajednickiMaterijal = artikli.First().Materijal,
                ZajednickiOpis = artikli.First().Opis,
                KategorijaId = artikli.First().KategorijaId,
                Artikli = artikli
            };

            ViewData["Kategorije"] = new SelectList(_context.TipoviOdjece, "Id", "Naziv", artikli.First().KategorijaId);
            return View(vm);
        }

        // POST: Artikals/EditGroup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGroup(EditGroupViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Kategorije"] = new SelectList(_context.TipoviOdjece, "Id", "Naziv", vm.KategorijaId);
                return View(vm);
            }

            // Dohvati sve iz grupe
            var artikli = await _context.Artikli
                .Where(a => a.GrupaId == vm.GrupaId)
                .ToListAsync();

            foreach (var a in artikli)
            {
                a.Cijena = (double)vm.ZajednickaCijena;
                a.Materijal = vm.ZajednickiMaterijal;
                a.Opis = vm.ZajednickiOpis;
                a.KategorijaId = vm.KategorijaId;
                // ne diraj Velicinu i Spol, oni su po-artiklu
            }

            _context.UpdateRange(artikli);
            await _context.SaveChangesAsync();

            var artikalIds = artikli.Select(a => a.Id).ToList();
            var stavke = await _context.StavkeKorpe
                .Where(s => artikalIds.Contains(s.ArtikalId))
                .ToListAsync();

            foreach (var s in stavke)
            {
                var art = artikli.First(a => a.Id == s.ArtikalId);
                s.CijenaPoKomadu = art.Cijena;

            }

            _context.UpdateRange(stavke);
            await _context.SaveChangesAsync();

            var stavkaIds = stavke.Select(s => s.Id).ToList();
            var links = await _context.KorpaStavkeKorpe
                .Where(link => stavkaIds.Contains(link.StavkaKorpeId))
                .ToListAsync();

            // 5) Grupiraj po KorpaId i za svaku košaricu izračunaj novi total
            var linksByCart = links.GroupBy(l => l.KorpaId);
            foreach (var group in linksByCart)
            {
                // Dohvati košaricu
                var korpa = await _context.Korpe.FindAsync(group.Key);
                if (korpa == null) continue;

                // Zbroji sve stavke: CijenaPoKomadu * Kolicina
                double noviTotal = 0;
                foreach (var link in group)
                {
                    var stavka = stavke.First(s => s.Id == link.StavkaKorpeId);
                    noviTotal += stavka.CijenaPoKomadu * stavka.Kolicina;
                }

                korpa.UkupnaCijena = noviTotal;
            }

            // 6) Snimi promjene u Korpa entitetima
            await _context.SaveChangesAsync();


            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteGroup(string? grupaId)
        {
            if (string.IsNullOrEmpty(grupaId))
                return BadRequest();

            // Provjeri da li grupa postoji
            bool ima = await _context.Artikli.AnyAsync(a => a.GrupaId == grupaId);
            if (!ima)
                return NotFound();

            // Prikaži view s potvrdom
            return View(model: grupaId);
        }

        // POST: Artikals/DeleteGroup
        [HttpPost, ActionName("DeleteGroup")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGroupConfirmed(string grupaId)
        {
            if (string.IsNullOrEmpty(grupaId))
                return BadRequest();

            // 1) Dohvati sve artikle iz grupe
            var artikli = await _context.Artikli
                .Where(a => a.GrupaId == grupaId)
                .ToListAsync();

            if (!artikli.Any())
                return NotFound();

            // 2) Prikupi njihove ID-jeve
            var artikalIds = artikli.Select(a => a.Id).ToList();

            // 3) Obriši sve QR kodove za te artikle
            var qrcodes = await _context.QRKodovi
                .Where(q => q.ArtikalId != null && artikalIds.Contains(q.ArtikalId.Value))
                .ToListAsync();
            _context.QRKodovi.RemoveRange(qrcodes);

            // 4) Pronađi sve stavke i linkove koje brišemo
            var stavkeZaBrisanje = await _context.StavkeKorpe
                .Where(s => artikalIds.Contains(s.ArtikalId))
                .ToListAsync();

            var stavkaIds = stavkeZaBrisanje.Select(s => s.Id).ToList();

            var linkoviZaBrisanje = await _context.KorpaStavkeKorpe
                .Where(link => stavkaIds.Contains(link.StavkaKorpeId))
                .ToListAsync();

            // 5) Skupi sve pogođene KorpaId
            var pogodeneKorpe = linkoviZaBrisanje
                .Select(l => l.KorpaId)
                .Distinct()
                .ToList();

            // 6) Obriši veze i stavke
            _context.KorpaStavkeKorpe.RemoveRange(linkoviZaBrisanje);
            _context.StavkeKorpe.RemoveRange(stavkeZaBrisanje);

            // 7) Obriši artikle
            _context.Artikli.RemoveRange(artikli);
            await _context.SaveChangesAsync();

            // 8) Preračunaj UkupnaCijena za svaku pogođenu košaricu
            foreach (var korpaId in pogodeneKorpe)
            {
                // Dohvati preostale linkove iz košarice
                var preostaliLinkovi = await _context.KorpaStavkeKorpe
                    .Where(link => link.KorpaId == korpaId)
                    .ToListAsync();

                // Sumiraj njihovu trenutnu vrijednost
                double noviTotal = 0;
                if (preostaliLinkovi.Any())
                {
                    var preostaleStavke = await _context.StavkeKorpe
                        .Where(s => preostaliLinkovi.Select(l => l.StavkaKorpeId).Contains(s.Id))
                        .ToListAsync();

                    noviTotal = preostaleStavke.Sum(s => s.CijenaPoKomadu * s.Kolicina);
                }

                var korpa = await _context.Korpe.FindAsync(korpaId);
                if (korpa != null)
                {
                    korpa.UkupnaCijena = noviTotal;
                    _context.Korpe.Update(korpa);
                }
            }

            // 9) Snimi sve promjene
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }




    }
}