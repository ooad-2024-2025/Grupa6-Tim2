using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DressCode.Data;
using DressCode.Models;
using Stripe.Treasury;
using Microsoft.VisualBasic;

namespace DressCode.Controllers
{
    public class PopustController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IQRCodeService _qrService;

        public PopustController(ApplicationDbContext context, IQRCodeService qrService)
        {
            _context = context;
            _qrService = qrService;
        }

        // GET: Popust
        public async Task<IActionResult> Index()
        {
            return View(await _context.Popusti.ToListAsync());
        }

        // GET: Popust/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var popust = await _context.Popusti
                .FirstOrDefaultAsync(m => m.Id == id);
            if (popust == null)
            {
                return NotFound();
            }

            return View(popust);
        }

        // GET: Popust/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Popust/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VrijednostPopusta,KodPopust")] Popust popust)
        {
            if (ModelState.IsValid)
            {
                var rand = new Random();
                popust.PristupniKod = rand.Next(100000, 999999).ToString();

                _context.Add(popust);
                await _context.SaveChangesAsync();
                var qr = new QRKod
                {
                    ArtikalId = null,
                    TipKoda = QRKodTip.POPUST,
                    DatumKreiranja = DateTime.UtcNow,
                    DatumIsteka = DateTime.UtcNow.AddMonths(6),
                    IsAktivan = true,
                    PromocijaId = popust.Id,
                    DataPayload = ""
                };

                _context.QRKodovi.Add(qr);
                await _context.SaveChangesAsync();

                popust.KodId = qr.Id;
                var accessUrl = Url.Action(
                    action: "Access",
                    controller: "Popust",
                    values: new { id = popust.Id },
                    protocol: Request.Scheme
                 );

                qr.DataPayload = _qrService.GenerateQrCodeBase64(accessUrl);
                _context.QRKodovi.Update(qr);
                _context.Popusti.Update(popust);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));


            }
            return View(popust);
        }

        // GET: Popust/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var popust = await _context.Popusti.FindAsync(id);
            if (popust == null)
            {
                return NotFound();
            }
            return View(popust);
        }

        // POST: Popust/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
    [Bind("VrijednostPopusta,KodPopust")] Popust input)
        {
            // Nikad ne koristi input.Id jer on nije bindan
            // Provjera postojanja
            var popust = await _context.Popusti.FindAsync(id);
            if (popust == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                // Vrati input i id u ViewData za formu
                ViewData["PopustId"] = id;
                return View(input);
            }

            // Ažuriraj samo ono što smije
            popust.VrijednostPopusta = input.VrijednostPopusta;
            popust.KodPopust = input.KodPopust;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PopustExists(id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: Popust/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var popust = await _context.Popusti
                .FirstOrDefaultAsync(m => m.Id == id);
            if (popust == null)
            {
                return NotFound();
            }

            return View(popust);
        }

        // POST: Popust/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var popust = await _context.Popusti.FindAsync(id);
            if (popust != null)
            {
                var qrKod = await _context.QRKodovi
                    .Where(q => q.PromocijaId == popust.Id)
                    .ToListAsync();

                _context.QRKodovi.RemoveRange(qrKod);
                _context.Popusti.Remove(popust);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PopustExists(int id)
        {
            return _context.Popusti.Any(e => e.Id == id);
        }


        [HttpGet]
        public async Task<IActionResult> Access(int id)
        {
            var pop = await _context.Popusti.FindAsync(id);
            if (pop == null) return NotFound();

            var vm = new EnterCodeViewModel { PopustId = id };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Access(EnterCodeViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var pop = await _context.Popusti.FindAsync(vm.PopustId);
            if (pop == null) return NotFound();

            if (pop.PristupniKod != vm.PristupniKod)
            {
                ModelState.AddModelError(nameof(vm.PristupniKod), "Neispravan pristupni kod.");
                return View(vm);
            }

            return RedirectToAction(nameof(Details), new { id = vm.PopustId });
        }


    }
}
