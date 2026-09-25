using CircuitoTereVerde.Core.Infra.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CircuitoTereVerde.Core.Infra.UnitOfWork;

public sealed class UnitOfWork
{
    private readonly DbContext _context;
    private ParqueRepository? _parqueRepository;
    private TipoPostagemRepository? _tipoPostagemRepository;
    private PostagemRepository? _postagemRepository;
    private FotoPostagemRepository? _fotoPostagemRepository;
    private UsuarioRepository? _usuarioRepository;

    public UnitOfWork(DbContext context)
    {
        _context = context;
    }

    #region Repositórios sob demanda

    // Cada propriedade cria o repositório na primeira chamada e conserva a instância no UoW.
    public ParqueRepository ParqueRepository =>
        _parqueRepository ??= new ParqueRepository(_context);

    public TipoPostagemRepository TipoPostagemRepository =>
        _tipoPostagemRepository ??= new TipoPostagemRepository(_context);

    public PostagemRepository PostagemRepository =>
        _postagemRepository ??= new PostagemRepository(_context);

    public FotoPostagemRepository FotoPostagemRepository =>
        _fotoPostagemRepository ??= new FotoPostagemRepository(_context);

    public UsuarioRepository UsuarioRepository =>
        _usuarioRepository ??= new UsuarioRepository(_context);

    #endregion

    #region Gravação da unidade de trabalho

    public async Task<int> SalvarAlteracoesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    #endregion
}
