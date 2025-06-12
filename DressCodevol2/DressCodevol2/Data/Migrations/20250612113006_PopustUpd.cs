using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DressCode.Data.Migrations
{
    /// <inheritdoc />
    public partial class PopustUpd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PristupniKod",
                table: "Popust",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PristupniKod",
                table: "Popust");
        }
    }
}
