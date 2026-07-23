using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bloom.Common
{
    public class Json
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
}
