using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DressCode.Data.Migrations
{
    /// <inheritdoc />
    public partial class TipOdjeceIzmjena : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bluza",
                table: "TipOdjece");

            migrationBuilder.DropColumn(
                name: "Carape",
                table: "TipOdjece");

            migrationBuilder.DropColumn(
                name: "Dukserica",
                table: "TipOdjece");

            migrationBuilder.DropColumn(
                name: "Haljina",
                table: "TipOdjece");

            migrationBuilder.DropColumn(
                name: "Hlace",
                table: "TipOdjece");

            migrationBuilder.DropColumn(
                name: "Jakna",
                table: "TipOdjece");

            migrationBuilder.DropColumn(
                name: "Kaput",
                table: "TipOdjece");

            migrationBuilder.DropColumn(
                name: "Kosulja",
                table: "TipOdjece");

            migrationBuilder.DropColumn(
                name: "Majica",
                table: "TipOdjece");

            migrationBuilder.DropColumn(
                name: "ModniDodaci",
                table: "TipOdjece");

            migrationBuilder.DropColumn(
                name: "Odijelo",
                table: "TipOdjece");

            migrationBuilder.DropColumn(
                name: "PlaznaOdjeca",
                table: "TipOdjece");

            migrationBuilder.DropColumn(
                name: "SportskaOprema",
                table: "TipOdjece");

            migrationBuilder.DropColumn(
                name: "Suknja",
                table: "TipOdjece");

            migrationBuilder.DropColumn(
                name: "Trenerka",
                table: "TipOdjece");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bluza",
                table: "TipOdjece",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Carape",
                table: "TipOdjece",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Dukserica",
                table: "TipOdjece",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Haljina",
                table: "TipOdjece",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Hlace",
                table: "TipOdjece",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Jakna",
                table: "TipOdjece",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Kaput",
                table: "TipOdjece",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Kosulja",
                table: "TipOdjece",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Majica",
                table: "TipOdjece",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ModniDodaci",
                table: "TipOdjece",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Odijelo",
                table: "TipOdjece",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PlaznaOdjeca",
                table: "TipOdjece",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SportskaOprema",
                table: "TipOdjece",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Suknja",
                table: "TipOdjece",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Trenerka",
                table: "TipOdjece",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
