namespace DressCode.Models
{
    public class StavkaKorpe
    {
        public int Id { get; set; }
        public int ArtikalId { get; set; }
        public Velicina Velicina { get; set; }
        public int Kolicina {  get; set; }
        public double CijenaPoKomadu { get; set; }
        public string? GrupaId { get; set; }
    }
}
