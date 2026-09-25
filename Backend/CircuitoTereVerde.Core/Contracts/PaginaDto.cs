namespace CircuitoTereVerde.Core.Contracts;

public sealed record PaginaDto<T>(
    IReadOnlyList<T> Itens,
    int Pagina,
    int TamanhoPagina,
    int TotalItens,
    int TotalPaginas);

