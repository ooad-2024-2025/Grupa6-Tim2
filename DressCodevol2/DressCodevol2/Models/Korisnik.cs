using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DressCode.Models
{
    public class Korisnik : IdentityUser
    {
        [Required]
        public String Ime { get; set; }
        [Required]
        public String Prezime { get; set; }
        public DateTime? DatumRodjenja { get; set; }
        public String? JMBG { get; set; }
        public Boolean IsLoyal {  get; set; }   
        public int? KarticaId { get; set; }  
        public string? SlikaUrl { get; set; }
    }
}
