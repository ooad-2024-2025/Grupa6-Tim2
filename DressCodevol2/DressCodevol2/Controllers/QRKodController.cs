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
    public class QRKodController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IQRCodeService _qrService;

        public QRKodController(ApplicationDbContext context, IQRCodeService qrService)
        {
            _context = context;
            _qrService = qrService;
        }

        // GET: QRKod
        public async Task<IActionResult> Index()
        {
            return View(await _context.QRKodovi.ToListAsync());
        }

        // GET: QRKod/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var qRKod = await _context.QRKodovi
                .FirstOrDefaultAsync(m => m.Id == id);
            if (qRKod == null)
            {
                return NotFound();
            }

            return View(qRKod);
        }

        // GET: QRKod/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: QRKod/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ArtikalId,TipKoda,DatumKreiranja,DatumIsteka,IsAktivan,DataPayload")] QRKod qRKod)
        {
            if (ModelState.IsValid)
            {
                _context.Add(qRKod);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(qRKod);
        }

        // GET: QRKod/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var qRKod = await _context.QRKodovi.FindAsync(id);
            if (qRKod == null)
            {
                return NotFound();
            }
            return View(qRKod);
        }

        // POST: QRKod/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ArtikalId,TipKoda,DatumKreiranja,DatumIsteka,IsAktivan,DataPayload")] QRKod qRKod)
        {
            if (id != qRKod.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(qRKod);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QRKodExists(qRKod.Id))
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
            return View(qRKod);
        }

        // GET: QRKod/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var qRKod = await _context.QRKodovi
                .FirstOrDefaultAsync(m => m.Id == id);
            if (qRKod == null)
            {
                return NotFound();
            }

            return View(qRKod);
        }

        // POST: QRKod/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var qRKod = await _context.QRKodovi.FindAsync(id);
            if (qRKod != null)
            {
                _context.QRKodovi.Remove(qRKod);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QRKodExists(int id)
        {
            return _context.QRKodovi.Any(e => e.Id == id);
        }

        // GET: QRKod/Preview/5
        public async Task<IActionResult> Preview(int id)
        {
            var artikal = await _context.Artikli.FindAsync(id);
            if (artikal == null) return NotFound();

            var url = Url.Action(
                    action: "Details",
                    controller: "Artikal",
                    values: new { id = id },
                    protocol: Request.Scheme
                );
            string qrDataUri = _qrService.GenerateQrCodeBase64(url);
            var vm = new ArtikalQrViewModel
            {
                Artikal = artikal,
                Opis = artikal.Opis,
                QrImageData = qrDataUri,
                QrKodId = id
            };

            return View(vm);
        }
    }
}
