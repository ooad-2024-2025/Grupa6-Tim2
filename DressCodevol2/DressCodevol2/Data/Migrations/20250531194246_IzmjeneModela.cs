using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DressCode.Data.Migrations
{
    /// <inheritdoc />
    public partial class IzmjeneModela : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "KorisnikId",
                table: "Narudzba",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "KorpaId",
                table: "KorpaStavkaKorpe",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StavkaKorpeId",
                table: "KorpaStavkaKorpe",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "KorisnikID",
                table: "Korpa",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "KorisnikId",
                table: "Kartica",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ArtikalId",
                table: "ArtikalNarudzba",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NarudzbaId",
                table: "ArtikalNarudzba",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KorpaId",
                table: "KorpaStavkaKorpe");

            migrationBuilder.DropColumn(
                name: "StavkaKorpeId",
                table: "KorpaStavkaKorpe");

            migrationBuilder.DropColumn(
                name: "ArtikalId",
                table: "ArtikalNarudzba");

            migrationBuilder.DropColumn(
                name: "NarudzbaId",
                table: "ArtikalNarudzba");

            migrationBuilder.AlterColumn<int>(
                name: "KorisnikId",
                table: "Narudzba",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "KorisnikID",
                table: "Korpa",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "KorisnikId",
                table: "Kartica",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
