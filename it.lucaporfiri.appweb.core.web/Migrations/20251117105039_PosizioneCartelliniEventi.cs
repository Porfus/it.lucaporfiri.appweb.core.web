using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace it.lucaporfiri.appweb.core.web.Migrations
{
    /// <inheritdoc />
    public partial class PosizioneCartelliniEventi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrdineInTipoEvento",
                table: "Eventi");

            migrationBuilder.AddColumn<double>(
                name: "Posizione",
                table: "Eventi",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Posizione",
                table: "Eventi");

            migrationBuilder.AddColumn<int>(
                name: "OrdineInTipoEvento",
                table: "Eventi",
                type: "int",
                nullable: true);
        }
    }
}
