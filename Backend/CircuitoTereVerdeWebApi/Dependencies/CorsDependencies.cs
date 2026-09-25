namespace CircuitoTereVerdeWebApi.Dependencies;

public static class CorsDependencies
{
    public static IServiceCollection AddCorsFrontend(this IServiceCollection services, IConfiguration configuration)
    {
        var origens = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        services.AddCors(options => options.AddPolicy("Frontend", policy =>
        {
            if (origens.Length > 0)
            {
                policy.WithOrigins(origens).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
            }
        }));

        return services;
    }
}
