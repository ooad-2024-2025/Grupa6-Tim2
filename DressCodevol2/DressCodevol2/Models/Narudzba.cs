﻿namespace DressCode.Models
{
    public class Narudzba
    {
        public int Id { get; set; }
        public string KorisnikId { get; set; }
        public NacinPlacanja NacinPlacanja { get; set; }
        public double UkupnaCijena { get; set; }
        public DateTime DatumKreiranja  { get; set; }
        public int AdresaId { get; set; }
        public int? KorpaId { get; set; }
        
    }
}
