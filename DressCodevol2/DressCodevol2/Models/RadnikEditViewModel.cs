using DressCode.Attributes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;

namespace DressCode.Models
{
    public class RadnikEditViewModel : IValidatableObject
    {
        public string? Id { get; set; }
        [Required] 
        public string Ime { get; set; }
        [Required] 
        public string Prezime { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Datum rođenja je obavezan")]
        public DateTime? DatumRodjenja { get; set; }

        [ValidJmbg]
        [Required(ErrorMessage = "JMBG je obavezan")]
        public string? JMBG { get; set; }   
        public bool? IsLoyal {  get; set; }  
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
}
