namespace DressCode.Models
{
    public class QRKodIndexViewModel
    {
        public string Show {  get; set; }
        public List<ArtikalGroupViewModel> Grupe {  get; set; } = new List<ArtikalGroupViewModel>();
        public List<QRKodCardViewModel> Promocije { get; set; } = new List<QRKodCardViewModel>();
    }
}
