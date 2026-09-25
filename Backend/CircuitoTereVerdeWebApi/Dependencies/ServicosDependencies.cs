using CircuitoTereVerde.Core.Infra.UnitOfWork;
using CircuitoTereVerde.Core.Services;
using CircuitoTereVerdeWebApi.Services;
using Microsoft.EntityFrameworkCore;

namespace CircuitoTereVerdeWebApi.Dependencies;

public static class ServicosDependencies
{
    public static IServiceCollection AddServicos(this IServiceCollection services)
    {
        services.AddControllersWithViews();
        services.AddOpenApi();
        services.AddSwaggerGen();
        services.AddScoped<UnitOfWork>(provider =>
        {
            return new UnitOfWork(provider.GetRequiredService<DbContext>());
        });
        services.AddScoped<CatalogoService>();
        services.AddScoped<PostagemService>();
        services.AddScoped<IdentityAutenticacaoService>();
        services.AddScoped<AdminPostagensService>();
        services.AddScoped<DatabaseInitializer>();
        return services;
    }
}
