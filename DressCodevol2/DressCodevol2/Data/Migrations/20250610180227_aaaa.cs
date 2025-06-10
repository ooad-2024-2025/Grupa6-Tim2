using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DressCode.Data.Migrations
{
    /// <inheritdoc />
    public partial class aaaa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "imageUrl",
                table: "Artikal",
                newName: "SlikaUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SlikaUrl",
                table: "Artikal",
                newName: "imageUrl");
        }
    }
}
