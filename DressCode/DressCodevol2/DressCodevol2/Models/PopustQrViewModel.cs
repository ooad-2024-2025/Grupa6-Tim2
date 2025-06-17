namespace DressCode.Models
{
    public class PopustQrViewModel
    {
        public int Id { get; set; }
        public int PopustId { get; set; }
        public double VrijednostPopusta { get; set; }
        public string KodPopust {  get; set; }
        public string PristupniKod { get; set; }
        public DateTime DatumIsteka { get; set; }
        public bool IsAktivan { get; set; }
        public string QrImageData { get; set; }
    }
}
