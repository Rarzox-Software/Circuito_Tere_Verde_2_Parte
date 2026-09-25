using CircuitoTereVerde.Core.Contracts;
using CircuitoTereVerde.Core.Domain;
using CircuitoTereVerde.Core.Services;

namespace CircuitoTereVerdeWebApi.Services;

// Coordena as operações administrativas e lê os bytes da foto recebida.
public sealed class AdminPostagensService
{
    private readonly PostagemService _postagemService;

    public AdminPostagensService(PostagemService postagemService)
    {
        _postagemService = postagemService;
    }

    #region Consultas administrativas

    public async Task<PaginaDto<PostagemDto>> ListarAsync(
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken = default)
    {
        return await _postagemService.ListarTodasAsync(pagina, tamanhoPagina, cancellationToken);
    }

    public async Task<PostagemDto> ObterAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _postagemService.ObterPorIdAsync(id, cancellationToken);
    }

    #endregion

    #region Cadastro e edição

    public async Task<PostagemDto> CriarAsync(
        SalvarPostagemDto dados,
        IFormFile? arquivoFoto,
        string usuarioId,
        CancellationToken cancellationToken = default)
    {
        FotoPostagemRegras.ValidarObrigatoria(arquivoFoto is not null);
        var foto = await PrepararFotoAsync(arquivoFoto!, cancellationToken);
        return await _postagemService.CriarAsync(
            dados, foto.Conteudo, foto.TipoConteudo, usuarioId, cancellationToken);
    }

    public async Task<PostagemDto> AtualizarAsync(
        int id,
        SalvarPostagemDto dados,
        IFormFile? arquivoFoto,
        CancellationToken cancellationToken = default)
    {
        byte[]? conteudoFoto = null;
        string? tipoConteudoFoto = null;
        if (arquivoFoto is not null)
        {
            var foto = await PrepararFotoAsync(arquivoFoto, cancellationToken);
            conteudoFoto = foto.Conteudo;
            tipoConteudoFoto = foto.TipoConteudo;
        }

        return await _postagemService.AtualizarAsync(
            id, dados, conteudoFoto, tipoConteudoFoto, cancellationToken);
    }

    public async Task ExcluirAsync(int id, CancellationToken cancellationToken = default)
    {
        await _postagemService.ExcluirAsync(id, cancellationToken);
    }

    #endregion

    #region Preparação da foto

    private static async Task<FotoPostagemDto> PrepararFotoAsync(
        IFormFile arquivoFoto,
        CancellationToken cancellationToken)
    {
        var tipoConteudo = FotoPostagemRegras.ValidarEObterTipo(
            arquivoFoto.Length, arquivoFoto.ContentType);

        await using var stream = new MemoryStream();
        await arquivoFoto.CopyToAsync(stream, cancellationToken);
        return new FotoPostagemDto(stream.ToArray(), tipoConteudo);
    }

    #endregion
}
