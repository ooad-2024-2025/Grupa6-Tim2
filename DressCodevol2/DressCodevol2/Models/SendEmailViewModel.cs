using System.ComponentModel.DataAnnotations;

namespace DressCode.Models
{
    public class SendEmailViewModel : IValidatableObject
    {
        public string? Email { get; set; }

        [Required(ErrorMessage = "Predmet je obavezno polje.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Poruka je obavezno polje.")]
        public string Message { get; set; }

        public bool SendToAll { get; set; }

        // Custom validacija
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!SendToAll && string.IsNullOrWhiteSpace(Email))
            {
                yield return new ValidationResult("Email adresa je obavezna ako ne šaljete svima.", new[] { nameof(Email) });
            }
        }
    }
}
