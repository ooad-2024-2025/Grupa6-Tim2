namespace DressCode.Models
{
    public class QRKodDetailsViewModel
    {
        public QRKod QRKod { get; set; }
        public Artikal? Artikal { get; set; } = null;
        public Popust? Popust { get; set; } = null;
    }
}
