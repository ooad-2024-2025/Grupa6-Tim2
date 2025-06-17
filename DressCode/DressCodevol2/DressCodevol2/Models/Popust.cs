using System.ComponentModel.DataAnnotations;

namespace DressCode.Models
{
    public class Popust
    {
        public int Id { get; set; }
        public int? KodId { get; set; }
        [Required(ErrorMessage="Polje vrijednost popusta je obavezno.")]
        [Range(0, 100, ErrorMessage ="Vrijednost popusta mora biti između 0-100%.")]
        public double VrijednostPopusta { get; set; }

        [Required(ErrorMessage ="Polje kod popusta je obavezno.")]
        [StringLength(40, ErrorMessage ="Kod popusta ne može biti duži od 30 znakova.")]
        [RegularExpression(@"^(?=.*[A-Za-zčćšđžČĆŠĐŽ])[A-Za-zčćšđžČĆŠĐŽ0-9]+$", ErrorMessage = "Kod mora sadržavati barem jedno slovo.")]
        public string KodPopust { get; set; }

        [StringLength(6)]
        public string? PristupniKod { get; set; }
    }
}
