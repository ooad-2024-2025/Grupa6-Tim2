using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DressCode.Data.Migrations
{
    /// <inheritdoc />
    public partial class KorpaPopust : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Korpa",
                table: "Korpa");

            migrationBuilder.RenameTable(
                name: "Korpa",
                newName: "Korpe");

            migrationBuilder.AddColumn<string>(
                name: "KodPopust",
                table: "Popust",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PopustId",
                table: "Korpe",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Korpe",
                table: "Korpe",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Korpe_PopustId",
                table: "Korpe",
                column: "PopustId");

            migrationBuilder.AddForeignKey(
                name: "FK_Korpe_Popust_PopustId",
                table: "Korpe",
                column: "PopustId",
                principalTable: "Popust",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Korpe_Popust_PopustId",
                table: "Korpe");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Korpe",
                table: "Korpe");

            migrationBuilder.DropIndex(
                name: "IX_Korpe_PopustId",
                table: "Korpe");

            migrationBuilder.DropColumn(
                name: "KodPopust",
                table: "Popust");

            migrationBuilder.DropColumn(
                name: "PopustId",
                table: "Korpe");

            migrationBuilder.RenameTable(
                name: "Korpe",
                newName: "Korpa");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Korpa",
                table: "Korpa",
                column: "Id");
        }
    }
}
