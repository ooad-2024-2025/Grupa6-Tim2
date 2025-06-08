namespace DressCode.Models
{
    public class KorpaViewModel
    {
        public int KorpaId { get; set; }    
        public double UkupnaCijena { get; set; }    
        public bool IsAktivna { get; set; }
        public List<KorpaStavkaDto> Stavke { get; set; } = new();
    }
}

public class KorpaStavkaDto
{
    public int StavkaKorpeId { get; set; }  
    public string ArtikalNaziv {  get; set; }   
    public int Kolicina {  get; set; }  
    public double CijenaPoKomadu    { get; set; }
    public double Ukupno => Kolicina * CijenaPoKomadu;
}
