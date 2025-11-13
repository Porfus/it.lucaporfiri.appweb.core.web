using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace it.lucaporfiri.appweb.core.web.Migrations
{
    /// <inheritdoc />
    public partial class ordineCartelliniEventi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrdineInTipoEvento",
                table: "Eventi",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrdineInTipoEvento",
                table: "Eventi");
        }
    }
}
