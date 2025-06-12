using System.ComponentModel.DataAnnotations;

namespace DressCode.Models
{
    public class Artikal
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Polje za kategoriju je obavezno.")]
        public int KategorijaId { get; set; }

        [Required(ErrorMessage = "Polje za kategoriju je obavezno.")]
        public TipOdjece? Kategorija { get; set; }

        [Required(ErrorMessage = "Polje za cijenu je obavezno.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cijena mora biti veća od 0")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Cijena mora biti broj (sa ili bez decimala)")]
        public double Cijena { get; set; }

        [Required(ErrorMessage = "Polje za materijal je obavezno.")]
        [StringLength(100, ErrorMessage = "Materijal ne može biti duži od 100 znakova")]
        [RegularExpression(@"^[a-zA-ZčćžšđČĆŽŠĐ\s,\.]+$", ErrorMessage = "Materijal može sadržavati samo slova, zareze i tačke.")]
        public string Materijal {  get; set; }

        [Required(ErrorMessage = "Polje za veličinu je obavezno.")]
        public Velicina Velicina { get; set; }

        [Required(ErrorMessage = "Polje za spol je obavezno.")]
        public Spol Spol { get; set; }

        [Required(ErrorMessage = "Polje za opis je obavezno.")]
        [StringLength(500, ErrorMessage = "Opis ne može biti duži od 500 znakova")]
        [RegularExpression(@"^[a-zA-ZčćžšđČĆŽŠĐ\s,\.]+$", ErrorMessage = "Opis može sadržavati samo slova, zareze i tačke.")]
        public string Opis { get; set; }

        [Required(ErrorMessage = "Polje za količinu je obavezno.")]
        public int Kolicina { get; set; }

        public string? SlikaUrl { get; set; }

        [Required(ErrorMessage = "Polje za grupni ID je obavezno.")]
        [StringLength(20, ErrorMessage = "Grupa ID ne može biti duža od 20 znakova")]
        [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Grupa ID može sadržavati samo velika slova i brojeve")]
        public string GrupaId { get; set; }

    }
}   
