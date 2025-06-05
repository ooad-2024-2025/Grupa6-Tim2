using Microsoft.AspNetCore.Identity;

namespace DressCode.Models
{
    public class Korisnik : IdentityUser
    {
        public String Id { get; set; }
        public String Ime { get; set; }
        public String Prezime { get; set; }
        public String Username { get; set; }
        public DateTime DatumRodjenja { get; set; }
        public String JMBG { get; set; }
        public Boolean IsLoyal {  get; set; }   
        public int KarticaId { get; set; }  
    }
}
