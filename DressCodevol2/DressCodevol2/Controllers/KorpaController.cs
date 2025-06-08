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
       
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Naruci()
{
    if (!User.Identity.IsAuthenticated)
        return RedirectToAction("Login", "Account");

    var korpa = await GetOrCreateKorpaAsync();
    if (korpa == null || !korpa.IsAktivna)
        return NotFound("Korpa nije pronađena ili nije aktivna.");

    // Check if cart has items
    var links = await _context.KorpaStavkeKorpe
        .Where(x => x.KorpaId == korpa.Id)
        .ToListAsync();

    if (!links.Any())
        return BadRequest("Korpa je prazna.");

    // Redirect to address form instead of creating order immediately
    var addressViewModel = new AdresaViewModel
    {
        KorpaId = korpa.Id,
        UkupnaCijena = korpa.UkupnaCijena
    };

    return View("Adresa", addressViewModel);
}

// Add new method to handle address form submission
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

    var korpa = await _context.Korpe.FindAsync(model.KorpaId);
    if (korpa == null || !korpa.IsAktivna)
        return NotFound("Korpa nije pronađena ili nije aktivna.");

    // Create address first
    var adresa = new Adresa
    {
        Ulica = model.Ulica,
        Grad = model.Grad,
        Drzava = model.Drzava
    };

    _context.Adrese.Add(adresa);
    await _context.SaveChangesAsync();

    // Create order with address
    var narudzba = new Narudzba
    {
        KorisnikId = korpa.KorisnikID,
        UkupnaCijena = korpa.UkupnaCijena,
        DatumKreiranja = DateTime.Now,
        NacinPlacanja = NacinPlacanja.KARTICNO,
        Adresa = adresa
    };

    _context.Narudzbe.Add(narudzba);
    await _context.SaveChangesAsync();

    // Store order ID in TempData to pass to payment
    TempData["NarudzbaId"] = narudzba.Id;
    TempData["Amount"] = narudzba.UkupnaCijena.ToString();

    // Deactivate the cart
    korpa.IsAktivna = false;
    _context.Update(korpa);
    await _context.SaveChangesAsync();

    // Redirect to Stripe payment
    return RedirectToAction("StripePayment", "Placanje");
}

    }
    
    
}
