using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DressCode.Data;
using DressCode.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace DressCode.Controllers
{
    public class QRKodController : Controller 
    {
        private readonly ApplicationDbContext _context;
        private readonly IQRCodeService _qrService;
        private static readonly string[] SizeOrder = { "XS", "S", "M", "L", "XL", "XXL", "XXXL" };
        private readonly ILogger<QRKodController> _logger;

        public QRKodController(ApplicationDbContext context, IQRCodeService qrService, ILogger<QRKodController> logger)
        {
            _context = context;
            _qrService = qrService;
            _logger = logger;
        }

        // GET: QRKod
        // Controllers/QRKodController.cs

        public async Task<IActionResult> Index(string show = "articles", string filter = null)
        {
            var vm = new QRKodIndexViewModel
            {
                Show = (show ?? "promotions").ToLower()
            };

            var isAdmin = User.IsInRole("Administrator");
            var isRadnik = User.IsInRole("Radnik");

            if (vm.Show == "promotions" && !isAdmin)
            {
                return Forbid();
            }

            if (vm.Show == "articles" && !isAdmin && !isRadnik)
            {
                return Forbid();
            }

            if (vm.Show == "articles")
            {
                // 1) Dohvati sve artikle
                var artikli = await _context.Artikli.ToListAsync();
                var etikete = new List<ArtikalQrViewModel>();

                // 2) Generiraj ili dohvatite postojeći OPISARTIKLA QR
                foreach (var a in artikli)
                {
                    var qr = await _context.QRKodovi
                        .FirstOrDefaultAsync(q => q.ArtikalId == a.Id
                                               && q.TipKoda == QRKodTip.OPISARTIKLA);

                    if (qr == null)
                    {
                        var url = Url.Action("Details", "Artikals", new { id = a.Id }, Request.Scheme);
                        qr = new QRKod
                        {
                            ArtikalId = a.Id,
                            TipKoda = QRKodTip.OPISARTIKLA,
                            DatumKreiranja = DateTime.UtcNow,
                            DatumIsteka = DateTime.UtcNow.AddYears(1),
                            IsAktivan = true,
                            DataPayload = _qrService.GenerateQrCodeBase64(url),
                            PromocijaId = null
                        };
                        _context.QRKodovi.Add(qr);
                    }

                    etikete.Add(new ArtikalQrViewModel
                    {
                        Id = qr.Id,
                        ArtikalId = a.Id,
                        GrupaId = a.GrupaId,
                        Opis = a.Opis,
                        Cijena = (decimal)a.Cijena,
                        Velicina = a.Velicina.ToString(),
                        QrImageData = qr.DataPayload
                    });

                    await _context.SaveChangesAsync();

                    vm.Grupe = etikete
                        .GroupBy(e => e.GrupaId)
                        .Select(g => new ArtikalGroupViewModel
                        {
                            GrupaId = g.Key,
                            Opis = g.First().Opis,
                            Etikete = g
                                .OrderBy(e => Array.IndexOf(SizeOrder, e.Velicina))
                                .ToList()
                        })
           .            ToList();
                }

                if (!string.IsNullOrWhiteSpace(filter))
                {
                    vm.Grupe = vm.Grupe
                        .Where(g => g.GrupaId.Contains(filter, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
            }
            else 
            {

                var popusti = await _context.Popusti.ToListAsync();
                var bonovi = new List<PopustQrViewModel>();

                foreach (var popust in popusti)
                {
                    var qr = await _context.QRKodovi
                       .FirstOrDefaultAsync(q => q.PromocijaId == popust.Id
                                              && q.TipKoda == QRKodTip.POPUST);


                    if (qr == null)
                    {
                        var url = Url.Action("Access", "Popust", new { id = popust.Id }, Request.Scheme);
                        qr = new QRKod
                        {
                            ArtikalId = null,
                            TipKoda = QRKodTip.POPUST,
                            DatumKreiranja = DateTime.UtcNow,
                            DatumIsteka = DateTime.UtcNow.AddMonths(6),
                            IsAktivan = true,
                            DataPayload = _qrService.GenerateQrCodeBase64(url),
                            PromocijaId = popust.Id
                        };
                        _context.QRKodovi.Add(qr);
                        await _context.SaveChangesAsync();
                    }

                    bonovi.Add(new PopustQrViewModel
                    {
                        Id = qr.Id,
                        PopustId = popust.Id,
                        VrijednostPopusta = popust.VrijednostPopusta,
                        DatumIsteka = qr.DatumIsteka,
                        IsAktivan = qr.IsAktivan,
                        KodPopust = popust.KodPopust,
                        PristupniKod = popust.PristupniKod,
                        QrImageData = qr.DataPayload
                    });

                }

                if (!string.IsNullOrWhiteSpace(filter))
                {
                    bonovi = bonovi
                        .Where (b => b.KodPopust.Contains(filter, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                vm.Promocije = bonovi;
            }
            ViewData["Filter"] = filter;
            return View(vm);
        }



        // GET: QRKod/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            Debug.WriteLine("Details called with id=" + id);

            if (id == null)
            {
                Debug.WriteLine("Details: id was null");
                return NotFound();
            }

            var qrKod = await _context.QRKodovi
                .FirstOrDefaultAsync(m => m.Id == id);

            if (qrKod == null)
            {
                Debug.WriteLine("Details: QRKod with id=" + id + " not found");
                return NotFound();
            }

            Debug.WriteLine("Details: found QRKod Id=" + id + ", Tip=" + qrKod.TipKoda);

            var vm = new QRKodDetailsViewModel { QRKod = qrKod };

            if (qrKod.TipKoda == QRKodTip.OPISARTIKLA)
            {
                if (qrKod.ArtikalId.HasValue)
                    vm.Artikal = await _context.Artikli.FindAsync(qrKod.ArtikalId.Value); 
            } else if (qrKod.TipKoda == QRKodTip.POPUST)
            {
                if (qrKod.PromocijaId.HasValue)
                    vm.Popust = await _context.Popusti.FindAsync(qrKod.PromocijaId.Value);
            }

            return View(vm);
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
       /* public async Task<IActionResult> Preview(int id)
        {
            var artikal = await _context.Artikli.FindAsync(id);
            if (artikal == null) return NotFound();

            var url = Url.Action(
                    action: "Details",
                    controller: "Artikals",
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
        }*/
    }
}
