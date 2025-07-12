using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace it.lucaporfiri.appweb.core.web.Migrations
{
    /// <inheritdoc />
    public partial class ModelloAtletaFixListeOraNonNullabili : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "Abbonamento",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nome",
                table: "Abbonamento");
        }
    }
}
