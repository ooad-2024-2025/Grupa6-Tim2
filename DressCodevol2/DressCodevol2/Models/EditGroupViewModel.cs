using System.ComponentModel.DataAnnotations;

namespace DressCode.Models
{
    public class EditGroupViewModel
    {
        public string GrupaId { get; set; }
        public int KategorijaId { get; set; }

        [Range(0.01, double.MaxValue)]
        public double? ZajednickaCijena {  get; set; }

        [StringLength(100)]
        public string? ZajednickiMaterijal {  get; set; }

        [StringLength(500)]
        public string? ZajednickiOpis {  get; set; }

        public List<Artikal> Artikli {  get; set; } = new List<Artikal>();
    }
}
