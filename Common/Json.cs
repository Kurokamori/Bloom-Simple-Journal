using System.Text.Json;

namespace Bloom.Common;

public static class Json
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        PropertyNameCaseInsensitive = true
    };

    public static string Write<T>(T value) => JsonSerializer.Serialize(value, Options);

    public static T? Read<T>(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return default;
        }
        try
        {
            return JsonSerializer.Deserialize<T>(text, Options);
        }
        catch (JsonException)
        {
            return default;
        }
    }
}
