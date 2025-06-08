using Microsoft.AspNetCore.Authorization;

namespace DressCode.Models
{
    public class Korpa
    {
        public int Id { get; set; }
        public string KorisnikID { get; set; }
        public double UkupnaCijena { get; set; }
        public bool IsAktivna { get; set; }
    }
}
