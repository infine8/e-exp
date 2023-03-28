namespace EthExplorer.Infrastructure.Common.Interceptors.Cache
{
    public record InvocationInfo
    {
        public string Key { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
        public Dictionary<string, string> Arguments { get; set; } = new();
        public List<string> ClearOnCallMethods { get; set; } = new();
    }
}
