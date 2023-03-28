using System.Reflection;
using System.Security.Cryptography;
using EthExplorer.Domain.Common.Primitives;
using Newtonsoft.Json;

namespace EthExplorer.Domain.Common;

public static class EntityUuidExtension
{
    public static Guid GetUuid<T>(this T entity) where T : BaseEntity<T>
    {
        var keyProps = entity.GetKeyProps();
        
        return GetUuid(JsonConvert.SerializeObject(keyProps));
    }
    
    private static IDictionary<string, object?> GetKeyProps<T>(this T entity) where T : BaseEntity<T>
    {
        var ctorParams = typeof(T).GetConstructors().Single().GetParameters();

        var dict = new Dictionary<string, object?>();

        foreach (var prop in entity.GetType().GetProperties())
        {
            var param = ctorParams.FirstOrDefault(_ => _.Name == prop.Name);
            var keyAttribute = param?.GetCustomAttribute<EntityPartKeyAttribute>();

            if (keyAttribute is not null)
            {
                dict.Add(prop.Name, prop.GetValue(entity));
            }
        }

        return dict;
    }


    private static Guid GetUuid(string data)
    {
        using var hasher = MD5.Create();

        var bytes = hasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data));
        
        var hash = string.Join(string.Empty, bytes.Select(_ => _.ToString("X2")));

        return new Guid(hash);
    }
}