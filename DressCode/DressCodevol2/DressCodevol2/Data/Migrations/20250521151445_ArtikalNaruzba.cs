using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DressCode.Data.Migrations
{
    /// <inheritdoc />
    public partial class ArtikalNaruzba : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ArtikalNarudzbe",
                table: "ArtikalNarudzbe");

            migrationBuilder.RenameTable(
                name: "ArtikalNarudzbe",
                newName: "ArtikalNarudzba");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ArtikalNarudzba",
                table: "ArtikalNarudzba",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ArtikalNarudzba",
                table: "ArtikalNarudzba");

            migrationBuilder.RenameTable(
                name: "ArtikalNarudzba",
                newName: "ArtikalNarudzbe");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ArtikalNarudzbe",
                table: "ArtikalNarudzbe",
                column: "Id");
        }
    }
}
