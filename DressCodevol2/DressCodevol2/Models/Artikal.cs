using System.ComponentModel.DataAnnotations;

namespace DressCode.Models
{
    public class Artikal
    {
        public int Id { get; set; }
        public int KategorijaId { get; set; } 
        public TipOdjece? Kategorija { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Cijena mora biti veća od 0")]
        public double Cijena { get; set; }
        [StringLength(100, ErrorMessage = "Materijal ne može biti duži od 100 znakova")]
        public string Materijal {  get; set; }
        public Velicina Velicina { get; set; }

        public Spol Spol { get; set; }
        [StringLength(500, ErrorMessage = "Opis ne može biti duži od 500 znakova")]
        public string Opis { get; set; }
        public int Kolicina { get; set; }

        public string? SlikaUrl { get; set; }

        [StringLength(20, ErrorMessage = "Grupa ID ne može biti duža od 20 znakova")]
        [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Grupa ID može sadržavati samo velika slova i brojeve")]
        public string GrupaId { get; set; }

    }
}   
