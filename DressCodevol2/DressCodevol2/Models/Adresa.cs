using Microsoft.AspNetCore.Routing.Constraints;

namespace DressCode.Models
{
    public class Adresa
    {
        public int Id { get; set; }
        public string Ulica { get; set; }
        public int Broj { get; set; }
        public string Grad {  get; set; }
        public string Drzava { get; set; }

    }
}
