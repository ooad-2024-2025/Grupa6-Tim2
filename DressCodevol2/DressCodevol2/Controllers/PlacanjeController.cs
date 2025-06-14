using DressCode.Data;
using DressCode.Data.Migrations;
using DressCode.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DressCode.Controllers
{
    public class PlacanjeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration; 

        public PlacanjeController(ApplicationDbContext context,  IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: Placanje
        public async Task<IActionResult> Index()
        {
            return View(await _context.Placanja.ToListAsync());
        }
        
        public IActionResult StripePayment()
        {
            ViewBag.PublishableKey = _configuration["Stripe:PublishableKey"];
    
            // Get order info from TempData
            if (TempData["NarudzbaId"] != null)
            {
                ViewBag.NarudzbaId = TempData["NarudzbaId"];
        
                // Convert string back to double
                if (TempData["Amount"] != null && double.TryParse(TempData["Amount"].ToString(), out double amount))
                {
                    ViewBag.Amount = amount;
                }
                else
                {
                    ViewBag.Amount = 0.0;
                }
        
                // Keep TempData for the next request (ProcessPayment)
                TempData.Keep("NarudzbaId");
                TempData.Keep("Amount");
            }
            else
            {
                // Default values if accessed directly
                ViewBag.Amount = 0.0;
                ViewBag.NarudzbaId = 0;
            }
    
            return View();
        }

        // POST: Payments
        // POST: Payments
        [HttpPost]
        public async Task<IActionResult> ProcessPayment(string stripeToken, decimal amount)
        {
            // Stripe charge
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
            var options = new ChargeCreateOptions
            {
                Amount = (long)(amount * 100),
                Currency = "bam",
                Description = "DressCode Payment",
                Source = stripeToken
            };
            var service = new ChargeService();
            Charge charge;
            try
            {
                charge = await service.CreateAsync(options);
            }
            catch (StripeException ex)
            {
                ModelState.AddModelError("", "Plaćanje nije uspjelo: " + ex.Message);
                return View("Error");
            }

            // Retrieve order and cart
            int narudzbaId = TempData["NarudzbaId"] as int? ?? 0;
            var narudzba = await _context.Narudzbe.FindAsync(narudzbaId);
            if (narudzba == null) return View("Error");

            var korpa = await _context.Korpe
                .FirstOrDefaultAsync(k => k.Id == narudzba.KorpaId);
            if (korpa == null) return View("Error");

            // Save payment record
            var placanje = new Placanje { NarudzbaId = narudzbaId, Cijena = (double)amount };
            _context.Placanja.Add(placanje);
            narudzba.NacinPlacanja = NacinPlacanja.KARTICNO;
            _context.Narudzbe.Update(narudzba);

            // Fetch all linking rows for this cart from KorpaStavkeKorpe
            var cartLinks = await _context.KorpaStavkeKorpe
                .Where(link => link.KorpaId == korpa.Id)
                .ToListAsync();

            foreach (var link in cartLinks)
            {
                // Fetch stavka by its ID
                var stavka = await _context.StavkeKorpe.FindAsync(link.StavkaKorpeId);
                if (stavka == null) continue;

                // Fetch artikal and decrease stock
                var artikal = await _context.Artikli.FindAsync(stavka.ArtikalId);
                if (artikal == null) continue;

                artikal.Kolicina -= stavka.Kolicina;
                if (artikal.Kolicina <= 0)
                {
                    // Remove all StavkeKorpe for this article
                    var sviStavke = await _context.StavkeKorpe
                        .Where(s => s.ArtikalId == artikal.Id)
                        .ToListAsync();
                    // Remove all links for those stavke
                    var sviLinkovi = await _context.KorpaStavkeKorpe
                        .Where(l => sviStavke.Select(s => s.Id).Contains(l.StavkaKorpeId))
                        .ToListAsync();

                    _context.KorpaStavkeKorpe.RemoveRange(sviLinkovi);
                    _context.StavkeKorpe.RemoveRange(sviStavke);
                    _context.Artikli.Remove(artikal);
                }
                else
                {
                    _context.Artikli.Update(artikal);
                    // Remove only this cart's link and stavka
                    _context.KorpaStavkeKorpe.Remove(link);
                    _context.StavkeKorpe.Remove(stavka);
                }
            }

            // Deactivate cart
            korpa.IsAktivna = false;

            // Recalculate UkupnaCijena from remaining links
            var remainingLinks = await _context.KorpaStavkeKorpe
                .Where(l => l.KorpaId == korpa.Id)
                .ToListAsync();
            double newTotal = 0;
            foreach (var link in remainingLinks)
            {
                var s = await _context.StavkeKorpe.FindAsync(link.StavkaKorpeId);
                if (s != null)
                    newTotal += s.CijenaPoKomadu * s.Kolicina;
            }
            korpa.UkupnaCijena = newTotal;
            _context.Korpe.Update(korpa);

            // Save all changes
            await _context.SaveChangesAsync();

            ViewBag.Amount = amount;
            ViewBag.SuccessMessage = "Plaćanje je uspešno izvršeno!";
            ViewBag.NarudzbaId = narudzbaId;
            return View("Success", charge);
        }



        private string GetCustomErrorMessage(string errorCode)
        {
            return errorCode switch
            {
                "card_declined" => "Kartica je odbijena. Pokušajte sa drugom karticom.",
                "insufficient_funds" => "Nemate dovoljno sredstava.",
                "expired_card" => "Kartica je istekla.",
                _ => "Došlo je do greške. Pokušajte ponovo."
            };
        }

        // GET: Placanje/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var placanje = await _context.Placanja
                .FirstOrDefaultAsync(m => m.Id == id);
            if (placanje == null)
            {
                return NotFound();
            }

            return View(placanje);
        }

        // GET: Placanje/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Placanje/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NarudzbaId,Cijena")] Placanje placanje)
        {
            if (ModelState.IsValid)
            {
                _context.Add(placanje);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(placanje);
        }

        // GET: Placanje/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var placanje = await _context.Placanja.FindAsync(id);
            if (placanje == null)
            {
                return NotFound();
            }
            return View(placanje);
        }

        // POST: Placanje/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NarudzbaId,Cijena")] Placanje placanje)
        {
            if (id != placanje.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(placanje);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlacanjeExists(placanje.Id))
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
            return View(placanje);
        }

        // GET: Placanje/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var placanje = await _context.Placanja
                .FirstOrDefaultAsync(m => m.Id == id);
            if (placanje == null)
            {
                return NotFound();
            }

            return View(placanje);
        }

        // POST: Placanje/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var placanje = await _context.Placanja.FindAsync(id);
            if (placanje != null)
            {
                _context.Placanja.Remove(placanje);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            
            
        }

        private bool PlacanjeExists(int id)
        {
            return _context.Placanja.Any(e => e.Id == id);
        }
    }
}
