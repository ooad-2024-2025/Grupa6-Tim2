﻿namespace DressCode.Models
{
    public class Kartica
    {
        public int Id { get; set; }
        public string KorisnikId { get; set; }
        public string BrojKartice { get; set; }
        public DateTime DatumIsteka { get; set; }
        public string ImeVlasnika { get; set; }

    }
}
