﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DressCode.Data;
using DressCode.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace DressCode.Controllers
{
    public class KorpaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KorpaController(ApplicationDbContext context)
        {
            _context = context;
        }

        private const string GuestSessionKey = "GuestUserId";

        private string? PeekGuestId() =>
            HttpContext.Session.GetString(GuestSessionKey);

        private string GetOrCreateGuestId()
        {
            var gid = PeekGuestId();
            if (!string.IsNullOrEmpty(gid))
                return gid;

            gid = Guid.NewGuid().ToString();
            HttpContext.Session.SetString(GuestSessionKey, gid);
            return gid;
        }

        private async Task<Korpa> GetOrCreateKorpaAsync()
        {
            bool isAuth = User.Identity?.IsAuthenticated == true;

            if (isAuth)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

                // 1) Pokušaj dohvatiti aktivnu korisničku korpu
                var userKorpa = await _context.Korpe
                    .FirstOrDefaultAsync(k => k.KorisnikID == userId && k.IsAktivna);

                // 2) Ako korisnička korpa postoji i ima bar jednu stavku, odmah je vraćamo
                if (userKorpa != null)
                {
                    var itemCount = await _context.KorpaStavkeKorpe
                        .CountAsync(l => l.KorpaId == userKorpa.Id);
                    if (itemCount > 0)
                        return userKorpa;
                }

                // 3) Inače (korpa ne postoji ili je prazna) pokušaj merge s guest korpom
                var guestId = PeekGuestId();
                if (!string.IsNullOrEmpty(guestId))
                {
                    var guestKorpa = await _context.Korpe
                        .FirstOrDefaultAsync(k => k.KorisnikID == guestId && k.IsAktivna);

                    if (guestKorpa != null)
                    {
                        // 3a) Ako userKorpa ne postoji, samo preimenuj guestKorpu
                        if (userKorpa == null)
                        {
                            guestKorpa.KorisnikID = userId;
                            _context.Korpe.Update(guestKorpa);
                            await _context.SaveChangesAsync();
                            HttpContext.Session.Remove(GuestSessionKey);
                            return guestKorpa;
                        }
                        // 3b) Ako userKorpa postoji ali je prazna, prebaci stavke iz guestKorpe
                        var guestLinks = await _context.KorpaStavkeKorpe
                            .Where(l => l.KorpaId == guestKorpa.Id)
                            .ToListAsync();

                        // prespoji svaki link
                        foreach (var link in guestLinks)
                            link.KorpaId = userKorpa.Id;

                        // ažuriraj cijenu userKorpe
                        var stavke = await _context.StavkeKorpe
                            .Where(s => guestLinks.Select(l => l.StavkaKorpeId).Contains(s.Id))
                            .ToListAsync();
                        userKorpa.UkupnaCijena += stavke.Sum(s => s.Kolicina * s.CijenaPoKomadu);

                        // izbriši guestKorpu
                        _context.Korpe.Remove(guestKorpa);
                        _context.Korpe.Update(userKorpa);
                        await _context.SaveChangesAsync();
                        HttpContext.Session.Remove(GuestSessionKey);
                        return userKorpa;
                    }
                }

                // 4) Ako ni jedna od njih nije postojala ili je prazna, osiguraj da imamo userKorpu
                if (userKorpa == null)
                {
                    userKorpa = new Korpa
                    {
                        KorisnikID = userId,
                        IsAktivna = true,
                        UkupnaCijena = 0
                    };
                    _context.Korpe.Add(userKorpa);
                    await _context.SaveChangesAsync();
                }
                return userKorpa;
            }
            else
            {
                // GOST – standardni flow po session GUID
                var guestKey = GetOrCreateGuestId();
                var guestKorpa = await _context.Korpe
                    .FirstOrDefaultAsync(k => k.KorisnikID == guestKey && k.IsAktivna);

                if (guestKorpa != null)
                    return guestKorpa;

                var newGuestKorpa = new Korpa
                {
                    KorisnikID = guestKey,
                    IsAktivna = true,
                    UkupnaCijena = 0
                };
                _context.Korpe.Add(newGuestKorpa);
                await _context.SaveChangesAsync();
                return newGuestKorpa;
            }
        }


        // GET: Korpa
        public async Task<IActionResult> Index()
        {
            var korpa = await GetOrCreateKorpaAsync();
            if (korpa == null)
                return RedirectToAction("Login", "Account");

            // Include popust u query
            korpa = await _context.Korpe
                .Include(k => k.Popust)
                .FirstOrDefaultAsync(k => k.Id == korpa.Id);

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

            // Izračunajte finalne cijene
            double ukupnaCijena = korpa.UkupnaCijena;
            double? iznosPopusta = null;
            double finalnaUkupnaCijena = ukupnaCijena;

            if (korpa.Popust != null)
            {
                iznosPopusta = ukupnaCijena * (korpa.Popust.VrijednostPopusta / 100);
                finalnaUkupnaCijena = ukupnaCijena - iznosPopusta.Value;
            }

            var vm = new KorpaViewModel
            {
                KorpaId = korpa.Id,
                UkupnaCijena = korpa.UkupnaCijena,
                IsAktivna = korpa.IsAktivna,
                Stavke = stavkeDto,
                KodPopusta = korpa.Popust?.KodPopust,
                VrijednostPopusta = korpa.Popust?.VrijednostPopusta,
                IznosPopusta = iznosPopusta,
                FinalnaUkupnaCijena = finalnaUkupnaCijena
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IzbaciElement(int stavkaKorpeId)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            // Find the cart item to remove
            var stavkaKorpe = await _context.StavkeKorpe.FindAsync(stavkaKorpeId);
            if (stavkaKorpe == null)
                return NotFound("Stavka korpe nije pronađena.");

            // Find the link between cart and cart item
            var link = await _context.KorpaStavkeKorpe
                .FirstOrDefaultAsync(x => x.StavkaKorpeId == stavkaKorpeId);
    
            if (link == null)
                return NotFound("Veza između korpe i stavke nije pronađena.");

            // Get the cart to update total price
            var korpa = await _context.Korpe.FindAsync(link.KorpaId);
            if (korpa == null)
                return NotFound("Korpa nije pronađena.");

            // Check if this cart belongs to the current user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (korpa.KorisnikID != userId)
                return Unauthorized();

            // Calculate the amount to subtract from total
            double amountToSubtract = stavkaKorpe.Kolicina * stavkaKorpe.CijenaPoKomadu;

            // Remove the link and the cart item
            _context.KorpaStavkeKorpe.Remove(link);
            _context.StavkeKorpe.Remove(stavkaKorpe);

            // Update cart total price
            korpa.UkupnaCijena -= amountToSubtract;
            if (korpa.UkupnaCijena < 0) korpa.UkupnaCijena = 0; // Ensure it doesn't go negative

            _context.Update(korpa);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

            [Authorize]
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Naruci()
            {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var korpa = await GetOrCreateKorpaAsync();
                if (korpa == null || !korpa.IsAktivna)
                    return NotFound("Korpa nije pronađena ili nije aktivna.");

                korpa = await _context.Korpe
                .Include(k => k.Popust)
                .FirstOrDefaultAsync(k => k.Id == korpa.Id);

            // Check if cart has items
            var links = await _context.KorpaStavkeKorpe
                    .Where(x => x.KorpaId == korpa.Id)
                    .ToListAsync();

                if (!links.Any())
                    return BadRequest("Korpa je prazna.");

                // Redirect to address form instead of creating order immediately
                double finalnaCijena = korpa.UkupnaCijena;
                if (korpa.Popust != null)
                {
                    double iznosPopusta = korpa.UkupnaCijena * (korpa.Popust.VrijednostPopusta / 100);
                    finalnaCijena -= iznosPopusta;
                }

                var addressViewModel = new AdresaViewModel
                {
                    KorpaId = korpa.Id,
                    UkupnaCijena = finalnaCijena // <- već postojeći property koristiš za finalnu cijenu
                };


                return View("Adresa", addressViewModel);
            }

   
    [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PrimijeniPopust(string kodPopusta)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(kodPopusta))
            {
                TempData["ErrorMessage"] = "Molimo unesite kod popusta.";
                return RedirectToAction(nameof(Index));
            }

            var korpa = await GetOrCreateKorpaAsync();
            if (korpa == null)
            {
                TempData["ErrorMessage"] = "Korpa nije pronađena.";
                return RedirectToAction(nameof(Index));
            }

            // Pronađi popust po kodu
            var popust = await _context.Popusti
                .FirstOrDefaultAsync(p => p.KodPopust == kodPopusta.ToUpper());

            if (popust == null)
            {
                TempData["ErrorMessage"] = "Kod popusta nije valjan.";
                return RedirectToAction(nameof(Index));
            }

            // Provjeri da li je popust već primijenjen
            if (korpa.PopustId == popust.Id)
            {
                TempData["ErrorMessage"] = "Ovaj popust je već primijenjen.";
                return RedirectToAction(nameof(Index));
            }

            // Primijeni popust
            korpa.PopustId = popust.Id;
            _context.Update(korpa);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Popust od {popust.VrijednostPopusta}% je uspješno primijenjen!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UkloniPopust()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var korpa = await GetOrCreateKorpaAsync();
            if (korpa == null)
            {
                TempData["ErrorMessage"] = "Korpa nije pronađena.";
                return RedirectToAction(nameof(Index));
            }

            korpa.PopustId = null;
            _context.Update(korpa);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Popust je uklonjen.";
            return RedirectToAction(nameof(Index));
        }

        // Modificirajte KreirajNarudzbu metodu da koristi finalnu cijenu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KreirajNarudzbu(AdresaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Adresa", model);
            }

            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var korpa = await _context.Korpe
                .Include(k => k.Popust)
                .FirstOrDefaultAsync(k => k.Id == model.KorpaId);

            if (korpa == null || !korpa.IsAktivna)
                return NotFound("Korpa nije pronađena ili nije aktivna.");

            // Izračunaj finalnu cijenu sa popustom
            double finalnaCijena = korpa.UkupnaCijena;
            if (korpa.Popust != null)
            {
                double iznosPopusta = korpa.UkupnaCijena * (korpa.Popust.VrijednostPopusta / 100);
                finalnaCijena = korpa.UkupnaCijena - iznosPopusta;
            }

            // Create address first
            var adresa = new Adresa
            {
                Ulica = model.Ulica,
                Grad = model.Grad,
                Drzava = model.Drzava
            };

            _context.Adrese.Add(adresa);
            await _context.SaveChangesAsync();

            // Create order with discounted price
            var narudzba = new Narudzba
            {
                KorisnikId = korpa.KorisnikID,
                UkupnaCijena = finalnaCijena, // Koristi finalnu cijenu
                DatumKreiranja = DateTime.Now,
                NacinPlacanja = NacinPlacanja.KARTICNO,
                AdresaId = adresa.Id,
                KorpaId = korpa.Id
            };

            _context.Narudzbe.Add(narudzba);
            await _context.SaveChangesAsync();

            // Store order ID in TempData to pass to payment
            TempData["NarudzbaId"] = narudzba.Id;
            TempData["Amount"] = narudzba.UkupnaCijena.ToString();

            var links = await _context.KorpaStavkeKorpe
                .Where(link => link.KorpaId == korpa.Id)
                .ToListAsync();
            


            foreach (var link in links)
            {
                var stavka = await _context.StavkeKorpe
                    .FirstOrDefaultAsync(s => s.Id == link.StavkaKorpeId);


                var narudzbaStavka = new NarudzbaStavka
                {
                    NarudzbaId = narudzba.Id,
                    GrupaId = stavka.GrupaId,
                    Velicina = stavka.Velicina,
                    Kolicina = stavka.Kolicina,
                    CijenaPoKomadu = (decimal)stavka.CijenaPoKomadu
                };

                _context.NarudzbaStavka.Add(narudzbaStavka);
                await _context.SaveChangesAsync();
            }

           
            _context.Update(korpa);
            await _context.SaveChangesAsync();

            // Redirect to Stripe payment
            return RedirectToAction("StripePayment", "Placanje");
        }



    }

}
