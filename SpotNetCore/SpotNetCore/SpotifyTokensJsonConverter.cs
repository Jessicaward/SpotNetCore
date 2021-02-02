using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.DataProtection;
using SpotNetCore.Models;

namespace SpotNetCore
{
    public class SpotifyTokensJsonConverter : JsonConverter<SpotifyAccessToken>
    {
        private IDataProtector _dataProtector;

        public SpotifyTokensJsonConverter(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtector = dataProtectionProvider.CreateProtector("SpotifyTokens");
        }

        public override SpotifyAccessToken? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var json = reader.GetString();
            return JsonSerializer.Deserialize<SpotifyAccessToken>(_dataProtector.Unprotect(json));
        }

        public override void Write(Utf8JsonWriter writer, SpotifyAccessToken value, JsonSerializerOptions options)
        {
            var json = JsonSerializer.Serialize(value);
            writer.WriteStringValue(_dataProtector.Protect(json));
        }
    }
}