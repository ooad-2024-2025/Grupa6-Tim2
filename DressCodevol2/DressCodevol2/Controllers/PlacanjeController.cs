using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DressCode.Data;
using DressCode.Models;
using Stripe;

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
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> ProcessPayment( string stripeToken, decimal amount)
        {
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

            var options = new ChargeCreateOptions
            {
                Amount = (long)(amount * 100), 
                Currency = "bam",
                Description = "DressCode Payment",
                Source = stripeToken
            };

            var service = new ChargeService();
            try
            {
                var charge = await service.CreateAsync(options);
              
                // Save payment to database
                var placanje = new Placanje
                {
                    Cijena =(double) amount
                };
                _context.Add(placanje);
                await _context.SaveChangesAsync();
                
                placanje.NarudzbaId = placanje.Id;
                _context.Update(placanje);
                await _context.SaveChangesAsync();
                
                ViewBag.Amount = amount;
                ViewBag.SuccessMessage = "Plaćanje je uspešno izvršeno!";
                ViewBag.NarudzbaId = placanje.Id; // Use the auto-generated Id
                return View("Success", charge);
            }
            catch (StripeException ex)
            {
                string customErrorMessage = GetCustomErrorMessage(ex.StripeError?.Code);
                ViewBag.ErrorMessage = customErrorMessage;
                ViewBag.OriginalError = ex.Message;
                return View("Error", ex);
            }
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
