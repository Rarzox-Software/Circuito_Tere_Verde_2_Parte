using CircuitoTereVerde.Core.Infra.Context;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CircuitoTereVerde.Core.Infra.Migrations
{
    // Registra a etapa de carga de desenvolvimento após a estrutura e os catálogos fixos.
    [DbContext(typeof(CircuitoTereVerdeDbContext))]
    [Migration("20261022225600_DadosDesenvolvimento")]
    public partial class DadosDesenvolvimento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // As postagens de exemplo dependem do administrador definido no appsettings e das fotos em BLOB.
            // Por isso, DatabaseInitializer as inclui somente no ambiente Development após esta migração.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // A remoção dos exemplos é feita apenas pela recriação da base de desenvolvimento.
        }
    }
}
