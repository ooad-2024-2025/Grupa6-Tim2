namespace DressCode.Models
{
    public class QRKod
    {
        public int Id { get; set; }
        public int? ArtikalId { get; set; }
        public QRKodTip TipKoda { get; set; }
        public DateTime DatumKreiranja { get; set; }
        public DateTime DatumIsteka { get; set; }
        public bool IsAktivan { get; set; }
        public string DataPayload { get; set; }
        public int? PromocijaId { get; set; }

    }
}
