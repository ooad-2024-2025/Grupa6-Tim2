using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DressCode.Data.Migrations
{
    public partial class neznija : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Drop old primary key
            migrationBuilder.DropPrimaryKey(
                name: "PK_StavkaKorpe",
                table: "StavkaKorpe");

            // 2. Drop old string Id column
            migrationBuilder.DropColumn(
                name: "Id",
                table: "StavkaKorpe");

            // 3. Add new int Id column with Identity
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "StavkaKorpe",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            // 4. Re-add primary key on new Id
            migrationBuilder.AddPrimaryKey(
                name: "PK_StavkaKorpe",
                table: "StavkaKorpe",
                column: "Id");

            // 5. Add new ArtikalId column
            migrationBuilder.AddColumn<int>(
                name: "ArtikalId",
                table: "StavkaKorpe",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1. Drop primary key on int Id
            migrationBuilder.DropPrimaryKey(
                name: "PK_StavkaKorpe",
                table: "StavkaKorpe");

            // 2. Drop ArtikalId and int Id columns
            migrationBuilder.DropColumn(
                name: "ArtikalId",
                table: "StavkaKorpe");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "StavkaKorpe");

            // 3. Re-add string Id column
            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "StavkaKorpe",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            // 4. Re-add primary key on string Id
            migrationBuilder.AddPrimaryKey(
                name: "PK_StavkaKorpe",
                table: "StavkaKorpe",
                column: "Id");
        }
    }
}
