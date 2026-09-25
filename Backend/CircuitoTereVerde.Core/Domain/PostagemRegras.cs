namespace CircuitoTereVerde.Core.Domain;

public static class PostagemRegras
{
    public static void ValidarDados(string titulo, string descricao, DateOnly? inicio, DateOnly? fim)
    {
        if (string.IsNullOrWhiteSpace(titulo) || titulo.Trim().Length > 160)
        {
            throw new ErroDominioException("Informe um título com até 160 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(descricao) || descricao.Trim().Length > 5000)
        {
            throw new ErroDominioException("Informe uma descrição com até 5000 caracteres.");
        }

        if (!inicio.HasValue)
        {
            throw new ErroDominioException("Informe a data inicial de exibição.");
        }

        if (fim.HasValue && fim.Value < inicio.Value)
        {
            throw new ErroDominioException("A data final deve ser igual ou posterior à data inicial.");
        }
    }
}
