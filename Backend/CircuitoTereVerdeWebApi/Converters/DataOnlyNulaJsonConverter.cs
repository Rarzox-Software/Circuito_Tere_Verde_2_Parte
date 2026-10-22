using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CircuitoTereVerdeWebApi.Converters;

// Aceita data vazia no contrato do frontend e a converte para ausência de data final.
public sealed class DataOnlyNulaJsonConverter : JsonConverter<DateOnly?>
{
    public override DateOnly? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var valor = reader.GetString();
            if (string.IsNullOrWhiteSpace(valor))
            {
                return null;
            }

            if (DateOnly.TryParse(valor, CultureInfo.InvariantCulture, DateTimeStyles.None, out var data))
            {
                return data;
            }
        }

        throw new JsonException("Informe uma data válida no formato yyyy-MM-dd.");
    }

    public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
    {
        if (!value.HasValue)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
    }
}
