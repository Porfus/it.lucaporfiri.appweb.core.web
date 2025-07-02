using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace it.lucaporfiri.appweb.core.web.Migrations
{
    /// <inheritdoc />
    public partial class Scheda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appuntamento_Atleta_AtletaId",
                table: "Appuntamento");

            migrationBuilder.DropForeignKey(
                name: "FK_Scheda_Atleta_AtletaId",
                table: "Scheda");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Appuntamento",
                table: "Appuntamento");

            migrationBuilder.RenameTable(
                name: "Appuntamento",
                newName: "Abbonamento");

            migrationBuilder.RenameIndex(
                name: "IX_Appuntamento_AtletaId",
                table: "Abbonamento",
                newName: "IX_Abbonamento_AtletaId");

            migrationBuilder.AlterColumn<int>(
                name: "AtletaId",
                table: "Scheda",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Abbonamento",
                table: "Abbonamento",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Abbonamento_Atleta_AtletaId",
                table: "Abbonamento",
                column: "AtletaId",
                principalTable: "Atleta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Scheda_Atleta_AtletaId",
                table: "Scheda",
                column: "AtletaId",
                principalTable: "Atleta",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Abbonamento_Atleta_AtletaId",
                table: "Abbonamento");

            migrationBuilder.DropForeignKey(
                name: "FK_Scheda_Atleta_AtletaId",
                table: "Scheda");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Abbonamento",
                table: "Abbonamento");

            migrationBuilder.RenameTable(
                name: "Abbonamento",
                newName: "Appuntamento");

            migrationBuilder.RenameIndex(
                name: "IX_Abbonamento_AtletaId",
                table: "Appuntamento",
                newName: "IX_Appuntamento_AtletaId");

            migrationBuilder.AlterColumn<int>(
                name: "AtletaId",
                table: "Scheda",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Appuntamento",
                table: "Appuntamento",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Appuntamento_Atleta_AtletaId",
                table: "Appuntamento",
                column: "AtletaId",
                principalTable: "Atleta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Scheda_Atleta_AtletaId",
                table: "Scheda",
                column: "AtletaId",
                principalTable: "Atleta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
