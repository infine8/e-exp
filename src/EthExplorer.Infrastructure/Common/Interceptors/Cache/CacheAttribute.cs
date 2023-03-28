namespace EthExplorer.Infrastructure.Common.Interceptors.Cache
{
    public class CacheAttribute : BaseMethodInterceptionAttribute
    {
        public static readonly Dictionary<CachePeriod, TimeSpan> CACHE_PERIOD_DICT = new()
        {
            { CachePeriod.UltraShort, TimeSpan.FromMinutes(1) },
            { CachePeriod.Short, TimeSpan.FromMinutes(5) },
            { CachePeriod.Middle, TimeSpan.FromHours(1) },
            { CachePeriod.Long, TimeSpan.FromDays(1) },
            { CachePeriod.UltraLong, TimeSpan.FromDays(7) }
        };

        public TimeSpan TimeSpan { get; }

        public string[] ClearOnCallMethods { get; }

        public CacheAttribute(params string[] clearOnCallMethods)
        {
            ClearOnCallMethods = clearOnCallMethods;

            if (TimeSpan == default)
                TimeSpan = CACHE_PERIOD_DICT[CachePeriod.Short];
        }

        public CacheAttribute(CachePeriod period, params string[] clearOnCallMethods) : this(clearOnCallMethods)
        {
            TimeSpan = CACHE_PERIOD_DICT[period];
        }

        public CacheAttribute(double seconds, params string[] clearOnCallMethods) : this(clearOnCallMethods)
        {
            TimeSpan = TimeSpan.FromSeconds(seconds);
        }
    }
}
