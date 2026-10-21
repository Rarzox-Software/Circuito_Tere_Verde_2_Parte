using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Globalization;

namespace CircuitoTereVerde.Tests;

// Centraliza os cabeçalhos e chamadas de autenticação usados pelos testes de rota.
internal static class ApiTesteHelpers
{
    public const string CabecalhoIdPostagem = "id";

    #region Login e cabeçalhos

    public static async Task EntrarComoAdministradorAsync(HttpClient cliente, ApiTesteFactory factory)
    {
        using var resposta = await cliente.PostAsJsonAsync("/api/auth/login", new
        {
            email = factory.EmailAdministrador,
            senha = factory.SenhaAdministrador
        });
        if (!resposta.IsSuccessStatusCode)
        {
            var detalhe = await resposta.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Login respondeu {(int)resposta.StatusCode}: {detalhe}");
        }
    }

    public static HttpRequestMessage CriarRequisicaoComId(
        HttpMethod metodo,
        string caminho,
        int id,
        HttpContent? conteudo = null)
    {
        var requisicao = new HttpRequestMessage(metodo, caminho)
        {
            Content = conteudo
        };
        requisicao.Headers.Add(CabecalhoIdPostagem, id.ToString());
        return requisicao;
    }

    public static async Task<HttpResponseMessage> EnviarComIdAsync(
        HttpClient cliente,
        HttpMethod metodo,
        string caminho,
        int id,
        HttpContent? conteudo = null)
    {
        using var requisicao = CriarRequisicaoComId(metodo, caminho, id, conteudo);
        return await cliente.SendAsync(requisicao);
    }

    #endregion

    #region Contratos enviados pelo frontend

    // Monta o JSON da criação, com a foto já convertida pelo frontend para data URI Base64.
    public static JsonContent CriarConteudoPostagem(
        string titulo,
        string descricao,
        DateOnly? dataInicio,
        DateOnly? dataFim,
        byte[]? foto,
        string parque = "PNSO",
        string tipo = "EVENTOS",
        string tipoFoto = "image/png")
    {
        var fotoDataUri = foto is null
            ? string.Empty
            : $"data:{tipoFoto};base64,{Convert.ToBase64String(foto)}";

        return JsonContent.Create(new
        {
            parque,
            tipo,
            titulo,
            descricao,
            dataInicio,
            dataFim,
            foto = fotoDataUri
        });
    }

    // Monta o formulário multipart usado somente na edição com uma nova foto.
    public static MultipartFormDataContent CriarFormularioPostagem(
        string titulo,
        string descricao,
        DateOnly? dataInicio,
        DateOnly? dataFim,
        byte[]? foto,
        string parque = "PNSO",
        string tipo = "EVENTOS",
        string tipoFoto = "image/png")
    {
        var formulario = new MultipartFormDataContent();
        formulario.Add(new StringContent(parque), "Parque");
        formulario.Add(new StringContent(tipo), "Tipo");
        formulario.Add(new StringContent(titulo), "Titulo");
        formulario.Add(new StringContent(descricao), "Descricao");

        if (dataInicio.HasValue)
        {
            formulario.Add(new StringContent(dataInicio.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)), "DataInicio");
        }

        if (dataFim.HasValue)
        {
            formulario.Add(new StringContent(dataFim.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)), "DataFim");
        }

        if (foto is not null)
        {
            var conteudoFoto = new ByteArrayContent(foto);
            conteudoFoto.Headers.ContentType = new MediaTypeHeaderValue(tipoFoto);
            formulario.Add(conteudoFoto, "foto", "foto.png");
        }

        return formulario;
    }

    #endregion
}
