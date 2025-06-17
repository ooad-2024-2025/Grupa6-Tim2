namespace DressCode.Models
{
    public class ArtikalQrViewModel
    {
        public int Id { get; set; } 
        public int ArtikalId { get; set; }  
        public string Opis { get; set; }  
        public decimal Cijena { get; set; }
        public string Velicina { get; set; }
        public string QrImageData { get; set; }
        public string GrupaId { get; set; }
    }
}
