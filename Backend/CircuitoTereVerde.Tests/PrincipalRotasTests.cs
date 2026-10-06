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

        // As duas listas de apoio ficam disponíveis ao visitante e vêm dos dados base do SQLite.
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

        // A paginação chega pelos cabeçalhos e deve ser refletida no contrato devolvido ao frontend.
        using var requisicaoPostagens = new HttpRequestMessage(HttpMethod.Get, "/api/postagens");
        requisicaoPostagens.Headers.Add("pagina", "1");
        requisicaoPostagens.Headers.Add("tamanhoPagina", "5");
        using var respostaPostagens = await cliente.SendAsync(requisicaoPostagens);
        Assert.True(respostaPostagens.IsSuccessStatusCode, await respostaPostagens.Content.ReadAsStringAsync());
        using var postagens = JsonDocument.Parse(await respostaPostagens.Content.ReadAsStringAsync());
        Assert.Equal(1, postagens.RootElement.GetProperty("pagina").GetInt32());
        Assert.Equal(5, postagens.RootElement.GetProperty("tamanhoPagina").GetInt32());

        // A principal limita a área de destaques a cinco postagens.
        using var respostaDestaques = await cliente.GetAsync("/api/postagens/destaques");
        Assert.Equal(HttpStatusCode.OK, respostaDestaques.StatusCode);
        using var destaques = JsonDocument.Parse(await respostaDestaques.Content.ReadAsStringAsync());
        Assert.True(destaques.RootElement.GetArrayLength() <= 5);
    }

    [Fact]
    public async Task PrincipalRespeitaPeriodoParqueTipoEDestaques()
    {
        using var administrador = _fixture.Factory.CreateClient();
        await ApiTesteHelpers.EntrarComoAdministradorAsync(administrador, _fixture.Factory);
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        // Cria registros pelas rotas reais: um evento vigente, um futuro e uma postagem SOBRE.
        // O evento vigente começa e termina hoje para verificar que as duas datas são inclusivas.
        var vigente = await CriarPostagemDeTesteAsync(administrador, "EVENTOS", hoje, hoje);
        var futura = await CriarPostagemDeTesteAsync(administrador, "EVENTOS", hoje.AddDays(1), null);
        var sobre = await CriarPostagemDeTesteAsync(administrador, "SOBRE", hoje, null);
        using var visitante = _fixture.Factory.CreateClient();

        using var listagem = await visitante.GetAsync("/api/postagens");
        Assert.Equal(HttpStatusCode.OK, listagem.StatusCode);
        using var postagens = JsonDocument.Parse(await listagem.Content.ReadAsStringAsync());
        var itens = postagens.RootElement.GetProperty("itens").EnumerateArray().ToList();
        Assert.Contains(itens, item => item.GetProperty("id").GetInt32() == vigente);
        Assert.DoesNotContain(itens, item => item.GetProperty("id").GetInt32() == futura);
        Assert.DoesNotContain(itens, item => item.GetProperty("id").GetInt32() == sobre);

        // SOBRE é consultada pelo filtro de tipo; o parque selecionado também chega no cabeçalho.
        using var requisicaoSobre = new HttpRequestMessage(HttpMethod.Get, "/api/postagens");
        requisicaoSobre.Headers.Add("parque", "PNSO");
        requisicaoSobre.Headers.Add("tipo", "SOBRE");
        using var respostaSobre = await visitante.SendAsync(requisicaoSobre);
        Assert.Equal(HttpStatusCode.OK, respostaSobre.StatusCode);
        using var listaSobre = JsonDocument.Parse(await respostaSobre.Content.ReadAsStringAsync());
        var itensSobre = listaSobre.RootElement.GetProperty("itens").EnumerateArray().ToList();
        Assert.Contains(itensSobre, item => item.GetProperty("id").GetInt32() == sobre);
        Assert.All(itensSobre, item => Assert.Equal("SOBRE", item.GetProperty("tipo").GetString()));

        // Ao selecionar outro parque, o evento criado no PNSO não pode aparecer.
        using var requisicaoOutroParque = new HttpRequestMessage(HttpMethod.Get, "/api/postagens");
        requisicaoOutroParque.Headers.Add("parque", "PETP");
        requisicaoOutroParque.Headers.Add("tipo", "EVENTOS");
        using var outroParque = await visitante.SendAsync(requisicaoOutroParque);
        Assert.Equal(HttpStatusCode.OK, outroParque.StatusCode);
        using var listaOutroParque = JsonDocument.Parse(await outroParque.Content.ReadAsStringAsync());
        Assert.DoesNotContain(listaOutroParque.RootElement.GetProperty("itens").EnumerateArray(),
            item => item.GetProperty("id").GetInt32() == vigente);

        // Destaques consideram o período e excluem o tipo SOBRE.
        using var respostaDestaques = await visitante.GetAsync("/api/postagens/destaques");
        Assert.Equal(HttpStatusCode.OK, respostaDestaques.StatusCode);
        using var destaques = JsonDocument.Parse(await respostaDestaques.Content.ReadAsStringAsync());
        var itensDestaques = destaques.RootElement.EnumerateArray().ToList();
        Assert.Contains(itensDestaques, item => item.GetProperty("id").GetInt32() == vigente);
        Assert.DoesNotContain(itensDestaques, item => item.GetProperty("id").GetInt32() == futura);
        Assert.DoesNotContain(itensDestaques, item => item.GetProperty("id").GetInt32() == sobre);

        // O visitante não consulta uma postagem futura; o administrador pode editar o mesmo registro.
        using var detalhePublico = await ApiTesteHelpers.EnviarComIdAsync(
            visitante, HttpMethod.Get, "/api/postagens/detalhe", futura);
        Assert.Equal(HttpStatusCode.NotFound, detalhePublico.StatusCode);
        using var detalheAdministrativo = await ApiTesteHelpers.EnviarComIdAsync(
            administrador, HttpMethod.Get, "/api/admin/postagens/selecionada", futura);
        Assert.Equal(HttpStatusCode.OK, detalheAdministrativo.StatusCode);

        // Uma postagem vigente tem detalhe público com a foto no próprio JSON.
        using var detalheVigente = await ApiTesteHelpers.EnviarComIdAsync(
            visitante, HttpMethod.Get, "/api/postagens/detalhe", vigente);
        Assert.Equal(HttpStatusCode.OK, detalheVigente.StatusCode);
        using var objetoVigente = JsonDocument.Parse(await detalheVigente.Content.ReadAsStringAsync());
        Assert.StartsWith("data:image/png;base64,", objetoVigente.RootElement.GetProperty("foto").GetString());
    }

    [Fact]
    public async Task PaginacaoAjustaLimitesERejeitaCabecalhoInvalido()
    {
        using var cliente = _fixture.Factory.CreateClient();
        // Valores fora dos limites são ajustados para página 1 e no máximo 50 itens.
        using var requisicao = new HttpRequestMessage(HttpMethod.Get, "/api/postagens");
        requisicao.Headers.Add("pagina", "0");
        requisicao.Headers.Add("tamanhoPagina", "1000");
        using var resposta = await cliente.SendAsync(requisicao);
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        using var pagina = JsonDocument.Parse(await resposta.Content.ReadAsStringAsync());
        Assert.Equal(1, pagina.RootElement.GetProperty("pagina").GetInt32());
        Assert.Equal(50, pagina.RootElement.GetProperty("tamanhoPagina").GetInt32());

        // Um cabeçalho que não pode ser convertido para inteiro recebe 400.
        using var invalida = new HttpRequestMessage(HttpMethod.Get, "/api/postagens");
        invalida.Headers.Add("pagina", "abc");
        using var erro = await cliente.SendAsync(invalida);
        Assert.Equal(HttpStatusCode.BadRequest, erro.StatusCode);
    }

    [Fact]
    public async Task SwaggerAbreEDocumentaUploadDeFoto()
    {
        using var cliente = _fixture.Factory.CreateClient();
        // A página e o documento do Swagger precisam abrir sem o erro 500 do upload.
        using var pagina = await cliente.GetAsync("/swagger/index.html");
        Assert.Equal(HttpStatusCode.OK, pagina.StatusCode);

        using var resposta = await cliente.GetAsync("/swagger/v1/swagger.json");
        Assert.True(resposta.IsSuccessStatusCode, await resposta.Content.ReadAsStringAsync());

        using var documento = JsonDocument.Parse(await resposta.Content.ReadAsStringAsync());
        // As duas operações devem descrever o campo foto como um arquivo multipart.
        var operacoes = documento.RootElement.GetProperty("paths").GetProperty("/api/admin/postagens");
        foreach (var metodo in new[] { "post", "put" })
        {
            var formulario = operacoes.GetProperty(metodo).GetProperty("requestBody")
                .GetProperty("content").GetProperty("multipart/form-data");
            var foto = formulario.GetProperty("schema").GetProperty("properties").GetProperty("foto");
            Assert.Equal("binary", foto.GetProperty("format").GetString());
        }

        // O ID de edição deve estar documentado como cabeçalho, conforme o contrato da API.
        var parametrosEdicao = operacoes.GetProperty("put").GetProperty("parameters").EnumerateArray();
        Assert.Contains(parametrosEdicao, parametro => parametro.GetProperty("name").GetString() == "id"
            && parametro.GetProperty("in").GetString() == "header");
    }

    #endregion

    #region Preparação das postagens de teste

    private async Task<int> CriarPostagemDeTesteAsync(HttpClient cliente, string tipo, DateOnly inicio, DateOnly? fim)
    {
        using var formulario = ApiTesteHelpers.CriarFormularioPostagem(
            $"Teste da principal {Guid.NewGuid():N}", "Postagem criada pelo teste.", inicio, fim, [1, 2, 3], tipo: tipo);
        using var resposta = await cliente.PostAsync("/api/admin/postagens", formulario);
        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        using var postagem = JsonDocument.Parse(await resposta.Content.ReadAsStringAsync());
        var id = postagem.RootElement.GetProperty("id").GetInt32();
        // A fixture remove somente as postagens e fotos criadas pela execução dos testes.
        _fixture.RegistrarPostagemCriada(id);
        return id;
    }

    #endregion
}
