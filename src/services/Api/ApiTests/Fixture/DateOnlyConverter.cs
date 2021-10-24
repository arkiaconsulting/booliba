// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Booliba.ApiTests.Fixture
{
    public class DateOnlyConverter : JsonConverter<DateOnly>
    {
        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            DateOnly.Parse(reader.GetString()!);

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value.ToString());
    }
}
