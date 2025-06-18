using DressCode.Attributes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;

namespace DressCode.Models
{
    public class RadnikEditViewModel : IValidatableObject
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Polje za ime je obavezno.")]
        [MinLength(3, ErrorMessage = "Ime mora imati najmanje 3 znaka.")]
        [RegularExpression(@"^[a-zA-ZšđčćžŠĐČĆŽ\s]+$", ErrorMessage = "Ime može sadržavati samo slova.")]
        [Display(Name = "Ime")]
        public string Ime { get; set; }

        [Required(ErrorMessage = "Polje za prezime je obavezno.")]
        [MinLength(3, ErrorMessage = "Prezime mora imati najmanje 3 znaka.")]
        [RegularExpression(@"^[a-zA-ZšđčćžŠĐČĆŽ\s]+$", ErrorMessage = "Prezime može sadržavati samo slova.")]
        [Display(Name = "Prezime")]
        public string Prezime { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Datum rođenja")]
        [CustomValidation(typeof(DateValidation), "ValidateDateNotInFuture")]
        public DateTime? DatumRodjenja { get; set; }

        [ValidJmbg]
        [Required(ErrorMessage = "JMBG je obavezan")]
        [RegularExpression(@"^\d{13}$", ErrorMessage = "JMBG mora imati tačno 13 brojeva.")]
        public string? JMBG { get; set; }   
        public bool? IsLoyal {  get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Broj mora biti veći od 0.")]
        public int? KarticaId { get; set; }
        public IFormFile? Slika { get; set; }
        public string? PostojeciSlikaUrl { get; set; }
        public bool IsLoyalDisplay => IsLoyal ?? false;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) 
        { 
            if(string.IsNullOrEmpty(JMBG) || !DatumRodjenja.HasValue)
                yield break;

            int dan = int.Parse(JMBG.Substring(0, 2));
            int mjesec = int.Parse(JMBG.Substring(2, 2));
            int g3 = int.Parse(JMBG.Substring(4, 3));
            int godina = (g3 <= 25 ? 2000 : 1000) + g3;

            var jmbgDatum = new DateTime(godina, mjesec, dan);
            if(jmbgDatum != DatumRodjenja.Value.Date)
            {
                yield return new ValidationResult(
                    $"Datum rođenja ({DatumRodjenja:dd.MM.yyyy}) ne odgovara datumu u JMBG ({jmbgDatum:dd.MM.yyyy}).",
                    new[] { nameof(JMBG), nameof(DatumRodjenja) }
                    );
            }
        }
    }

    public static class DateValidation
    {
        public static ValidationResult ValidateDateNotInFuture(DateTime datum, ValidationContext context)
        {
            if (datum > DateTime.Today)
            {
                return new ValidationResult("Datum rođenja ne može biti u budućnosti.");
            }
            return ValidationResult.Success;
        }
    }
}
