using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DressCode.Data.Migrations
{
    /// <inheritdoc />
    public partial class KorpaStavkeKorpe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artikli_TipoviOdjece_KategorijaId",
                table: "Artikli");

            migrationBuilder.DropForeignKey(
                name: "FK_Narudzbe_Adrese_AdresaId",
                table: "Narudzbe");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TipoviOdjece",
                table: "TipoviOdjece");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StavkeKorpe",
                table: "StavkeKorpe");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Racuni",
                table: "Racuni");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QRKodovi",
                table: "QRKodovi");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Popusti",
                table: "Popusti");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Placanja",
                table: "Placanja");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Narudzbe",
                table: "Narudzbe");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Korpe",
                table: "Korpe");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KorpaStavkeKorpe",
                table: "KorpaStavkeKorpe");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Kartice",
                table: "Kartice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ArtikliNarudzbe",
                table: "ArtikliNarudzbe");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Artikli",
                table: "Artikli");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Adrese",
                table: "Adrese");

            migrationBuilder.RenameTable(
                name: "TipoviOdjece",
                newName: "TipOdjece");

            migrationBuilder.RenameTable(
                name: "StavkeKorpe",
                newName: "StavkaKorpe");

            migrationBuilder.RenameTable(
                name: "Racuni",
                newName: "Racun");

            migrationBuilder.RenameTable(
                name: "QRKodovi",
                newName: "QRKod");

            migrationBuilder.RenameTable(
                name: "Popusti",
                newName: "Popust");

            migrationBuilder.RenameTable(
                name: "Placanja",
                newName: "Placanje");

            migrationBuilder.RenameTable(
                name: "Narudzbe",
                newName: "Narudzba");

            migrationBuilder.RenameTable(
                name: "Korpe",
                newName: "Korpa");

            migrationBuilder.RenameTable(
                name: "KorpaStavkeKorpe",
                newName: "KorpaStavkaKorpe");

            migrationBuilder.RenameTable(
                name: "Kartice",
                newName: "Kartica");

            migrationBuilder.RenameTable(
                name: "ArtikliNarudzbe",
                newName: "ArtikalNarudzbe");

            migrationBuilder.RenameTable(
                name: "Artikli",
                newName: "Artikal");

            migrationBuilder.RenameTable(
                name: "Adrese",
                newName: "Adresa");

            migrationBuilder.RenameIndex(
                name: "IX_Narudzbe_AdresaId",
                table: "Narudzba",
                newName: "IX_Narudzba_AdresaId");

            migrationBuilder.RenameIndex(
                name: "IX_Artikli_KategorijaId",
                table: "Artikal",
                newName: "IX_Artikal_KategorijaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TipOdjece",
                table: "TipOdjece",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StavkaKorpe",
                table: "StavkaKorpe",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Racun",
                table: "Racun",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QRKod",
                table: "QRKod",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Popust",
                table: "Popust",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Placanje",
                table: "Placanje",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Narudzba",
                table: "Narudzba",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Korpa",
                table: "Korpa",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KorpaStavkaKorpe",
                table: "KorpaStavkaKorpe",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Kartica",
                table: "Kartica",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ArtikalNarudzbe",
                table: "ArtikalNarudzbe",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Artikal",
                table: "Artikal",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Adresa",
                table: "Adresa",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Artikal_TipOdjece_KategorijaId",
                table: "Artikal",
                column: "KategorijaId",
                principalTable: "TipOdjece",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Narudzba_Adresa_AdresaId",
                table: "Narudzba",
                column: "AdresaId",
                principalTable: "Adresa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artikal_TipOdjece_KategorijaId",
                table: "Artikal");

            migrationBuilder.DropForeignKey(
                name: "FK_Narudzba_Adresa_AdresaId",
                table: "Narudzba");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TipOdjece",
                table: "TipOdjece");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StavkaKorpe",
                table: "StavkaKorpe");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Racun",
                table: "Racun");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QRKod",
                table: "QRKod");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Popust",
                table: "Popust");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Placanje",
                table: "Placanje");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Narudzba",
                table: "Narudzba");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KorpaStavkaKorpe",
                table: "KorpaStavkaKorpe");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Korpa",
                table: "Korpa");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Kartica",
                table: "Kartica");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ArtikalNarudzbe",
                table: "ArtikalNarudzbe");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Artikal",
                table: "Artikal");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Adresa",
                table: "Adresa");

            migrationBuilder.RenameTable(
                name: "TipOdjece",
                newName: "TipoviOdjece");

            migrationBuilder.RenameTable(
                name: "StavkaKorpe",
                newName: "StavkeKorpe");

            migrationBuilder.RenameTable(
                name: "Racun",
                newName: "Racuni");

            migrationBuilder.RenameTable(
                name: "QRKod",
                newName: "QRKodovi");

            migrationBuilder.RenameTable(
                name: "Popust",
                newName: "Popusti");

            migrationBuilder.RenameTable(
                name: "Placanje",
                newName: "Placanja");

            migrationBuilder.RenameTable(
                name: "Narudzba",
                newName: "Narudzbe");

            migrationBuilder.RenameTable(
                name: "KorpaStavkaKorpe",
                newName: "KorpaStavkeKorpe");

            migrationBuilder.RenameTable(
                name: "Korpa",
                newName: "Korpe");

            migrationBuilder.RenameTable(
                name: "Kartica",
                newName: "Kartice");

            migrationBuilder.RenameTable(
                name: "ArtikalNarudzbe",
                newName: "ArtikliNarudzbe");

            migrationBuilder.RenameTable(
                name: "Artikal",
                newName: "Artikli");

            migrationBuilder.RenameTable(
                name: "Adresa",
                newName: "Adrese");

            migrationBuilder.RenameIndex(
                name: "IX_Narudzba_AdresaId",
                table: "Narudzbe",
                newName: "IX_Narudzbe_AdresaId");

            migrationBuilder.RenameIndex(
                name: "IX_Artikal_KategorijaId",
                table: "Artikli",
                newName: "IX_Artikli_KategorijaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TipoviOdjece",
                table: "TipoviOdjece",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StavkeKorpe",
                table: "StavkeKorpe",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Racuni",
                table: "Racuni",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QRKodovi",
                table: "QRKodovi",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Popusti",
                table: "Popusti",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Placanja",
                table: "Placanja",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Narudzbe",
                table: "Narudzbe",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KorpaStavkeKorpe",
                table: "KorpaStavkeKorpe",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Korpe",
                table: "Korpe",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Kartice",
                table: "Kartice",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ArtikliNarudzbe",
                table: "ArtikliNarudzbe",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Artikli",
                table: "Artikli",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Adrese",
                table: "Adrese",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Artikli_TipoviOdjece_KategorijaId",
                table: "Artikli",
                column: "KategorijaId",
                principalTable: "TipoviOdjece",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Narudzbe_Adrese_AdresaId",
                table: "Narudzbe",
                column: "AdresaId",
                principalTable: "Adrese",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
