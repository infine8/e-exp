using Newtonsoft.Json;

namespace EthExplorer.Domain.Common.Extensions;

public static class JsonExtensions
{
    public static string ToJson(this object obj) => ToJson(obj, false);

    public static string ToJson(this object obj, bool advanced) =>
        !advanced
            ? JsonConvert.SerializeObject(obj)
            : JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.All
            });
    
    public static T? DeserializeObject<T>(this string str) => DeserializeObject<T>(str, false);

    public static object DeserializeObject(this string str, Type type)
        => JsonConvert.DeserializeObject(str, type);

    public static T? DeserializeObject<T>(this string? str, bool advanced)
    {
        if (str.IsNullOrEmpty()) return default;
            
        return !advanced
            ? JsonConvert.DeserializeObject<T>(str)
            : JsonConvert.DeserializeObject<T>(str, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.All
            });
    }
}