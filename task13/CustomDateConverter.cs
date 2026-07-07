using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13
{
    public class CustomDateConverter : JsonConverter<DateTime>
    {
        private const string Format = "yyyy-MM-dd";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? dateStr = reader.GetString();
            if (string.IsNullOrEmpty(dateStr))
            {
                throw new JsonException("Значение даты не может быть пустым или отсутствовать.");
            }
            return DateTime.ParseExact(dateStr, Format, null);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format));
        }
    }
}