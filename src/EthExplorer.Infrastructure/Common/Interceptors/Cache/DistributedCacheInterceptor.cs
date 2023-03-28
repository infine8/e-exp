using Castle.DynamicProxy;
using EthExplorer.Domain.Common.Extensions;
using EthExplorer.Infrastructure.Common.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Optional;
using Optional.Unsafe;
using StackExchange.Redis;

namespace EthExplorer.Infrastructure.Common.Interceptors.Cache
{
    public class DistributedCacheInterceptor : BaseInterceptor<CacheAttribute>
    {
        private static readonly string CACHE_KEY_PREFIX = "CACHE_INTERCEPTOR_";

        private static readonly TimeSpan MEMORY_CACHE_TIMEOUT = TimeSpan.FromSeconds(1);

        protected bool BypassCache { get; set; }

        protected string? InvokerId { get; set; }

        private IDatabase DistributedCache { get; }
        private IMemoryCache MemoryCache { get; }

        private string CachedItemsInfoDictKey => $"{CACHE_KEY_PREFIX}CACHED_ITEMS";

        public DistributedCacheInterceptor(IDatabase distributedCache, IMemoryCache memoryCache)
        {
            DistributedCache = distributedCache;
            MemoryCache = memoryCache;
        }

        protected override void OnInterceptSync(IInvocation invocation)
        {
            var cachedVal = GetCacheValue(invocation);

            if (!BypassCache && cachedVal.HasValue)
            {
                invocation.ReturnValue = cachedVal.ValueOrFailure();
                return;
            }

            invocation.Proceed();

            SetCacheValue(invocation, invocation.ReturnValue);
        }

        protected override void OnInterceptAsync<TResult>(IInvocation invocation)
        {
            var cachedVal = GetCacheValue<TResult>(invocation);

            if (!BypassCache && cachedVal.HasValue)
            {
                invocation.ReturnValue = Task.FromResult(cachedVal.ValueOrFailure());
                return;
            }

            base.OnInterceptAsync<TResult>(invocation);
        }

        protected override Task OnAfterInternalInterceptAsync<TResult>(IInvocation invocation, TResult result)
        {
            if (result == null) return Task.CompletedTask;

            SetCacheValue(invocation, result);
            
            return Task.CompletedTask;
        }

        protected override Task OnAfterIntercept(IInvocation invocation)
        {
            RemoveConditionalCacheValue(invocation);
            
            return Task.CompletedTask;
        }

        private void SetCacheValue(IInvocation invocation, object val)
        {
            var invInfo = GetInvocationInfo(invocation);

            DistributedCache.Set(invInfo.Key, val, invInfo.ExpiresAt);
            MemoryCache.Set(invInfo.Key, val, MEMORY_CACHE_TIMEOUT);

            if (invInfo.ClearOnCallMethods.HasItems())
            {
                DistributedCache.HashSet(CachedItemsInfoDictKey, new[] { new HashEntry(invInfo.Key, invInfo.ToJson()) }, CommandFlags.FireAndForget);
            }
        }

        public Option<T> GetCacheValue<T>(IInvocation invocation)
        {
            var key = GetCacheKey(invocation);

            if (MemoryCache.TryGetValue(key, out var obj)) return Option.Some((T) obj);

            var val = DistributedCache.Get<T>(key);
            
            if (val.HasValue)
                MemoryCache.Set(key, val.ValueOrFailure(), MEMORY_CACHE_TIMEOUT);

            return val;
        }

        public Option<object> GetCacheValue(IInvocation invocation)
        {
            var key = GetCacheKey(invocation);
            
            if (MemoryCache.TryGetValue(key, out var obj)) return Option.Some(obj);

            var val = DistributedCache.Get(key, invocation.Method.ReturnType); 

            if (val.HasValue)
                MemoryCache.Set(key, val.ValueOrFailure(), MEMORY_CACHE_TIMEOUT);

            return val;
        }

        private DateTimeOffset GetExpiration(IInvocation invocation)
        {
            return DateTimeOffset.UtcNow.Add(GetAttribute(invocation)?.TimeSpan ?? default);
        }

        private string GetCacheKey(IInvocation invocation)
        {
            var methodName = GetInvocationMethodFullName(invocation);
            var arguments = invocation.Arguments.ToJson();

            InvokerId ??= string.Empty;

            return $"{CACHE_KEY_PREFIX}{string.Join("_", InvokerId, methodName, arguments).EncryptMd5()}";
        }

        private void RemoveConditionalCacheValue(IInvocation invocation)
        {
            var methodName = GetInvocationMethodFullName(invocation);

            var cachedItems = GetCachedItemsInfoDict();

            var items2Clear = cachedItems.Values.Where(_ => _.ClearOnCallMethods.Contains(methodName)).ToList();

            cachedItems.Values.Where(_ => _.ExpiresAt < DateTimeOffset.UtcNow).Select(_ => _.Key).ToList()
                .ForEach(key => DistributedCache.HashDelete(CachedItemsInfoDictKey, key, CommandFlags.FireAndForget));

            if (!items2Clear.Any()) return;

            var condInv = GetInvocationInfo(invocation);

            foreach (var invInfo in items2Clear)
            {
                var isSatisfied = true;

                invInfo.Arguments.Keys.Intersect(condInv.Arguments.Keys).ForEach(commonParamName =>
                {
                    isSatisfied = isSatisfied && (invInfo.Arguments[commonParamName] == condInv.Arguments[commonParamName]);
                });

                if (!isSatisfied) continue;

                DistributedCache.DeleteKey(invInfo.Key);
                MemoryCache.Remove(invInfo.Key);
                DistributedCache.HashDelete(CachedItemsInfoDictKey, invInfo.Key, CommandFlags.FireAndForget);
            }
        }

        private InvocationInfo GetInvocationInfo(IInvocation invocation)
        {
            var declaringType = invocation.Method.DeclaringType;

            var inv = new InvocationInfo
            {
                Key = GetCacheKey(invocation),
                ExpiresAt = GetExpiration(invocation),
                ClearOnCallMethods = GetAttribute(invocation)?.ClearOnCallMethods?.ToList() ?? new List<string>()
            };

            for (var i = 0; i < inv.ClearOnCallMethods.Count; i++)
            {
                if (declaringType == null) continue;

                var methodName = inv.ClearOnCallMethods[i];
                if (methodName.Contains(".")) continue;

                inv.ClearOnCallMethods[i] = $"{declaringType.FullName}.{methodName}";
            }

            foreach (var arg in GetArguments(invocation))
            {
                inv.Arguments.Add(arg.Name, arg.Value.ToJson());
            }

            return inv;
        }

        private Dictionary<string, InvocationInfo> GetCachedItemsInfoDict()
        {
            if (MemoryCache.TryGetValue(CachedItemsInfoDictKey, out var obj)) return (Dictionary<string, InvocationInfo>)obj;

            var val = DistributedCache.HashGetAll(CachedItemsInfoDictKey);
            if (val is null) return new Dictionary<string, InvocationInfo>();

            var dict = val.ToStringDictionary().ToDictionary(_ => _.Key, _ => _.Value.DeserializeObject<InvocationInfo>());

            MemoryCache.Set(CachedItemsInfoDictKey, dict, MEMORY_CACHE_TIMEOUT);

            return dict;
        }

    }

}
