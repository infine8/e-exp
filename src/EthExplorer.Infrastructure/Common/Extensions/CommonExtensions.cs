using System.Reflection;
using EthExplorer.Domain.Common.Extensions;
using EthExplorer.Infrastructure.Common.Interceptors;

namespace EthExplorer.Infrastructure.Common.Extensions;

public static class CommonExtensions
{
    public static MethodParamInfo? GetParamInfo(this MethodInfo? method, int index)
    {
        if (method is null || method.GetParameters().Length <= index) return null;

        var param = method.GetParameters()[index];
        return new MethodParamInfo { Name = param.Name, Type = param.ParameterType };
    }
    
    public static byte[] ToByteArray(this object obj)
    {
        return obj.ToJson().Zip();
    }

    public static T? ToObject<T>(this byte[] bytes)
    {
        var json = bytes.Unzip();
        
        return json.DeserializeObject<T>();
    }

    public static object ToObject(this byte[] bytes, Type type, bool isCompressed = false)
    {
        return bytes.Unzip().DeserializeObject(type);
    }
}