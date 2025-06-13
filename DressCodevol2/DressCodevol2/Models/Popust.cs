using System.ComponentModel.DataAnnotations;

namespace DressCode.Models
{
    public class Popust
    {
        public int Id { get; set; }
        public int? KodId { get; set; }
        public double VrijednostPopusta { get; set; }
        public string KodPopust { get; set; }

        [StringLength(10)]
        public string? PristupniKod { get; set; }
    }
}
