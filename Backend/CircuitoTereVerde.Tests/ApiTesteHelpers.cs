using System.Net.Http.Json;
using System.Text.Json;

namespace CircuitoTereVerde.Tests;

// Centraliza as chamadas de autenticação e o token CSRF usados pelos testes de rota.
internal static class ApiTesteHelpers
{
    public static async Task EntrarComoAdministradorAsync(HttpClient cliente, ApiTesteFactory factory)
    {
        var token = await ObterTokenCsrfAsync(cliente);
        using var requisicao = new HttpRequestMessage(HttpMethod.Post, "/api/auth/login")
        {
            Content = JsonContent.Create(new
            {
                email = factory.EmailAdministrador,
                senha = factory.SenhaAdministrador
            })
        };
        requisicao.Headers.Add("X-XSRF-TOKEN", token);
        using var resposta = await cliente.SendAsync(requisicao);
        if (!resposta.IsSuccessStatusCode)
        {
            var detalhe = await resposta.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Login respondeu {(int)resposta.StatusCode}: {detalhe}");
        }
    }

    public static async Task<HttpResponseMessage> EnviarComCsrfAsync(
        HttpClient cliente,
        HttpMethod metodo,
        string caminho,
        HttpContent? conteudo = null)
    {
        var token = await ObterTokenCsrfAsync(cliente);
        using var requisicao = new HttpRequestMessage(metodo, caminho)
        {
            Content = conteudo
        };
        requisicao.Headers.Add("X-XSRF-TOKEN", token);
        return await cliente.SendAsync(requisicao);
    }

    private static async Task<string> ObterTokenCsrfAsync(HttpClient cliente)
    {
        using var resposta = await cliente.GetAsync("/api/auth/csrf");
        if (!resposta.IsSuccessStatusCode)
        {
            var detalhe = await resposta.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"A API de teste respondeu {(int)resposta.StatusCode}: {detalhe}");
        }
        using var documento = JsonDocument.Parse(await resposta.Content.ReadAsStringAsync());
        return documento.RootElement.GetProperty("token").GetString()!;
    }
}
