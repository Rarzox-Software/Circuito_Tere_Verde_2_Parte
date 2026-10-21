using System.Net;
using System.Text.Json;
using CircuitoTereVerde.Core.Domain;
using CircuitoTereVerde.Core.Infra.Context;
using CircuitoTereVerde.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
        // O CRUD percorre as rotas reais com o cookie do administrador de desenvolvimento.
        await ApiTesteHelpers.EntrarComoAdministradorAsync(cliente, _fixture.Factory);
        await using var escopo = _fixture.Factory.Services.CreateAsyncScope();
        var contexto = escopo.ServiceProvider.GetRequiredService<CircuitoTereVerdeDbContext>();

        var titulo = $"Postagem de teste {_fixture.Factory.EmailAdministrador}";
        var dataInicio = DateOnly.FromDateTime(DateTime.UtcNow);
        var dataFim = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var bytesFoto = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 1, 2, 3, 4 };
        int? idCriado = null;

        // Avança apenas a sequência das fotos para garantir IDs diferentes de postagem e foto.
        // Esse registro auxiliar é salvo e removido no próprio SQLite antes de iniciar o CRUD.
        var fotoAuxiliar = new FotoPostagem { Conteudo = bytesFoto, TipoConteudo = "image/png" };
        contexto.FotosPostagem.Add(fotoAuxiliar);
        await contexto.SaveChangesAsync();
        Assert.True(fotoAuxiliar.Id > 0);
        contexto.FotosPostagem.Remove(fotoAuxiliar);
        await contexto.SaveChangesAsync();

        try
        {
            // O cadastro envia o contrato JSON, com a foto já preparada em Base64 pelo frontend.
            using var conteudo = ApiTesteHelpers.CriarConteudoPostagem(
                titulo, "Descrição criada pelo teste de integração.", dataInicio, dataFim, bytesFoto);
            using var criacao = await cliente.PostAsync("/api/admin/postagens", conteudo);
            Assert.Equal(HttpStatusCode.Created, criacao.StatusCode);
            using var criada = JsonDocument.Parse(await criacao.Content.ReadAsStringAsync());
            idCriado = criada.RootElement.GetProperty("id").GetInt32();
            _fixture.RegistrarPostagemCriada(idCriado.Value);
            var fotoEsperada = $"data:image/png;base64,{Convert.ToBase64String(bytesFoto)}";
            Assert.Equal(fotoEsperada, criada.RootElement.GetProperty("foto").GetString());

            // A postagem guarda a chave da foto; cada tabela tem seu próprio ID automático.
            var persistida = await contexto.Postagens.AsNoTracking().SingleAsync(item => item.Id == idCriado.Value);
            var idFoto = persistida.FotoPostagemId;
            Assert.True(persistida.Id > 0);
            Assert.True(idFoto > 0);
            Assert.NotEqual(persistida.Id, idFoto);
            Assert.Equal(idFoto, persistida.FotoPostagem.Id);
            Assert.Equal(bytesFoto, persistida.FotoPostagem.Conteudo);

            // A listagem pública devolve o mesmo conteúdo da foto, sem uma rota separada para ela.
            using var requisicaoPublica = new HttpRequestMessage(HttpMethod.Get, "/api/postagens");
            requisicaoPublica.Headers.Add("tipo", "EVENTOS");
            using var respostaPublica = await cliente.SendAsync(requisicaoPublica);
            Assert.Equal(HttpStatusCode.OK, respostaPublica.StatusCode);
            using var listagem = JsonDocument.Parse(await respostaPublica.Content.ReadAsStringAsync());
            var postagemPublica = listagem.RootElement.GetProperty("itens").EnumerateArray()
                .Single(item => item.GetProperty("id").GetInt32() == idCriado.Value);
            Assert.Equal(titulo, postagemPublica.GetProperty("titulo").GetString());
            Assert.Equal(fotoEsperada, postagemPublica.GetProperty("foto").GetString());

            using var clientePublico = _fixture.Factory.CreateClient();

            // Uma edição sem novo arquivo deve preservar a foto já salva.
            using var formularioAtualizacao = new MultipartFormDataContent();
            formularioAtualizacao.Add(new StringContent(CodigosCatalogo.Parques.Pnso), "Parque");
            formularioAtualizacao.Add(new StringContent(CodigosCatalogo.Tipos.Eventos), "Tipo");
            formularioAtualizacao.Add(new StringContent($"{titulo} editada"), "Titulo");
            formularioAtualizacao.Add(new StringContent("Descrição atualizada pelo teste."), "Descricao");
            formularioAtualizacao.Add(new StringContent(dataInicio.ToString("yyyy-MM-dd")), "DataInicio");
            formularioAtualizacao.Add(new StringContent(dataFim.ToString("yyyy-MM-dd")), "DataFim");

            using var atualizacao = await ApiTesteHelpers.EnviarComIdAsync(
                cliente, HttpMethod.Put, "/api/admin/postagens", idCriado.Value, formularioAtualizacao);
            Assert.Equal(HttpStatusCode.OK, atualizacao.StatusCode);
            using var atualizada = JsonDocument.Parse(await atualizacao.Content.ReadAsStringAsync());
            Assert.Equal($"{titulo} editada", atualizada.RootElement.GetProperty("titulo").GetString());
            Assert.Equal(fotoEsperada, atualizada.RootElement.GetProperty("foto").GetString());

            // A troca da imagem usa FotoPostagemId, mesmo quando ele é diferente do ID da postagem.
            var novosBytes = new byte[] { 82, 73, 70, 70, 10, 20, 30, 40 };
            using var formularioFoto = ApiTesteHelpers.CriarFormularioPostagem(
                $"{titulo} editada", "Descrição com nova foto.",
                dataInicio, dataFim, novosBytes, tipoFoto: "image/webp");
            using var trocaFoto = await ApiTesteHelpers.EnviarComIdAsync(
                cliente, HttpMethod.Put, "/api/admin/postagens", idCriado.Value, formularioFoto);
            Assert.Equal(HttpStatusCode.OK, trocaFoto.StatusCode);
            fotoEsperada = $"data:image/webp;base64,{Convert.ToBase64String(novosBytes)}";
            using var comNovaFoto = JsonDocument.Parse(await trocaFoto.Content.ReadAsStringAsync());
            Assert.Equal(fotoEsperada, comNovaFoto.RootElement.GetProperty("foto").GetString());
            var fotoAtualizada = await contexto.FotosPostagem.AsNoTracking().SingleAsync(item => item.Id == idFoto);
            Assert.Equal(novosBytes, fotoAtualizada.Conteudo);
            Assert.Equal("image/webp", fotoAtualizada.TipoConteudo);
            Assert.Equal(idFoto, await contexto.Postagens.Where(item => item.Id == idCriado.Value)
                .Select(item => item.FotoPostagemId).SingleAsync());

            // Ao encerrar o período, a postagem sai da principal, mas continua disponível para o administrador.
            var dataInicioEncerrada = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2));
            var dataFimEncerrada = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
            using var formularioEncerrado = new MultipartFormDataContent();
            formularioEncerrado.Add(new StringContent(CodigosCatalogo.Parques.Pnso), "Parque");
            formularioEncerrado.Add(new StringContent(CodigosCatalogo.Tipos.Eventos), "Tipo");
            formularioEncerrado.Add(new StringContent($"{titulo} encerrada"), "Titulo");
            formularioEncerrado.Add(new StringContent("Evento encerrado pelo teste."), "Descricao");
            formularioEncerrado.Add(new StringContent(dataInicioEncerrada.ToString("yyyy-MM-dd")), "DataInicio");
            formularioEncerrado.Add(new StringContent(dataFimEncerrada.ToString("yyyy-MM-dd")), "DataFim");

            using var encerramento = await ApiTesteHelpers.EnviarComIdAsync(
                cliente, HttpMethod.Put, "/api/admin/postagens", idCriado.Value, formularioEncerrado);
            Assert.Equal(HttpStatusCode.OK, encerramento.StatusCode);

            using var requisicaoPublicaEncerrada = new HttpRequestMessage(HttpMethod.Get, "/api/postagens");
            requisicaoPublicaEncerrada.Headers.Add("tipo", "EVENTOS");
            using var listagemPublicaEncerrada = await clientePublico.SendAsync(requisicaoPublicaEncerrada);
            Assert.Equal(HttpStatusCode.OK, listagemPublicaEncerrada.StatusCode);
            using var itensPublicos = JsonDocument.Parse(await listagemPublicaEncerrada.Content.ReadAsStringAsync());
            Assert.DoesNotContain(itensPublicos.RootElement.GetProperty("itens").EnumerateArray(), item =>
                item.GetProperty("id").GetInt32() == idCriado.Value);

            using var requisicaoDetalheEncerrada = ApiTesteHelpers.CriarRequisicaoComId(
                HttpMethod.Get, "/api/postagens/detalhe", idCriado.Value);
            using var detalhePublicoEncerrado = await clientePublico.SendAsync(requisicaoDetalheEncerrada);
            Assert.Equal(HttpStatusCode.NotFound, detalhePublicoEncerrado.StatusCode);

            using var requisicaoAdministrativaEncerrada = ApiTesteHelpers.CriarRequisicaoComId(
                HttpMethod.Get, "/api/admin/postagens/selecionada", idCriado.Value);
            using var postagemAdministrativaEncerrada = await cliente.SendAsync(requisicaoAdministrativaEncerrada);
            Assert.Equal(HttpStatusCode.OK, postagemAdministrativaEncerrada.StatusCode);
            using var dadosAdministrativos = JsonDocument.Parse(
                await postagemAdministrativaEncerrada.Content.ReadAsStringAsync());
            Assert.Equal(fotoEsperada, dadosAdministrativos.RootElement.GetProperty("foto").GetString());

            // Excluir pela API precisa remover a postagem e a foto vinculada no banco.
            var idPostagem = idCriado.Value;
            using var exclusao = await ApiTesteHelpers.EnviarComIdAsync(
                cliente, HttpMethod.Delete, "/api/admin/postagens", idPostagem);
            Assert.Equal(HttpStatusCode.NoContent, exclusao.StatusCode);
            idCriado = null;
            Assert.False(await contexto.Postagens.AnyAsync(item => item.Id == idPostagem));
            Assert.False(await contexto.FotosPostagem.AnyAsync(item => item.Id == idFoto));

            using var requisicaoRemovida = ApiTesteHelpers.CriarRequisicaoComId(
                HttpMethod.Get, "/api/admin/postagens/selecionada", idPostagem);
            using var removida = await cliente.SendAsync(requisicaoRemovida);
            Assert.Equal(HttpStatusCode.NotFound, removida.StatusCode);
        }
        finally
        {
            // Se uma validação falhar antes da exclusão, remove somente o registro criado por este teste.
            if (idCriado.HasValue)
            {
                using var exclusao = await ApiTesteHelpers.EnviarComIdAsync(
                    cliente, HttpMethod.Delete, "/api/admin/postagens", idCriado.Value);
            }
        }
    }

    #endregion

    #region Validações do cadastro e registros inexistentes

    [Theory]
    [InlineData("titulo_vazio")]
    [InlineData("titulo_longo")]
    [InlineData("descricao_vazia")]
    [InlineData("descricao_longa")]
    [InlineData("inicio_ausente")]
    [InlineData("fim_antes_inicio")]
    [InlineData("parque_invalido")]
    [InlineData("tipo_invalido")]
    [InlineData("foto_ausente")]
    [InlineData("foto_vazia")]
    [InlineData("foto_formato_invalido")]
    [InlineData("foto_acima_limite")]
    public async Task CadastroRejeitaDadosInvalidosSemSalvar(string caso)
    {
        using var cliente = _fixture.Factory.CreateClient();
        await ApiTesteHelpers.EntrarComoAdministradorAsync(cliente, _fixture.Factory);
        await using var escopo = _fixture.Factory.Services.CreateAsyncScope();
        var contexto = escopo.ServiceProvider.GetRequiredService<CircuitoTereVerdeDbContext>();
        var totalPostagens = await contexto.Postagens.CountAsync();
        var totalFotos = await contexto.FotosPostagem.CountAsync();

        // Parte de um cadastro válido e altera somente o campo do caso que está sendo verificado.
        var titulo = "Postagem para validar dados";
        var descricao = "Descrição válida.";
        DateOnly? inicio = DateOnly.FromDateTime(DateTime.UtcNow);
        DateOnly? fim = null;
        var parque = "PNSO";
        var tipo = "EVENTOS";
        byte[]? foto = [1, 2, 3];
        var tipoFoto = "image/png";

        switch (caso)
        {
            case "titulo_vazio": titulo = " "; break;
            case "titulo_longo": titulo = new string('a', 161); break;
            case "descricao_vazia": descricao = " "; break;
            case "descricao_longa": descricao = new string('a', 5001); break;
            case "inicio_ausente": inicio = null; break;
            case "fim_antes_inicio": fim = inicio.Value.AddDays(-1); break;
            case "parque_invalido": parque = "INEXISTENTE"; break;
            case "tipo_invalido": tipo = "INEXISTENTE"; break;
            case "foto_ausente": foto = null; break;
            case "foto_vazia": foto = []; break;
            case "foto_formato_invalido": tipoFoto = "image/gif"; break;
            case "foto_acima_limite": foto = new byte[5 * 1024 * 1024 + 1]; break;
        }

        using var conteudo = ApiTesteHelpers.CriarConteudoPostagem(
            titulo, descricao, inicio, fim, foto, parque, tipo, tipoFoto);
        using var resposta = await cliente.PostAsync("/api/admin/postagens", conteudo);
        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        using var problema = JsonDocument.Parse(await resposta.Content.ReadAsStringAsync());
        Assert.False(string.IsNullOrWhiteSpace(problema.RootElement.GetProperty("title").GetString()));

        // Uma entrada inválida não pode deixar uma postagem ou uma foto parcial no SQLite.
        Assert.Equal(totalPostagens, await contexto.Postagens.CountAsync());
        Assert.Equal(totalFotos, await contexto.FotosPostagem.CountAsync());
    }

    [Fact]
    public async Task ConsultarEditarEExcluirIdInexistenteRecebe404()
    {
        using var cliente = _fixture.Factory.CreateClient();
        await ApiTesteHelpers.EntrarComoAdministradorAsync(cliente, _fixture.Factory);
        const int idInexistente = int.MaxValue;

        // Os IDs são enviados no cabeçalho id, inclusive quando o registro não existe.
        foreach (var rota in new[] { "/api/postagens/detalhe", "/api/admin/postagens/selecionada" })
        {
            using var resposta = await ApiTesteHelpers.EnviarComIdAsync(cliente, HttpMethod.Get, rota, idInexistente);
            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }

        using var formulario = ApiTesteHelpers.CriarFormularioPostagem(
            "Título válido", "Descrição válida.", DateOnly.FromDateTime(DateTime.UtcNow), null, null);
        using var edicao = await ApiTesteHelpers.EnviarComIdAsync(
            cliente, HttpMethod.Put, "/api/admin/postagens", idInexistente, formulario);
        Assert.Equal(HttpStatusCode.NotFound, edicao.StatusCode);

        using var exclusao = await ApiTesteHelpers.EnviarComIdAsync(
            cliente, HttpMethod.Delete, "/api/admin/postagens", idInexistente);
        Assert.Equal(HttpStatusCode.NotFound, exclusao.StatusCode);
    }

    #endregion
}
