using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace App.Helper
{
    public class NodeBufferToByteArrayConverter : JsonConverter<byte[]>
    {
        public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (var doc = JsonDocument.ParseValue(ref reader))
            {
                if (doc.RootElement.TryGetProperty("data", out var dataElement) && dataElement.ValueKind == JsonValueKind.Array)
                {
                    var bytes = new byte[dataElement.GetArrayLength()];
                    int i = 0;
                    foreach (var item in dataElement.EnumerateArray())
                    {
                        bytes[i++] = item.GetByte();
                    }
                    return bytes;
                }
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}