using System.Net;
using System.Text.Json;

namespace CircuitoTereVerde.Tests;

[Collection(ApiTesteCollection.Nome)]
public sealed class PrincipalRotasTests
{
    private readonly ApiTesteFixture _fixture;

    public PrincipalRotasTests(ApiTesteFixture fixture)
    {
        _fixture = fixture;
    }

    #region Principal e catálogos públicos

    [Fact]
    public async Task PrincipalListaParquesTiposEPostagensPublicamente()
    {
        using var cliente = _fixture.Factory.CreateClient();

        using var respostaParques = await cliente.GetAsync("/api/parques");
        Assert.Equal(HttpStatusCode.OK, respostaParques.StatusCode);
        using var parques = JsonDocument.Parse(await respostaParques.Content.ReadAsStringAsync());
        Assert.Equal(3, parques.RootElement.GetArrayLength());

        using var respostaTipos = await cliente.GetAsync("/api/tipos-postagem");
        Assert.Equal(HttpStatusCode.OK, respostaTipos.StatusCode);
        using var tipos = JsonDocument.Parse(await respostaTipos.Content.ReadAsStringAsync());
        var codigos = tipos.RootElement.EnumerateArray()
            .Select(item => item.GetProperty("codigo").GetString())
            .ToList();
        Assert.Equal(7, codigos.Count);
        Assert.Contains("ATRAÇÕES", codigos);

        using var respostaPostagens = await cliente.GetAsync("/api/postagens?pagina=1&tamanhoPagina=5");
        Assert.True(respostaPostagens.IsSuccessStatusCode, await respostaPostagens.Content.ReadAsStringAsync());
        using var postagens = JsonDocument.Parse(await respostaPostagens.Content.ReadAsStringAsync());
        Assert.Equal(1, postagens.RootElement.GetProperty("pagina").GetInt32());
        Assert.Equal(5, postagens.RootElement.GetProperty("tamanhoPagina").GetInt32());

        using var respostaDestaques = await cliente.GetAsync("/api/postagens/destaques");
        Assert.Equal(HttpStatusCode.OK, respostaDestaques.StatusCode);
        using var destaques = JsonDocument.Parse(await respostaDestaques.Content.ReadAsStringAsync());
        Assert.True(destaques.RootElement.GetArrayLength() <= 5);
    }

    #endregion
}
