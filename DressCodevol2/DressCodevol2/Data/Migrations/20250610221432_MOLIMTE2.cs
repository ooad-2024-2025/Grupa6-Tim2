using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DressCode.Data.Migrations
{
    public partial class MOLIMTE2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            // DODAJ kolonu Velicina u Artikal
            migrationBuilder.AddColumn<int>(
                name: "Velicina",
                table: "Artikal",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // DODAJ kolonu Kolicina u Artikal
            migrationBuilder.AddColumn<int>(
                name: "Kolicina",
                table: "Artikal",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // OBRISI kolonu Velicina iz Artikal
            migrationBuilder.DropColumn(
                name: "Velicina",
                table: "Artikal");

            // OBRISI kolonu Kolicina iz Artikal
            migrationBuilder.DropColumn(
                name: "Kolicina",
                table: "Artikal");

       
        }
    }
}
