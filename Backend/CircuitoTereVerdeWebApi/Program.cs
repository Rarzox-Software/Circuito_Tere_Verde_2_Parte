using CircuitoTereVerde.Core.Domain;
using CircuitoTereVerdeWebApi.Services;
using CircuitoTereVerdeWebApi.Dependencies;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Cada parte da aplicação registra suas dependências na pasta Dependencies.
builder.Services.AddConexao(builder.Configuration, builder.Environment);
builder.Services.AddAutenticacao(builder.Configuration, builder.Environment);
builder.Services.AddCorsFrontend(builder.Configuration);
builder.Services.AddServicos();

var app = builder.Build();

// Converte os erros conhecidos em respostas simples da API.
app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    var status = exception switch
    {
        RegistroNaoEncontradoException => StatusCodes.Status404NotFound,
        ErroDominioException => StatusCodes.Status400BadRequest,
        AntiforgeryValidationException => StatusCodes.Status400BadRequest,
        _ => StatusCodes.Status500InternalServerError
    };
    var problem = new ProblemDetails
    {
        Status = status,
        Title = status == StatusCodes.Status500InternalServerError
            ? "Ocorreu um erro ao processar a solicitação."
            : exception?.Message,
        Detail = status == StatusCodes.Status500InternalServerError ? null : exception?.Message
    };

    context.Response.StatusCode = status;
    await context.Response.WriteAsJsonAsync(problem);
}));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Aplica as migrações, prepara os perfis e cria o administrador configurado.
await using (var scope = app.Services.CreateAsyncScope())
{
    var inicializador = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await inicializador.InitializeAsync();
}

app.Run();

// Expõe o ponto de entrada para os testes de rota.
public partial class Program
{
}
