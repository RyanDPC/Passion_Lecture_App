using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace App.Helper
{
    public class ByteArray : JsonConverter<byte[]>
    {
        public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Cas 1 : chaîne Base64
            if (reader.TokenType == JsonTokenType.String)
            {
                string base64 = reader.GetString();
                try
                {
                    return Convert.FromBase64String(base64);
                }
                catch
                {
                    return Array.Empty<byte>();
                }
            }

            // Cas 2 : tableau d'octets
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                // Laissez JsonSerializer gérer la désérialisation de la liste d'octets
                List<byte> list = JsonSerializer.Deserialize<List<byte>>(ref reader, options);
                return list?.ToArray() ?? Array.Empty<byte>();
            }

            // Autre cas : on ignore proprement
            return Array.Empty<byte>();
        }

        public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
        {
            // Écrit toujours comme Base64
            writer.WriteStringValue(Convert.ToBase64String(value));
        }
    }
}
