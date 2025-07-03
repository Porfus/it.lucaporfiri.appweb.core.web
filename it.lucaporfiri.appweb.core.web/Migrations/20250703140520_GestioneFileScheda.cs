using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace it.lucaporfiri.appweb.core.web.Migrations
{
    /// <inheritdoc />
    public partial class GestioneFileScheda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Scheda",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomeFileArchiviato",
                table: "Scheda",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomeFileOriginale",
                table: "Scheda",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Scheda");

            migrationBuilder.DropColumn(
                name: "NomeFileArchiviato",
                table: "Scheda");

            migrationBuilder.DropColumn(
                name: "NomeFileOriginale",
                table: "Scheda");
        }
    }
}
