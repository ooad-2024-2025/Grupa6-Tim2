using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DressCode.Models;
using Microsoft.AspNetCore.Identity;
namespace DressCode.Data
{
    public class ApplicationDbContext : IdentityDbContext<Korisnik, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Adresa> Adrese { get; set; }
        public DbSet<Artikal> Artikli { get; set; }
        public DbSet<ArtikalNarudzba> ArtikliNarudzbe { get; set; }
        public DbSet<Kartica> Kartice { get; set; }
        public DbSet<Korpa> Korpe { get; set; }
        public DbSet<KorpaStavkaKorpe> KorpaStavkeKorpe { get; set; }
        public DbSet<Narudzba> Narudzbe { get; set; }
        public DbSet<Placanje> Placanja { get; set; }
        public DbSet<Popust> Popusti { get; set; }
        public DbSet<QRKod> QRKodovi { get; set; }
        public DbSet<Racun> Racuni { get; set; }
        public DbSet<StavkaKorpe> StavkeKorpe { get; set; }
        public DbSet<TipOdjece> TipoviOdjece { get; set; }
        public DbSet<Korisnik> Korisnik { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Adresa>().ToTable("Adresa");
            modelBuilder.Entity<Artikal>().ToTable("Artikal");
            modelBuilder.Entity<ArtikalNarudzba>().ToTable("ArtikalNarudzbe");
            modelBuilder.Entity<Kartica>().ToTable("Kartica");
            modelBuilder.Entity<Korpa>().ToTable("Korpa");
            modelBuilder.Entity<KorpaStavkaKorpe>().ToTable("KorpaStavkaKorpe");
            modelBuilder.Entity<Narudzba>().ToTable("Narudzba");
            modelBuilder.Entity<Placanje>().ToTable("Placanje");
            modelBuilder.Entity<Popust>().ToTable("Popust");
            modelBuilder.Entity<QRKod>().ToTable("QRKod");
            modelBuilder.Entity<Racun>().ToTable("Racun");
            modelBuilder.Entity<StavkaKorpe>().ToTable("StavkaKorpe"); 
            modelBuilder.Entity<TipOdjece>().ToTable("TipOdjece");
            modelBuilder.Entity<ArtikalNarudzba>().ToTable("ArtikalNarudzba");
            modelBuilder.Entity<Korisnik>().ToTable("Korisnik");

            
        }

    }
}
