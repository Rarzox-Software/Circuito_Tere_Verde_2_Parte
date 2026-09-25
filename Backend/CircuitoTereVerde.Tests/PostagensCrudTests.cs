using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using CircuitoTereVerde.Core.Domain;

namespace CircuitoTereVerde.Tests;

[Collection(ApiTesteCollection.Nome)]
public sealed class PostagensCrudTests
{
    private readonly ApiTesteFixture _fixture;

    public PostagensCrudTests(ApiTesteFixture fixture)
    {
        _fixture = fixture;
    }

    #region Cadastro, consulta, edição, imagem e exclusão

    [Fact]
    public async Task AdministradorFazCrudEImagemPermaneceNoBanco()
    {
        using var cliente = _fixture.Factory.CreateClient();
        await ApiTesteHelpers.EntrarComoAdministradorAsync(cliente, _fixture.Factory);

        var titulo = $"Postagem de teste {_fixture.Factory.EmailAdministrador}";
        var dataInicio = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd");
        var dataFim = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)).ToString("yyyy-MM-dd");
        var bytesFoto = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 1, 2, 3, 4 };
        int? idCriado = null;

        try
        {
            using var formulario = new MultipartFormDataContent();
            formulario.Add(new StringContent(CodigosCatalogo.Parques.Pnso), "Parque");
            formulario.Add(new StringContent(CodigosCatalogo.Tipos.Eventos), "Tipo");
            formulario.Add(new StringContent(titulo), "Titulo");
            formulario.Add(new StringContent("Descrição criada pelo teste de integração."), "Descricao");
            formulario.Add(new StringContent(dataInicio), "DataInicio");
            formulario.Add(new StringContent(dataFim), "DataFim");
            var conteudoFoto = new ByteArrayContent(bytesFoto);
            conteudoFoto.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            formulario.Add(conteudoFoto, "foto", "teste.png");

            using var criacao = await ApiTesteHelpers.EnviarComCsrfAsync(
                cliente, HttpMethod.Post, "/api/admin/postagens", formulario);
            Assert.Equal(HttpStatusCode.Created, criacao.StatusCode);
            using var criada = JsonDocument.Parse(await criacao.Content.ReadAsStringAsync());
            idCriado = criada.RootElement.GetProperty("id").GetInt32();
            _fixture.RegistrarPostagemCriada(idCriado.Value);

            using var respostaPublica = await cliente.GetAsync("/api/postagens?tipo=EVENTOS");
            Assert.Equal(HttpStatusCode.OK, respostaPublica.StatusCode);
            using var listagem = JsonDocument.Parse(await respostaPublica.Content.ReadAsStringAsync());
            Assert.Contains(listagem.RootElement.GetProperty("itens").EnumerateArray(), item =>
                item.GetProperty("id").GetInt32() == idCriado.Value
                    && item.GetProperty("titulo").GetString() == titulo);

            using var clientePublico = _fixture.Factory.CreateClient();
            using var fotoPublica = await clientePublico.GetAsync($"/api/postagens/{idCriado}/foto");
            Assert.Equal(HttpStatusCode.OK, fotoPublica.StatusCode);
            Assert.Equal(bytesFoto, await fotoPublica.Content.ReadAsByteArrayAsync());
            Assert.Equal("image/png", fotoPublica.Content.Headers.ContentType?.MediaType);

            using var formularioAtualizacao = new MultipartFormDataContent();
            formularioAtualizacao.Add(new StringContent(CodigosCatalogo.Parques.Pnso), "Parque");
            formularioAtualizacao.Add(new StringContent(CodigosCatalogo.Tipos.Eventos), "Tipo");
            formularioAtualizacao.Add(new StringContent($"{titulo} editada"), "Titulo");
            formularioAtualizacao.Add(new StringContent("Descrição atualizada pelo teste."), "Descricao");
            formularioAtualizacao.Add(new StringContent(dataInicio), "DataInicio");
            formularioAtualizacao.Add(new StringContent(dataFim), "DataFim");

            using var atualizacao = await ApiTesteHelpers.EnviarComCsrfAsync(
                cliente, HttpMethod.Put, $"/api/admin/postagens/{idCriado}", formularioAtualizacao);
            Assert.Equal(HttpStatusCode.OK, atualizacao.StatusCode);
            using var atualizada = JsonDocument.Parse(await atualizacao.Content.ReadAsStringAsync());
            Assert.Equal($"{titulo} editada", atualizada.RootElement.GetProperty("titulo").GetString());

            var dataInicioEncerrada = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2)).ToString("yyyy-MM-dd");
            var dataFimEncerrada = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)).ToString("yyyy-MM-dd");
            using var formularioEncerrado = new MultipartFormDataContent();
            formularioEncerrado.Add(new StringContent(CodigosCatalogo.Parques.Pnso), "Parque");
            formularioEncerrado.Add(new StringContent(CodigosCatalogo.Tipos.Eventos), "Tipo");
            formularioEncerrado.Add(new StringContent($"{titulo} encerrada"), "Titulo");
            formularioEncerrado.Add(new StringContent("Evento encerrado pelo teste."), "Descricao");
            formularioEncerrado.Add(new StringContent(dataInicioEncerrada), "DataInicio");
            formularioEncerrado.Add(new StringContent(dataFimEncerrada), "DataFim");

            using var encerramento = await ApiTesteHelpers.EnviarComCsrfAsync(
                cliente, HttpMethod.Put, $"/api/admin/postagens/{idCriado}", formularioEncerrado);
            Assert.Equal(HttpStatusCode.OK, encerramento.StatusCode);

            using var listagemPublicaEncerrada = await clientePublico.GetAsync("/api/postagens?tipo=EVENTOS");
            Assert.Equal(HttpStatusCode.OK, listagemPublicaEncerrada.StatusCode);
            using var itensPublicos = JsonDocument.Parse(await listagemPublicaEncerrada.Content.ReadAsStringAsync());
            Assert.DoesNotContain(itensPublicos.RootElement.GetProperty("itens").EnumerateArray(), item =>
                item.GetProperty("id").GetInt32() == idCriado.Value);

            using var fotoPublicaEncerrada = await clientePublico.GetAsync($"/api/postagens/{idCriado}/foto");
            Assert.Equal(HttpStatusCode.NotFound, fotoPublicaEncerrada.StatusCode);

            using var fotoAdministrativaEncerrada = await cliente.GetAsync($"/api/postagens/{idCriado}/foto");
            Assert.Equal(HttpStatusCode.OK, fotoAdministrativaEncerrada.StatusCode);

            var idPostagem = idCriado.Value;
            using var exclusao = await ApiTesteHelpers.EnviarComCsrfAsync(
                cliente, HttpMethod.Delete, $"/api/admin/postagens/{idPostagem}");
            Assert.Equal(HttpStatusCode.NoContent, exclusao.StatusCode);
            idCriado = null;

            using var removida = await cliente.GetAsync($"/api/admin/postagens/{idPostagem}");
            Assert.Equal(HttpStatusCode.NotFound, removida.StatusCode);
        }
        finally
        {
            if (idCriado.HasValue)
            {
                using var exclusao = await ApiTesteHelpers.EnviarComCsrfAsync(
                    cliente, HttpMethod.Delete, $"/api/admin/postagens/{idCriado.Value}");
            }
        }
    }

    #endregion
}
