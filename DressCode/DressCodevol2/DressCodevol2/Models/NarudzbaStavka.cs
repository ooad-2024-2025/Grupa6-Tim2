namespace DressCode.Models
{
    public class NarudzbaStavka
    {
        public int Id { get; set; }
        public int NarudzbaId { get; set; }
        public string GrupaId { get; set; }
        public Velicina Velicina { get; set; }
        public int Kolicina { get; set; }
        public decimal CijenaPoKomadu { get; set; }
    }
}
