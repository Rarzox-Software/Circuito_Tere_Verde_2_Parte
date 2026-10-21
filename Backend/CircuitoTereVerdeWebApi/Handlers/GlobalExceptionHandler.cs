using CircuitoTereVerde.Core.Domain;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CircuitoTereVerdeWebApi.Handlers;

// Centraliza a conversão de exceções da aplicação em respostas JSON para o frontend.
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    #region Tratamento das exceções

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var status = StatusCodes.Status500InternalServerError;
        var titulo = "Ocorreu um erro ao processar a solicitação.";
        string? detalhe = null;

        // Informa que o recurso solicitado não existe.
        if (exception is RegistroNaoEncontradoException)
        {
            status = StatusCodes.Status404NotFound;
            titulo = exception.Message;
            detalhe = exception.Message;
        }
        // Informa que os dados não seguem uma regra do domínio.
        else if (exception is ErroDominioException)
        {
            status = StatusCodes.Status400BadRequest;
            titulo = exception.Message;
            detalhe = exception.Message;
        }

        var problema = new ProblemDetails
        {
            Status = status,
            Title = titulo,
            Detail = detalhe
        };

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(problema, cancellationToken: cancellationToken);

        return true;
    }

    #endregion
}
