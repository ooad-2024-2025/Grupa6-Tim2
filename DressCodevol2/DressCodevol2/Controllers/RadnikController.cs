using Azure.Identity;
using DressCode.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace DressCode.Controllers
{
    [Authorize(Roles ="Administrator")]
    public class RadnikController : Controller
    {
        private readonly UserManager<Korisnik> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _env;

        public RadnikController(
            UserManager<Korisnik> userManager,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
        }

        // GET: /Radnik
        public async Task<IActionResult> Index()
        {
            var radnici = await _userManager.GetUsersInRoleAsync("Radnik")
                ?? new List<Korisnik>();
            var model = radnici.Select(u => new RadnikListItemViewModel
            {
                Id = u.Id,
                Ime = u.Ime,
                Prezime = u.Prezime,
                SlikaUrl = string.IsNullOrEmpty(u.SlikaUrl)
                    ? "/images/UserImageDefault.jpg"
                    : u.SlikaUrl
            }).ToList();
            return View(model);
        }

        // GET: /Radnik/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var u = await _userManager.FindByIdAsync(id);
            if (u == null) return NotFound();

            var vm = new RadnikEditViewModel
            {
                Id = u.Id,
                Ime = u.Ime,
                Prezime = u.Prezime,
                DatumRodjenja = u.DatumRodjenja,
                JMBG = u.JMBG,
                IsLoyal = u.IsLoyal,
                KarticaId = u.KarticaId,
                PostojeciSlikaUrl = string.IsNullOrEmpty(u.SlikaUrl)
                    ? "/images/UserImageDefault.jpg"
                    : u.SlikaUrl
            };
            return View(vm);   
        }

        public IActionResult Create()
            => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RadnikEditViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = new Korisnik
            {
                Email = $"{vm.Ime}_{vm.Prezime}@dresscode.com",
                UserName = $"{vm.Ime}_{vm.Prezime}@dresscode.com", 
                EmailConfirmed = true,
                Ime = vm.Ime,
                Prezime = vm.Prezime,
                DatumRodjenja = vm.DatumRodjenja,
                JMBG = vm.JMBG,
                IsLoyal = false,
                KarticaId = null
            };

            if (vm.Slika != null && vm.Slika.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "images/radnici");
                Directory.CreateDirectory(uploads);
                var filename = $"{Guid.NewGuid()}{Path.GetExtension(vm.Slika.FileName)}";
                var filepath = Path.Combine(uploads, filename);
                using var stream = new FileStream(filepath, FileMode.Create);
                await vm.Slika.CopyToAsync(stream);
                user.SlikaUrl = $"/images/radnici/{filename}";
            } else
            {
                user.SlikaUrl = "images/UserImageDefault.jpg";
            }

                var result = await _userManager.CreateAsync(user, "DressCodeRadnik123!");
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                {
                    ModelState.AddModelError("", e.Description);
                }
                return View(vm);
            }

            await _userManager.AddToRoleAsync(user, "Radnik");
            return RedirectToAction(nameof(Index));
        }

        // GET: /Radnik/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var u = await _userManager.FindByIdAsync(id);
            if (u == null) return NotFound();

            var vm = new RadnikEditViewModel
            {
                Id = u.Id,
                Ime = u.Ime,
                Prezime = u.Prezime,
                DatumRodjenja = u.DatumRodjenja,
                JMBG = u.JMBG,
                IsLoyal = u.IsLoyal,
                KarticaId = u.KarticaId,
                PostojeciSlikaUrl = string.IsNullOrEmpty(u.SlikaUrl)
                    ? "/images/UserImageDefault.jpg"
                    : u.SlikaUrl
            };
            return View(vm);
        }

        // POST: /Radnik/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RadnikEditViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _userManager.FindByIdAsync(vm.Id);
            if (user == null) return NotFound();

            user.Ime = vm.Ime;
            user.Prezime = vm.Prezime;
            user.DatumRodjenja = vm.DatumRodjenja;
            user.JMBG = vm.JMBG;
            user.IsLoyal = (bool)(vm.IsLoyal == null ? false : vm.IsLoyal);
            user.KarticaId = vm.KarticaId;
            user.Email = $"{vm.Ime}_{vm.Prezime}@dresscode.com";
            user.UserName = $"{vm.Ime}_{vm.Prezime}@dresscode.com";

            if (vm.Slika != null && vm.Slika.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "images/radnici");
                Directory.CreateDirectory(uploads);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(vm.Slika.FileName)}";
                var filepath = Path.Combine(uploads, fileName);
                using var stream = new FileStream(filepath, FileMode.Create); 
                await vm.Slika.CopyToAsync(stream);
                user.SlikaUrl = $"/images/radnici/{fileName}";
            }
            else
            {
                user.SlikaUrl = vm.PostojeciSlikaUrl;
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var e in updateResult.Errors) ModelState.AddModelError("", e.Description);
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Radnik/Delete/5

        public async Task<IActionResult> Delete(string id)
        {
            var u = await _userManager.FindByIdAsync(id);
            if (u == null) return NotFound();
            var vm = new RadnikListItemViewModel
            {
                Id = u.Id,
                Ime = u.Ime,
                Prezime = u.Prezime,
                SlikaUrl = string.IsNullOrEmpty(u.SlikaUrl)
                        ? "/images/UserImageDefault.jpg"
                        : u.SlikaUrl
            };
            return View(vm);
        }

        // POST: /Radnik/Delete/5
        [HttpPost, ActionName("DeleteConfirmed"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
                await _userManager.DeleteAsync(user);
            return View(DeleteConfirmed(id));
        }
    }
}
