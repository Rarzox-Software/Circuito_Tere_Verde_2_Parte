using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;

namespace CircuitoTereVerdeWebApi.Dependencies;

public static class ProxyReversoDependencies
{
    private const string ConfiarNosCabecalhos = "ProxyReverso:ConfiarNosCabecalhos";

    public static IServiceCollection AddProxyReverso(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (!configuration.GetValue<bool>(ConfiarNosCabecalhos))
        {
            return services;
        }

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            // O Nginx informa o protocolo original usado pelo visitante.
            options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;

            // A API fica interna na VPS e recebe requisições externas somente pelo Nginx.
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        return services;
    }

    public static IApplicationBuilder UseProxyReverso(
        this IApplicationBuilder app,
        IConfiguration configuration)
    {
        if (configuration.GetValue<bool>(ConfiarNosCabecalhos))
        {
            app.UseForwardedHeaders();
        }

        return app;
    }
}
