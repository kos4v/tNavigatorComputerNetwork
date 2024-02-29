using System.Text.Json;
// using System.Text.Json.Serialization.Metadata;

namespace tNavigatorModels
{
    public static class JsonUtil
    {
        private static JsonSerializerOptions JsonOptions { get; } = new()
        {
            // TypeInfoResolver = new DefaultJsonTypeInfoResolver()
        };
        
        public static T? Deserialize<T>(string data) => JsonSerializer.Deserialize<T>(data, JsonOptions);
        
        public static string Serialize<T>(T data) => JsonSerializer.Serialize(data, JsonOptions);
    }
}