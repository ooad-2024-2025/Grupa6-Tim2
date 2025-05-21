namespace DressCode.Models
{
    public class Artikal
    {
        public int Id { get; set; }
        public TipOdjece Kategorija { get; set; }
        public double Cijena { get; set; }
        public string Materijal {  get; set; }
        public int Velicina { get; set; }
        public Spol Spol { get; set; }
        public string Opis { get; set; }

    }
}
