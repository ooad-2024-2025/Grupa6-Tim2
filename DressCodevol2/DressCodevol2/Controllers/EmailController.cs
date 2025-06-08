using DressCode.Models;
using Microsoft.AspNetCore.Mvc;

namespace DressCode.Controllers
{
    public class EmailController : Controller  // ← Dodaj ovu liniju!
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        // GET: Prikaži formu za slanje
        public IActionResult Send()
        {
            return View();
        }

        // POST: Pošalji email
        [HttpPost]
        public async Task<IActionResult> Send(string email, string subject, string message)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(message))
            {
                ViewBag.Error = "Sva polja su obavezna!";
                return View();
            }

            try
            {
                await _emailService.SendEmailAsync(email, subject, message);
                ViewBag.Success = "Email je uspješno poslat!";
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Greška pri slanju: " + ex.Message;
            }

            return View();
        }
    }
}