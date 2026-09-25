namespace CircuitoTereVerde.Core.Domain;

public class ErroDominioException(string message) : Exception(message);

public sealed class RegistroNaoEncontradoException(string message) : ErroDominioException(message);
