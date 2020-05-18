namespace CurrencyExchange.Application.Configuration
{
    public interface ISdmxApiConfiguration
    {
        public string UriTemplate { get; }
        public string SourceCurrencyKey { get; }
        public string TargetCurrencyKey { get; }
    }
}