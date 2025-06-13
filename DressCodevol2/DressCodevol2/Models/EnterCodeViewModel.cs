using System.ComponentModel.DataAnnotations;

namespace DressCode.Models
{
    public class EnterCodeViewModel
    {
        public int PopustId { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Pristupni kod je 6 znakova.")]
        [Display(Name = "Pristupni kod")]
        public string PristupniKod { get; set; } = "";
    }
}
