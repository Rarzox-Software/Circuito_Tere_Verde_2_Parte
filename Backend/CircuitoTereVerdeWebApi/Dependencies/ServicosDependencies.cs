using CircuitoTereVerde.Core.Infra.UnitOfWork;
using CircuitoTereVerde.Core.Services;
using CircuitoTereVerdeWebApi.Converters;
using CircuitoTereVerdeWebApi.Handlers;
using CircuitoTereVerdeWebApi.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace CircuitoTereVerdeWebApi.Dependencies;

public static class ServicosDependencies
{
    public static IServiceCollection AddServicos(this IServiceCollection services)
    {
        // Mantém a obrigatoriedade indicada explicitamente pelos contratos.
        services.AddControllers(options =>
        {
            options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
        })
        .AddJsonOptions(options =>
        {
            // Não devolve campos nulos que não são úteis ao frontend.
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

            // Uma data final vazia representa uma postagem sem prazo de encerramento.
            options.JsonSerializerOptions.Converters.Add(new DataOnlyNulaJsonConverter());
        });

        // Registra a resposta padrão e a classe que traduz erros da aplicação.
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        // Registra a documentação interativa dos endpoints.
        services.AddSwaggerGen();

        // Registra a unidade de trabalho que cria os repositórios sob demanda.
        services.AddScoped<UnitOfWork>(provider =>
        {
            return new UnitOfWork(provider.GetRequiredService<DbContext>());
        });

        // Registra os serviços que concentram as regras da aplicação.
        services.AddScoped<CatalogoService>();
        services.AddScoped<PostagemService>();
        services.AddScoped<IdentityAutenticacaoService>();
        services.AddScoped<AdminPostagensService>();
        services.AddScoped<DatabaseInitializer>();
        return services;
    }
}
