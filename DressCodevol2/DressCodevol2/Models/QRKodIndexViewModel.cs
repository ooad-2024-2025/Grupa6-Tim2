namespace DressCode.Models
{
    public class QRKodIndexViewModel
    {
        public string Show {  get; set; }
        public List<ArtikalQrViewModel> Artikli {  get; set; } = new List<ArtikalQrViewModel>();
        public List<QRKodCardViewModel> Promocije { get; set; } = new List<QRKodCardViewModel>();
    }
}
