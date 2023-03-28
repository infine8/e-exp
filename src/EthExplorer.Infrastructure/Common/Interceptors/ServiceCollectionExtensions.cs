using Castle.DynamicProxy;

namespace EthExplorer.Infrastructure.Common.Interceptors
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInterceptors(this IServiceCollection services, params Type[] interceptors)
        {
            services.AddSingleton<IProxyGenerator, ProxyGenerator>();

            services.AddSingleton<IAsyncInterceptor, LeakingThisInterceptor>();

            foreach (var interceptorType in interceptors)
            {
                services.AddScoped(typeof(IAsyncInterceptor), interceptorType);
            }
        }

        public static T CreateClassProxy<T>(this IServiceProvider sp, params object[] constructorArgs) where T : class
        {
            var args = new List<object> { sp };
            args.AddRange(constructorArgs);

            return sp.GetService<IProxyGenerator>().CreateClassProxy(typeof(T), args.ToArray(), sp.GetServices<IAsyncInterceptor>().ToArray()) as T;
        }

        public static IServiceCollection AddProxySingleton<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService
        {
            return services.AddSingleton<TService>(sp => sp.CreateClassProxy<TImplementation>());
        }

        public static IServiceCollection AddProxySingleton<TService>(this IServiceCollection services) where TService : class
        {
            return services.AddSingleton(sp => sp.CreateClassProxy<TService>());
        }

        public static IServiceCollection AddProxyScoped<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService
        {
            return services.AddScoped<TService>(sp => sp.CreateClassProxy<TImplementation>());
        }

        public static IServiceCollection AddProxyScoped<TService>(this IServiceCollection services) where TService : class
        {
            return services.AddScoped(sp => sp.CreateClassProxy<TService>());
        }

        public static IServiceCollection AddProxyTransient<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService
        {
            return services.AddTransient<TService>(sp => sp.CreateClassProxy<TImplementation>());
        }

        public static IServiceCollection AddProxyTransient<TService>(this IServiceCollection services) where TService : class
        {
            return services.AddTransient(sp => sp.CreateClassProxy<TService>());
        }
    }
}
