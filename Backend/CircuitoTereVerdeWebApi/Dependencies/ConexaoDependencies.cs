using CircuitoTereVerde.Core.Infra.Connection;
using CircuitoTereVerde.Core.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace CircuitoTereVerdeWebApi.Dependencies;

public static class ConexaoDependencies
{
    public static IServiceCollection AddConexao(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        // A configuração muda por ambiente e pode ser substituída por ConnectionStrings__DefaultConnection.
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Configure ConnectionStrings:DefaultConnection.");
        }

        var connectionNormalizada = SqliteConnectionFactory.Normalizar(connectionString, environment.ContentRootPath);
        services.AddDbContext<CircuitoTereVerdeDbContext>(options => options.UseSqlite(connectionNormalizada));
        services.AddScoped<DbContext>(provider =>
        {
            return provider.GetRequiredService<CircuitoTereVerdeDbContext>();
        });

        return services;
    }
}
