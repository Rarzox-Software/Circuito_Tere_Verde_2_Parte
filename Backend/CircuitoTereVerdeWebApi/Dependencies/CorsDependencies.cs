namespace CircuitoTereVerdeWebApi.Dependencies;

public static class CorsDependencies
{
    public const string FrontendCorsPolicy = "CircuitoTereVerdeFrontend";

    public static IServiceCollection AddCorsFrontend(this IServiceCollection services, IConfiguration configuration)
    {
        var origens = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

        services.AddCors(options => options.AddPolicy(FrontendCorsPolicy, policy =>
        {
            // O navegador só recebe permissão quando sua origem estiver no appsettings.
            if (origens.Length > 0)
            {
                policy.WithOrigins(origens).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
            }
        }));

        return services;
    }
}
