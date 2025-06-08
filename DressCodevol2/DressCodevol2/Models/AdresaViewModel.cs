
using System.ComponentModel.DataAnnotations;

namespace DressCode.Models
{
    public class AdresaViewModel
    {
        public int KorpaId { get; set; }
        public double UkupnaCijena { get; set; }
        
        [Required(ErrorMessage = "Ulica je obavezna")]
        [Display(Name = "Ulica i broj")]
        public string Ulica { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Grad je obavezan")]
        [Display(Name = "Grad")]
        public string Grad { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Poštanski broj je obavezan")]
        [Display(Name = "Poštanski broj")]
        public string PostanskiBroj { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Država je obavezna")]
        [Display(Name = "Država")]
        public string Drzava { get; set; } = "Bosna i Hercegovina";
        
        [Display(Name = "Dodatne napomene")]
        public string? Napomene { get; set; }
    }
}