using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace it.lucaporfiri.appweb.core.web.Migrations
{
    /// <inheritdoc />
    public partial class AtletaIdNullableInEvento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Eventi_Atleta_AtletaId",
                table: "Eventi");

            migrationBuilder.AlterColumn<int>(
                name: "AtletaId",
                table: "Eventi",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Eventi_Atleta_AtletaId",
                table: "Eventi",
                column: "AtletaId",
                principalTable: "Atleta",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Eventi_Atleta_AtletaId",
                table: "Eventi");

            migrationBuilder.AlterColumn<int>(
                name: "AtletaId",
                table: "Eventi",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Eventi_Atleta_AtletaId",
                table: "Eventi",
                column: "AtletaId",
                principalTable: "Atleta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
