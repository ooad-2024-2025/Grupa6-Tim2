
using System.ComponentModel.DataAnnotations;

namespace DressCode.Models
{
    public class AdresaViewModel
    {
        public int KorpaId { get; set; }
        public double UkupnaCijena { get; set; }
        
        [Required(ErrorMessage = "Ulica je obavezna.")]
        [RegularExpression(@"^[a-zA-Z0-9\s\-čćžšđČĆŽŠĐ]+$", ErrorMessage = "Ulica može sadržavati samo slova, brojeve i razmake.")]
        [Display(Name = "Ulica i broj")]
        public string Ulica { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Grad je obavezan.")]
        [Display(Name = "Grad")]
        [RegularExpression(@"^[a-zA-Z\s\-čćžšđČĆŽŠĐ]+$", ErrorMessage = "Grad može sadržavati samo slova i razmake.")]
        public string Grad { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Poštanski broj je obavezan.")]
        [Display(Name = "Poštanski broj")]
        [Range(1, int.MaxValue, ErrorMessage = "Broj mora biti veći od 0.")]
        public string PostanskiBroj { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Država je obavezna.")]
        [Display(Name = "Država")]
        public string Drzava { get; set; } = "Bosna i Hercegovina";
        
        [Display(Name = "Dodatne napomene")]
        public string? Napomene { get; set; }
    }
}