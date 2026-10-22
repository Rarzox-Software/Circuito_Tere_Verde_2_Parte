using CircuitoTereVerdeWebApi.Dependencies;
using CircuitoTereVerdeWebApi.Services;

// Carrega as configurações e prepara os serviços da aplicação.
var builder = WebApplication.CreateBuilder(args);

// Registra a conexão SQLite e o contexto do Entity Framework.
builder.Services.AddConexao(builder.Configuration, builder.Environment);

// Configura o Identity, o cookie de sessão e as permissões de acesso.
builder.Services.AddAutenticacao(builder.Environment);

// Configura o encaminhamento do protocolo original pelo Nginx, quando habilitado.
builder.Services.AddProxyReverso(builder.Configuration);

// Registra a política CORS que permite as chamadas do frontend.
builder.Services.AddCorsFrontend(builder.Configuration);

// Registra controllers, respostas JSON, tratamento de erros e serviços da API.
builder.Services.AddServicos();

// Constrói a aplicação com os serviços registrados.
var app = builder.Build();

// Lê o protocolo original enviado pelo Nginx antes de criar ou validar o cookie.
app.UseProxyReverso(builder.Configuration);

// Converte as exceções da aplicação em respostas JSON consistentes.
app.UseExceptionHandler();

if (app.Environment.IsProduction())
{
    // Exige HTTPS em produção; o protocolo encaminhado pelo Nginx já foi lido acima.
    app.UseHttpsRedirection();
}

if (app.Environment.IsDevelopment())
{
    // Gera o documento com os endpoints disponíveis na API.
    app.UseSwagger();

    // Disponibiliza a interface para consultar e testar os endpoints.
    app.UseSwaggerUI();
}

// Aplica as origens, os cabeçalhos, os métodos e os cookies permitidos ao frontend.
app.UseCors(CorsDependencies.FrontendCorsPolicy);

// Identifica a sessão presente no cookie da requisição.
app.UseAuthentication();

// Confere os perfis exigidos pelos endpoints administrativos.
app.UseAuthorization();

// Mapeia os endpoints definidos nos controllers.
app.MapControllers();

// Em cada início, aplica migrações, prepara perfis e cria o administrador configurado.
await using (var scope = app.Services.CreateAsyncScope())
{
    var inicializador = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await inicializador.InitializeAsync();
}

app.Run();

// Expõe o ponto de entrada para os testes de integração.
public partial class Program
{
}
