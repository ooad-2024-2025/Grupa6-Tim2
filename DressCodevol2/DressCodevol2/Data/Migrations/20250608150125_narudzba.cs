using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DressCode.Data.Migrations
{
    /// <inheritdoc />
    public partial class narudzba : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArtikalId",
                table: "Narudzba");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArtikalId",
                table: "Narudzba",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
