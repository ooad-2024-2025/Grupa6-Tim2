using System.ComponentModel.DataAnnotations;

namespace DressCode.Models
{
    public class RadnikEditViewModel
    {
        public string? Id { get; set; }
        [Required] public string Ime { get; set; }
        [Required] public string Prezime { get; set; }
        public DateTime? DatumRodjenja { get; set; }    
        public string? JMBG { get; set; }   
        public bool? IsLoyal {  get; set; }  
        public int? KarticaId { get; set; }
        public IFormFile? Slika { get; set; }
        public string? PostojeciSlikaUrl { get; set; }
        public bool IsLoyalDisplay => IsLoyal ?? false;
    }
}