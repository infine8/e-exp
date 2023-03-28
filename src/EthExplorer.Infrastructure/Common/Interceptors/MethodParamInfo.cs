namespace EthExplorer.Infrastructure.Common.Interceptors
{
    public record MethodParamInfo
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public object Value { get; set; }
    }
}
