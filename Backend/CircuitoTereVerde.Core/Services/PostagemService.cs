using CircuitoTereVerde.Core.Contracts;
using CircuitoTereVerde.Core.Domain;
using CircuitoTereVerde.Core.Models;
using System.Globalization;
using System.Text;
using UnitOfWork = CircuitoTereVerde.Core.Infra.UnitOfWork.UnitOfWork;

namespace CircuitoTereVerde.Core.Services;

public sealed class PostagemService
{
    private const int TamanhoMaximoPagina = 50;
    private readonly UnitOfWork _uow;

    public PostagemService(UnitOfWork uow)
    {
        _uow = uow;
    }

    #region Consultas públicas

    // A principal mostra somente postagens dentro do período de exibição.
    public async Task<PaginaDto<PostagemDto>> ListarPublicasAsync(
        string? parque,
        string? tipo,
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken = default)
    {
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var repositorio = _uow.PostagemRepository;
        var consulta = repositorio.Consultar()
            .Where(postagem => postagem.DataInicio <= hoje
                && (postagem.DataFim == null || postagem.DataFim >= hoje));

        if (!string.IsNullOrWhiteSpace(parque))
        {
            var codigoParque = await ResolverCodigoParqueAsync(parque, cancellationToken);
            consulta = consulta.Where(postagem => postagem.Parque.Codigo == codigoParque);
        }

        if (!string.IsNullOrWhiteSpace(tipo))
        {
            var codigoTipo = await ResolverCodigoTipoAsync(tipo, cancellationToken);
            consulta = consulta.Where(postagem => postagem.TipoPostagem.Codigo == codigoTipo);
        }
        else
        {
            consulta = consulta.Where(postagem => postagem.TipoPostagem.Codigo != CodigosCatalogo.Tipos.Sobre);
        }

        consulta = consulta.OrderByDescending(postagem => postagem.DataInicio).ThenByDescending(postagem => postagem.Id);
        return await CriarPaginaAsync(consulta, pagina, tamanhoPagina, cancellationToken);
    }

    public async Task<IReadOnlyList<PostagemDto>> ListarDestaquesAsync(CancellationToken cancellationToken = default)
    {
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var repositorio = _uow.PostagemRepository;
        var consulta = repositorio.Consultar()
            .Where(postagem => postagem.DataInicio <= hoje
                && (postagem.DataFim == null || postagem.DataFim >= hoje)
                && postagem.TipoPostagem.Codigo != CodigosCatalogo.Tipos.Sobre)
            .OrderByDescending(postagem => postagem.DataInicio)
            .ThenByDescending(postagem => postagem.Id)
            .Take(5);
        var postagens = await repositorio.ListarAsync(consulta, cancellationToken);
        return postagens.Select(Mapear).ToList();
    }

    public async Task<PostagemDto> ObterPublicaAsync(int id, CancellationToken cancellationToken = default)
    {
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var repositorio = _uow.PostagemRepository;
        var postagem = await repositorio.PrimeiroOuPadraoAsync(
            repositorio.Consultar()
                .Where(item => item.Id == id
                    && item.DataInicio <= hoje
                    && (item.DataFim == null || item.DataFim >= hoje)),
            cancellationToken);

        if (postagem is null)
        {
            throw new RegistroNaoEncontradoException("Postagem não encontrada.");
        }

        return Mapear(postagem);
    }

    #endregion

    #region Consultas e operações administrativas

    // A consulta administrativa inclui postagens futuras e encerradas.
    public async Task<PaginaDto<PostagemDto>> ListarTodasAsync(
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken = default)
    {
        var repositorio = _uow.PostagemRepository;
        var consulta = repositorio.Consultar()
            .OrderByDescending(postagem => postagem.DataInicio)
            .ThenByDescending(postagem => postagem.Id);
        return await CriarPaginaAsync(consulta, pagina, tamanhoPagina, cancellationToken);
    }

    public async Task<PostagemDto> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var repositorio = _uow.PostagemRepository;
        var postagem = await repositorio.PrimeiroOuPadraoAsync(
            repositorio.Consultar().Where(item => item.Id == id), cancellationToken);

        if (postagem is null)
        {
            throw new RegistroNaoEncontradoException("Postagem não encontrada.");
        }

        return Mapear(postagem);
    }

    public async Task<PostagemDto> CriarAsync(
        SalvarPostagemDto dados,
        byte[] conteudoFoto,
        string tipoConteudoFoto,
        string usuarioId,
        CancellationToken cancellationToken = default)
    {
        PostagemRegras.ValidarDados(dados.Titulo, dados.Descricao, dados.DataInicio, dados.DataFim);
        var (parque, tipo) = await ObterCatalogoAsync(dados.Parque, dados.Tipo, cancellationToken);
        var agora = DateTimeOffset.UtcNow;
        var postagem = new Postagem
        {
            ParqueId = parque.Id,
            TipoPostagemId = tipo.Id,
            FotoPostagem = new FotoPostagem
            {
                Conteudo = conteudoFoto,
                TipoConteudo = tipoConteudoFoto
            },
            Titulo = dados.Titulo.Trim(),
            Descricao = dados.Descricao.Trim(),
            DataPublicacao = agora,
            DataInicio = dados.DataInicio!.Value,
            DataFim = dados.DataFim,
            DataCriacao = agora,
            CriadoPorId = usuarioId
        };

        var repositorio = _uow.PostagemRepository;
        await repositorio.AdicionarAsync(postagem, cancellationToken);
        await _uow.SalvarAlteracoesAsync(cancellationToken);

        var salva = await repositorio.PrimeiroOuPadraoAsync(
            repositorio.Consultar().Where(item => item.Id == postagem.Id), cancellationToken);
        return Mapear(salva!);
    }

    public async Task<PostagemDto> AtualizarAsync(
        int id,
        SalvarPostagemDto dados,
        byte[]? novoConteudoFoto,
        string? novoTipoConteudoFoto,
        CancellationToken cancellationToken = default)
    {
        PostagemRegras.ValidarDados(dados.Titulo, dados.Descricao, dados.DataInicio, dados.DataFim);
        var repositorio = _uow.PostagemRepository;
        var postagem = await repositorio.PrimeiroOuPadraoAsync(
            repositorio.Consultar(rastrearAlteracoes: true).Where(item => item.Id == id), cancellationToken);

        if (postagem is null)
        {
            throw new RegistroNaoEncontradoException("Postagem não encontrada.");
        }

        var (parque, tipo) = await ObterCatalogoAsync(dados.Parque, dados.Tipo, cancellationToken);
        postagem.ParqueId = parque.Id;
        postagem.TipoPostagemId = tipo.Id;
        postagem.Titulo = dados.Titulo.Trim();
        postagem.Descricao = dados.Descricao.Trim();
        postagem.DataInicio = dados.DataInicio!.Value;
        postagem.DataFim = dados.DataFim;
        postagem.DataAtualizacao = DateTimeOffset.UtcNow;

        if (novoConteudoFoto is not null)
        {
            var repositorioFoto = _uow.FotoPostagemRepository;
            var fotoAtual = await repositorioFoto.PrimeiroOuPadraoAsync(
                repositorioFoto.Consultar(rastrearAlteracoes: true)
                    .Where(item => item.Id == postagem.FotoPostagemId), cancellationToken);

            if (fotoAtual is null)
            {
                throw new RegistroNaoEncontradoException("Imagem da postagem não encontrada.");
            }

            fotoAtual.Conteudo = novoConteudoFoto;
            fotoAtual.TipoConteudo = novoTipoConteudoFoto ?? "image/jpeg";
        }

        await _uow.SalvarAlteracoesAsync(cancellationToken);
        var atualizada = await repositorio.PrimeiroOuPadraoAsync(
            repositorio.Consultar().Where(item => item.Id == postagem.Id), cancellationToken);
        return Mapear(atualizada!);
    }

    public async Task ExcluirAsync(int id, CancellationToken cancellationToken = default)
    {
        var repositorio = _uow.PostagemRepository;
        var postagem = await repositorio.PrimeiroOuPadraoAsync(
            repositorio.Consultar(rastrearAlteracoes: true).Where(item => item.Id == id), cancellationToken);

        if (postagem is null)
        {
            throw new RegistroNaoEncontradoException("Postagem não encontrada.");
        }

        repositorio.Excluir(postagem);
        // Remove a foto vinculada no mesmo salvamento da exclusão da postagem.
        _uow.FotoPostagemRepository.Excluir(postagem.FotoPostagem);
        await _uow.SalvarAlteracoesAsync(cancellationToken);
    }

    #endregion

    #region Validação, paginação e mapeamento

    private async Task<PaginaDto<PostagemDto>> CriarPaginaAsync(
        IQueryable<Postagem> consulta,
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken)
    {
        var resultado = await _uow.PostagemRepository
            .PaginarAsync(consulta, pagina, tamanhoPagina, cancellationToken);
        pagina = Math.Max(1, pagina);
        tamanhoPagina = Math.Clamp(tamanhoPagina, 1, TamanhoMaximoPagina);
        var totalPaginas = resultado.Total == 0 ? 0 : (int)Math.Ceiling(resultado.Total / (double)tamanhoPagina);

        return new PaginaDto<PostagemDto>(
            resultado.Itens.Select(Mapear).ToList(), pagina, tamanhoPagina, resultado.Total, totalPaginas);
    }

    private async Task<string> ResolverCodigoParqueAsync(string valor, CancellationToken cancellationToken)
    {
        var repositorio = _uow.ParqueRepository;
        var parques = await repositorio.ListarAsync(repositorio.Consultar(), cancellationToken);

        return parques.FirstOrDefault(parque =>
                   TextosEquivalentes(parque.Codigo, valor) ||
                   TextosEquivalentes(parque.Nome, valor))?.Codigo
               ?? valor.Trim();
    }

    private async Task<string> ResolverCodigoTipoAsync(string valor, CancellationToken cancellationToken)
    {
        var repositorio = _uow.TipoPostagemRepository;
        var tipos = await repositorio.ListarAsync(repositorio.Consultar(), cancellationToken);

        return tipos.FirstOrDefault(tipo =>
                   TextosEquivalentes(tipo.Codigo, valor) ||
                   TextosEquivalentes(tipo.Nome, valor))?.Codigo
               ?? valor.Trim();
    }

    private static bool TextosEquivalentes(string primeiroValor, string segundoValor) =>
        string.Equals(
            NormalizarTexto(primeiroValor),
            NormalizarTexto(segundoValor),
            StringComparison.Ordinal);

    private static string NormalizarTexto(string valor)
    {
        var textoDecomposto = valor.Trim().Normalize(NormalizationForm.FormD);
        var caracteresSemAcento = textoDecomposto.Where(caractere =>
            CharUnicodeInfo.GetUnicodeCategory(caractere) != UnicodeCategory.NonSpacingMark);

        return new string(caracteresSemAcento.ToArray()).ToUpperInvariant();
    }

    private async Task<(Parque Parque, TipoPostagem Tipo)> ObterCatalogoAsync(
        string codigoParque,
        string codigoTipo,
        CancellationToken cancellationToken)
    {
        var codigoParqueResolvido = await ResolverCodigoParqueAsync(codigoParque, cancellationToken);
        var codigoTipoResolvido = await ResolverCodigoTipoAsync(codigoTipo, cancellationToken);
        var repositorioParque = _uow.ParqueRepository;
        var parque = await repositorioParque.PrimeiroOuPadraoAsync(
            repositorioParque.Consultar()
                .Where(item => item.Codigo == codigoParqueResolvido), cancellationToken);
        var repositorioTipo = _uow.TipoPostagemRepository;
        var tipo = await repositorioTipo.PrimeiroOuPadraoAsync(
            repositorioTipo.Consultar()
                .Where(item => item.Codigo == codigoTipoResolvido), cancellationToken);

        if (parque is null)
        {
            throw new ErroDominioException("Selecione um parque válido.");
        }

        if (tipo is null)
        {
            throw new ErroDominioException("Selecione um tipo de postagem válido.");
        }

        return (parque, tipo);
    }

    private static PostagemDto Mapear(Postagem postagem)
    {
        return new PostagemDto(
            postagem.Id,
            postagem.Parque.Nome,
            postagem.TipoPostagem.Codigo,
            postagem.Titulo,
            postagem.Descricao,
            postagem.FotoPostagem is null
                ? string.Empty
                : $"data:{postagem.FotoPostagem.TipoConteudo};base64,{Convert.ToBase64String(postagem.FotoPostagem.Conteudo)}",
            postagem.DataPublicacao,
            postagem.DataInicio,
            postagem.DataFim,
            postagem.DataCriacao,
            postagem.DataAtualizacao);
    }

    #endregion
}


