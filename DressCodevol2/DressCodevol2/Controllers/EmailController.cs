/*using DressCode.Models;
using Microsoft.AspNetCore.Mvc;

namespace DressCode.Controllers
{
    public class EmailController : Controller  
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }*/

// GET: Prikaži formu za slanje
/* public IActionResult Send()
 {
     return View();
 }*/



// POST: Pošalji email
/* [HttpPost]
 public async Task<IActionResult> Send(string email, string subject, string message, bool? confirm = false)
 {
     if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(message))
     {
         ViewBag.Error = "Sva polja su obavezna!";
         return View();
     }

     try
     {
         if (confirm == true)
         {
             await _emailService.SendEmailToLoyalUsersAsync(subject, message);
             ViewBag.Success = "Email je poslan svim Loyalty korisnicima!";
         }
         else
         {
             if (string.IsNullOrWhiteSpace(email))
             {
                 ViewBag.Error = "Email adresa je obavezna ako ne šalješ svima!";
                 return View();
             }

             await _emailService.SendEmailAsync(email, subject, message);
             ViewBag.Success = "Email je uspješno poslan!";
         }
     }
     catch (Exception ex)
     {
         ViewBag.Error = "Greška pri slanju: " + ex.Message;
     }

     return View();
 }
}
}
*/
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
            if (TempData["Success"] != null)
                ViewBag.Success = TempData["Success"];
            return View(new SendEmailViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(SendEmailViewModel vm)
        {
            // **manualna** validacija Email polja:
            if (!vm.SendToAll && string.IsNullOrWhiteSpace(vm.Email))
                ModelState.AddModelError(nameof(vm.Email),
                    "Email adresa je obavezna ako ne šaljete svima!");

            // ostale (Subject/Message) validiraju se preko [Required]
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



