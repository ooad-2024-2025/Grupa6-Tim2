using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace DressCode.Attributes
{
    public class ValidJmbgAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var jmbg = value as string;
            if (string.IsNullOrEmpty(jmbg) || !IsValidJmbg(jmbg))
                return new ValidationResult("Neispravan JMBG.");

            return ValidationResult.Success;
        }

        private bool IsValidJmbg(string jmbg)
        {
            if (jmbg.Length != 13 || !long.TryParse(jmbg, out _))
                return false;

            // Datum
            int dan = int.Parse(jmbg.Substring(0, 2));
            int mjesec = int.Parse(jmbg.Substring(2, 2));
            int g3 = int.Parse(jmbg.Substring(4, 3));
            int godina = (g3 <= 21 ? 2000 : 1000) + g3;
            if (!DateTime.TryParseExact(
                    $"{dan:00}.{mjesec:00}.{godina:0000}",
                    "dd.MM.yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out _))
            {
                return false;
            }

            // Kontrolna cifra
            int[] m = { 7, 6, 5, 4, 3, 2 };
            int suma = 0;
            for (int i = 0; i < 6; i++)
                suma += m[i] * ((jmbg[i] - '0') + (jmbg[i + 6] - '0'));

            int k = 11 - (suma % 11);
            if (k == 11) k = 0;
            if (k == 10) return false;

            return k == (jmbg[12] - '0');
        }
    }
}
