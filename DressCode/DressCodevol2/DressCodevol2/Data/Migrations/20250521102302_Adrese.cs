using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DressCode.Data.Migrations
{
    /// <inheritdoc />
    public partial class Adrese : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Adrese",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ulica = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Broj = table.Column<int>(type: "int", nullable: false),
                    Grad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Drzava = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adrese", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArtikliNarudzbe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtikliNarudzbe", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kartice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikId = table.Column<int>(type: "int", nullable: false),
                    BrojKartice = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatumIsteka = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImeVlasnika = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kartice", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KorpaStavkeKorpe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KorpaStavkeKorpe", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Korpe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikID = table.Column<int>(type: "int", nullable: false),
                    UkupnaCijena = table.Column<double>(type: "float", nullable: false),
                    IsAktivna = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Korpe", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Placanja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NarudzbaId = table.Column<int>(type: "int", nullable: false),
                    Cijena = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Placanja", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Popusti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KodId = table.Column<int>(type: "int", nullable: false),
                    VrijednostPopusta = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Popusti", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QRKodovi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArtikalId = table.Column<int>(type: "int", nullable: false),
                    TipKoda = table.Column<int>(type: "int", nullable: false),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatumIsteka = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAktivan = table.Column<bool>(type: "bit", nullable: false),
                    DataPayload = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QRKodovi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Racuni",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NarudzbaId = table.Column<int>(type: "int", nullable: false),
                    Cijena = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Racuni", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StavkeKorpe",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Velicina = table.Column<int>(type: "int", nullable: false),
                    Kolicina = table.Column<int>(type: "int", nullable: false),
                    CijenaPoKomadu = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StavkeKorpe", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoviOdjece",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Majica = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kosulja = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bluza = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dukserica = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Jakna = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kaput = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hlace = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Trenerka = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Haljina = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Suknja = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Odijelo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SportskaOprema = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlaznaOdjeca = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Carape = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModniDodaci = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoviOdjece", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Narudzbe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikId = table.Column<int>(type: "int", nullable: false),
                    ArtikalId = table.Column<int>(type: "int", nullable: false),
                    NacinPlacanja = table.Column<int>(type: "int", nullable: false),
                    UkupnaCijena = table.Column<double>(type: "float", nullable: false),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Narudzbe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Narudzbe_Adrese_AdresaId",
                        column: x => x.AdresaId,
                        principalTable: "Adrese",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Artikli",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KategorijaId = table.Column<int>(type: "int", nullable: false),
                    Cijena = table.Column<double>(type: "float", nullable: false),
                    Materijal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Velicina = table.Column<int>(type: "int", nullable: false),
                    Spol = table.Column<int>(type: "int", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artikli", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Artikli_TipoviOdjece_KategorijaId",
                        column: x => x.KategorijaId,
                        principalTable: "TipoviOdjece",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Artikli_KategorijaId",
                table: "Artikli",
                column: "KategorijaId");

            migrationBuilder.CreateIndex(
                name: "IX_Narudzbe_AdresaId",
                table: "Narudzbe",
                column: "AdresaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Artikli");

            migrationBuilder.DropTable(
                name: "ArtikliNarudzbe");

            migrationBuilder.DropTable(
                name: "Kartice");

            migrationBuilder.DropTable(
                name: "KorpaStavkeKorpe");

            migrationBuilder.DropTable(
                name: "Korpe");

            migrationBuilder.DropTable(
                name: "Narudzbe");

            migrationBuilder.DropTable(
                name: "Placanja");

            migrationBuilder.DropTable(
                name: "Popusti");

            migrationBuilder.DropTable(
                name: "QRKodovi");

            migrationBuilder.DropTable(
                name: "Racuni");

            migrationBuilder.DropTable(
                name: "StavkeKorpe");

            migrationBuilder.DropTable(
                name: "TipoviOdjece");

            migrationBuilder.DropTable(
                name: "Adrese");
        }
    }
}
