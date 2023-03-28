using System.Reflection;
using System.Text;
using EthExplorer.Domain.Common.Extensions;
using EthExplorer.Infrastructure.Common.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Optional;
using StackExchange.Redis;

namespace EthExplorer.Infrastructure.Common.Interceptors.Cache
{
    public static class CacheExtensions
    {
        private static byte[] DataKey => Encoding.UTF8.GetBytes("data");

        public static T GetCacheObjValue<T>(this IMemoryCache cache, string name) where T : class, new()
        {
            var val = cache.Get<T>(name);

            if (val is not null) return val;
            
            val = new T();
            cache.Set(name, val);

            return val;
        }

        public static T? GetCacheValue<T>(this IMemoryCache cache, string name)
        {
            var val = cache.Get<T>(name);

            if (val is not null) return val;
            
            val = default;
            cache.Set(name, val);

            return val;
        }

        public static long GetApproximateSize(this IMemoryCache cache)
        {
            var statsField = typeof(IMemoryCache).GetField("_stats", BindingFlags.NonPublic | BindingFlags.Instance);
            var statsValue = statsField.GetValue(cache);
            var monitorField = statsValue.GetType().GetField("_cacheMemoryMonitor", BindingFlags.NonPublic | BindingFlags.Instance);
            var monitorValue = monitorField.GetValue(statsValue);
            var sizeField = monitorValue.GetType().GetField("_sizedRef", BindingFlags.NonPublic | BindingFlags.Instance);
            var sizeValue = sizeField.GetValue(monitorValue);
            var approxProp = sizeValue.GetType().GetProperty("ApproximateSize", BindingFlags.NonPublic | BindingFlags.Instance);
            return (long)(approxProp?.GetValue(sizeValue, null) ?? 0);
        }

        public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value)
        {
            await cache.SetAsync(key, value.ToByteArray(), new DistributedCacheEntryOptions());
        }

        public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value, TimeSpan? expiresIn)
        {
            await cache.SetAsync(key, value.ToByteArray(), new DistributedCacheEntryOptions { SlidingExpiration = expiresIn });
        }

        public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value, DateTimeOffset? expiresAt)
        {
            await cache.SetAsync(key, value.ToByteArray(), new DistributedCacheEntryOptions { AbsoluteExpiration = expiresAt });
        }

        public static void Set<T>(this IDistributedCache cache, string key, T value)
        {
            cache.Set(key, value.ToByteArray(), new DistributedCacheEntryOptions());
        }

        public static void Set<T>(this IDistributedCache cache, string key, T value, TimeSpan? expiresIn)
        {
            cache.Set(key, value.ToByteArray(), new DistributedCacheEntryOptions { SlidingExpiration = expiresIn });
        }

        public static void Set<T>(this IDistributedCache cache, string key, T value, DateTimeOffset? expiresAt)
        {
            cache.Set(key, value.ToByteArray(), new DistributedCacheEntryOptions { AbsoluteExpiration = expiresAt });
        }

        public static async Task<Option<T>> GetAsync<T>(this IDistributedCache cache, string key)
        {
            var bytes = await cache.GetAsync(key);

            return bytes is not null ? Option.Some(bytes.ToObject<T>()) : Option.None<T>();
        }

        public static async Task<Option<object>> GetAsync(this IDistributedCache cache, string key, Type type)
        {
            var bytes = await cache.GetAsync(key);

            return bytes is not null ? Option.Some(bytes.ToObject(type)) : Option.None<object>();
        }

        public static Option<T> Get<T>(this IDistributedCache cache, string key)
        {
            var bytes = cache.Get(key);

            return bytes is not null ? Option.Some(bytes.ToObject<T>()) : Option.None<T>();
        }

        public static Option<object> Get(this IDistributedCache cache, string key, Type type)
        {
            var bytes = cache.Get(key);

            return bytes is not null ? Option.Some(bytes.ToObject(type)) : Option.None<object>();
        }

        public static async Task<bool> ExistsAsync(this IDistributedCache cache, string key)
        {
            var bytes = await cache.GetAsync(key);

            return bytes is not null;
        }

        public static bool Exists(this IDistributedCache cache, string key)
        {
            var bytes = cache.Get(key);

            return bytes is not null;
        }

        public static void Set(this IDatabase cache, string key, IEnumerable<HashEntry> entries, TimeSpan? expiresIn = null, DateTimeOffset? expiresAt = null, bool waitToComplete = false)
        {
            var commandFlag = waitToComplete ? CommandFlags.None : CommandFlags.FireAndForget;

            cache.HashSet(key, entries.ToArray(), commandFlag);

            if (expiresIn.HasValue) cache.KeyExpire(key, expiresIn.Value, commandFlag);
            if (expiresAt.HasValue) cache.KeyExpire(key, expiresAt.Value.UtcDateTime, commandFlag);
        }

        public static void Set<T>(this IDatabase cache, string key, T value, bool waitToComplete = false)
        {
            cache.Set(key, new[] { new HashEntry(DataKey, value.ToByteArray()) }, null, null, waitToComplete);
        }

        public static void Set<T>(this IDatabase cache, string key, T value, TimeSpan? expiresIn, bool waitToComplete = false)
        {
            cache.Set(key, new[] { new HashEntry(DataKey, value.ToByteArray()) }, expiresIn, null, waitToComplete);
        }

        public static void Set<T>(this IDatabase cache, string key, T value, DateTimeOffset? expiresAt, bool waitToComplete = false)
        {
            var bytes = value.ToByteArray();
            
            cache.Set(key, new[] { new HashEntry(DataKey, bytes) }, null, expiresAt?.UtcDateTime, waitToComplete);
        }

        public static Option<T> Get<T>(this IDatabase cache, string key)
        {
            var bytes = (byte[])cache.HashGet(key, DataKey);

            return bytes is not null ? Option.Some(bytes.ToObject<T>()) : Option.None<T>();
        }

        public static Option<object> Get(this IDatabase cache, string key, Type type)
        {
            var bytes = (byte[])cache.HashGet(key, DataKey);

            return bytes is not null ? Option.Some(bytes.ToObject(type)) : Option.None<object>();
        }

        public static bool DoesExist(this IDatabase cache, string key)
        {
            return cache.KeyExists(key);
        }

        public static void DeleteKey(this IDatabase cache, string key)
        {
            cache.KeyDelete(key, CommandFlags.FireAndForget);
        }

        public static void DeleteKey(this IDatabase cache, string key, object entryKey)
        {
            cache.HashDelete(key, entryKey.ToJson(), CommandFlags.FireAndForget);
        }

        public static void SetDict<TKey, TVal>(this IDatabase cache, string key, IDictionary<TKey, TVal> dict, bool waitToComplete = false)
        {
            var fields = dict.Select(_ => new HashEntry(_.Key.ToJson(), _.Value.ToJson().Zip())).ToArray();

            cache.HashSet(key, fields, waitToComplete ? CommandFlags.None : CommandFlags.FireAndForget);
        }

        public static Option<T> GetDictEntry<T>(this IDatabase cache, string key, object entryKey)
        {
            var bytes = cache.HashGet(key, entryKey.ToJson());
            if (!bytes.HasValue) return Option.None<T>();

            var val = ((byte[])bytes).Unzip().DeserializeObject<T>();

            return Option.Some(val);
        }

        public static IDictionary<TKey, TVal> GetDict<TKey, TVal>(this IDatabase cache, string key)
        {
            var entries = cache.HashGetAll(key);
            if (entries == null) return new Dictionary<TKey, TVal>();

            return entries.ToDictionary(_ => _.Name.ToString().DeserializeObject<TKey>(), _ => ((byte[])_.Value).Unzip().DeserializeObject<TVal>());
        }

        public static IDictionary<string, string> GetPlainDictionary(this IDatabase cache, string key)
        {
            var entries = cache.HashGetAll(key);
            if (entries is null) 
                return new Dictionary<string, string>();
            
            return entries.ToDictionary(_ => _.Name.ToString(), _ => ((byte[])_.Value).Unzip());
        }
        
        public static string GetPlainValue(this IDatabase cache, string key, string entryKey)
        {
            var bytes = cache.HashGet(key, entryKey);
            if (!bytes.HasValue) return null;

            var val = ((byte[])bytes).Unzip();
            return val;
        }
    }
}
