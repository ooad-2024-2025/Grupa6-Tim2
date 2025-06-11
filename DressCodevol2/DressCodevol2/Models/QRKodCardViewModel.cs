namespace DressCode.Models
{
    public class QRKodCardViewModel
    {
        public int Id { get; set; }
        public int ArtikalId { get; set; }
        public string ArtikalNaziv { get; set; }
        public DateTime DatumIsteka { get; set; }
        public bool IsAktivan { get; set; }
        public string QrImageData { get; set; }
    }
}
