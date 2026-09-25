namespace CircuitoTereVerde.Core.Domain;

// Mantém no domínio as regras de tamanho e formato aceitos para fotos.
public static class FotoPostagemRegras
{
    private const long TamanhoMaximo = 5 * 1024 * 1024;

    public static void ValidarObrigatoria(bool selecionada)
    {
        if (!selecionada)
        {
            throw new ErroDominioException("Selecione uma imagem para a postagem.");
        }
    }

    public static string ValidarEObterTipo(long tamanho, string tipoConteudo)
    {
        if (tamanho <= 0)
        {
            throw new ErroDominioException("A imagem não pode estar vazia.");
        }

        if (tamanho > TamanhoMaximo)
        {
            throw new ErroDominioException("A imagem deve ter até 5 MB.");
        }

        var tipoNormalizado = tipoConteudo.Trim().ToLowerInvariant();
        if (tipoNormalizado is not "image/jpeg" and not "image/png" and not "image/webp")
        {
            throw new ErroDominioException("Use uma imagem JPG, PNG ou WEBP.");
        }

        return tipoNormalizado;
    }
}
