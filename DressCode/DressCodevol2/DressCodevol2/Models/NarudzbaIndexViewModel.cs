namespace DressCode.Models
{
    public class NarudzbaIndexViewModel
    {
        public int NarudzbaId { get; set; }
        public string ImePrezime { get; set; }
        public string DatumKreiranja { get; set; }   // već formatiran
        public string Adresa { get; set; }
        public List<string> Artikli { get; set; }
        public string Status { get; set; } = "ČEKA NA ISPORUKU";
    }
}
