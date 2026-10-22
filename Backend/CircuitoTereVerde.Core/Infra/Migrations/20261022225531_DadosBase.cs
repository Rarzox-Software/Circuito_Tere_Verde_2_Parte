using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CircuitoTereVerde.Core.Infra.Migrations
{
    /// <inheritdoc />
    public partial class DadosBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Parques",
                columns: new[] { "Id", "Codigo", "Nome" },
                values: new object[,]
                {
                    { 1, "PNSO", "Parque Nacional da Serra dos Órgãos" },
                    { 2, "PNMMT", "Parque Natural Municipal Montanhas de Teresópolis" },
                    { 3, "PETP", "Parque Estadual dos Três Picos" }
                });

            migrationBuilder.InsertData(
                table: "TiposPostagem",
                columns: new[] { "Id", "Codigo", "Nome" },
                values: new object[,]
                {
                    { 1, "TRILHAS", "TRILHAS" },
                    { 2, "CACHOEIRAS", "CACHOEIRAS" },
                    { 3, "BIODIVERSIDADE", "BIODIVERSIDADE" },
                    { 4, "EVENTOS", "EVENTOS" },
                    { 5, "NOVIDADES", "NOVIDADES" },
                    { 6, "ATRAÇÕES", "ATRAÇÕES" },
                    { 7, "SOBRE", "SOBRE" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Parques",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Parques",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Parques",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TiposPostagem",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TiposPostagem",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TiposPostagem",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TiposPostagem",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TiposPostagem",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TiposPostagem",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "TiposPostagem",
                keyColumn: "Id",
                keyValue: 7);
        }
    }
}
