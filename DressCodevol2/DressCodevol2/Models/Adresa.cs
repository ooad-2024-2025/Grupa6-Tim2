using Microsoft.AspNetCore.Routing.Constraints;
using System.ComponentModel.DataAnnotations;

namespace DressCode.Models
{
    public class Adresa
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ulica je obavezna.")]
        [RegularExpression(@"^[a-zA-Z0-9\s\-čćžšđČĆŽŠĐ]+$", ErrorMessage = "Ulica može sadržavati samo slova, brojeve i razmake.")]
        public string Ulica { get; set; }

        [Required(ErrorMessage = "Broj je obavezan.")]
        [Range(1, int.MaxValue, ErrorMessage = "Broj mora biti veći od 0.")]
        public int Broj { get; set; }

        [Required(ErrorMessage = "Grad je obavezan.")]
        [RegularExpression(@"^[a-zA-Z\s\-čćžšđČĆŽŠĐ]+$", ErrorMessage = "Grad može sadržavati samo slova i razmake.")]
        public string Grad {  get; set; }

        [Required(ErrorMessage = "Država je obavezna.")]
        public string Drzava { get; set; }

    }
}
