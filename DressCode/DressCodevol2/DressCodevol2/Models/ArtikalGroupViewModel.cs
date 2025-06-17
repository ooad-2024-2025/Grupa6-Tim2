namespace DressCode.Models
{
    public class ArtikalGroupViewModel
    {
        public string GrupaId { get; set; }
        public string Opis {  get; set; }
        public List<ArtikalQrViewModel> Etikete { get; set; } = new List<ArtikalQrViewModel>();
    }
}
