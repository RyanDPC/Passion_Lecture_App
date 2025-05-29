using System;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace App.Helper
{
    public class BufferToByteArrayConverter : JsonConverter<byte[]>
    {
        public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                using var document = JsonDocument.ParseValue(ref reader);
                var root = document.RootElement;


                if (root.ValueKind == JsonValueKind.Object)
                {
                    if (root.TryGetProperty("type", out var typeElement) &&
                        typeElement.GetString() == "Buffer" &&
                        root.TryGetProperty("data", out var dataElement))
                    {
                        var result = dataElement.EnumerateArray()
                            .Select(x => (byte)x.GetInt32())
                            .ToArray();

                        return result;
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }
            writer.WriteStartObject();
            writer.WriteString("type", "Buffer");
            writer.WriteStartArray("data");
            foreach (byte b in value)
            {
                writer.WriteNumberValue(b);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }
}
