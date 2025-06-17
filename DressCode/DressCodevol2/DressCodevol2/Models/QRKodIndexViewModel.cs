namespace DressCode.Models
{
    public class QRKodIndexViewModel
    {
        public string Show {  get; set; }
        public List<ArtikalGroupViewModel> Grupe {  get; set; } = new List<ArtikalGroupViewModel>();
        public List<PopustQrViewModel> Promocije { get; set; } = new List<PopustQrViewModel>();
       
    }
}
