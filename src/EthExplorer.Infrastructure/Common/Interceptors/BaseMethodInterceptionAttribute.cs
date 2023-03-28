using System.Diagnostics.CodeAnalysis;

namespace EthExplorer.Infrastructure.Common.Interceptors
{
    /// <summary>
    /// Method Interception Attribute (inherit from this to create your interception attributes)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method), ExcludeFromCodeCoverage]
    public abstract class BaseMethodInterceptionAttribute : Attribute
    {
    }
}
