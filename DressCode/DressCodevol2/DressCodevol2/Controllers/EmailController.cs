using DressCode.Models;
using Microsoft.AspNetCore.Mvc;
namespace DressCode.Controllers
{
    public class EmailController : Controller
    {
        private readonly IEmailService _emailService;
        public EmailController(IEmailService emailService) => _emailService = emailService;
        [HttpGet]
        public IActionResult Send()
        {
            var isAdmin = User.IsInRole("Administrator");
            if (!isAdmin)
            {
                return Forbid();
            }
            if (TempData["Success"] != null)
                ViewBag.Success = TempData["Success"];
            return View(new SendEmailViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(SendEmailViewModel vm)
        {
            var isAdmin = User.IsInRole("Administrator");
            if (!isAdmin)
            {
                return Forbid();
            }
            // Uklanjamo ModelState greške za Email ako je SendToAll true
            if (vm.SendToAll)
            {
                ModelState.Remove(nameof(vm.Email));
            }
            else if (string.IsNullOrWhiteSpace(vm.Email))
            {
                ModelState.AddModelError(nameof(vm.Email),
                    "Email adresa je obavezna ako ne šaljete svima!");
            }

            if (!ModelState.IsValid)
                return View(vm);

            if (vm.SendToAll)
                await _emailService.SendEmailToLoyalUsersAsync(vm.Subject, vm.Message);
            else
                await _emailService.SendEmailAsync(vm.Email!, vm.Subject, vm.Message);

            TempData["Success"] = vm.SendToAll
                ? "Email je poslan svim Loyalty korisnicima!"
                : "Email je uspješno poslan!";

            return RedirectToAction(nameof(Send));
        }
    }
}