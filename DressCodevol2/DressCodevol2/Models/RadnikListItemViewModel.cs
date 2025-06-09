namespace DressCode.Models
{
    public class RadnikListItemViewModel
    {
        public string Id { get; set; }
        public string ImePrezime => $"{Ime} {Prezime}";
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string? SlikaUrl { get; set; }
    }
}
