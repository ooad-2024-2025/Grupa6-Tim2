using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DressCode.Data.Migrations
{
    /// <inheritdoc />
    public partial class NarudzbaStavka : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NarudzbaStavka",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NarudzbaId = table.Column<int>(type: "int", nullable: false),
                    GrupaId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Velicina = table.Column<int>(type: "int", nullable: false),
                    Kolicina = table.Column<int>(type: "int", nullable: false),
                    CijenaPoKomadu = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NarudzbaStavka", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NarudzbaStavka");

          /*  migrationBuilder.CreateIndex(
                name: "IX_Narudzba_AdresaId",
                table: "Narudzba",
                column: "AdresaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Narudzba_Adresa_AdresaId",
                table: "Narudzba",
                column: "AdresaId",
                principalTable: "Adresa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);*/
        }
    }
}
