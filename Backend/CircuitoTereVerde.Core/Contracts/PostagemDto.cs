namespace CircuitoTereVerde.Core.Contracts;

public sealed record PostagemDto(
    int Id,
    string Parque,
    string Tipo,
    string Titulo,
    string Descricao,
    string Foto,
    DateTimeOffset DataPublicacao,
    DateOnly DataInicio,
    DateOnly? DataFim,
    DateTimeOffset DataCriacao,
    DateTimeOffset? DataAtualizacao);

