using Microsoft.Data.Sqlite;

namespace CircuitoTereVerde.Core.Infra.Connection;

public static class SqliteConnectionFactory
{
    public static string Normalizar(string connectionString, string contentRoot)
    {
        var configuracao = new SqliteConnectionStringBuilder(connectionString);
        if (configuracao.DataSource != ":memory:" && !Path.IsPathRooted(configuracao.DataSource))
        {
            configuracao.DataSource = Path.GetFullPath(Path.Combine(contentRoot, configuracao.DataSource));
        }

        var pastaBanco = Path.GetDirectoryName(configuracao.DataSource);
        if (!string.IsNullOrWhiteSpace(pastaBanco))
        {
            Directory.CreateDirectory(pastaBanco);
        }

        return configuracao.ToString();
    }
}
