using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CircuitoTereVerde.Core.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AjustarRelacionamentoPostagemAdministrador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Postagens_CriadoPorId",
                table: "Postagens",
                column: "CriadoPorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Postagens_AspNetUsers_CriadoPorId",
                table: "Postagens",
                column: "CriadoPorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Postagens_AspNetUsers_CriadoPorId",
                table: "Postagens");

            migrationBuilder.DropIndex(
                name: "IX_Postagens_CriadoPorId",
                table: "Postagens");
        }
    }
}
