using CurrencyExchange.Application.Enums;

namespace CurrencyExchange.Application.Configuration
{
    public interface IGeneralConfiguration
    {
        public ApiType ApiType { get; }
        public int DaysGoingBackForMissingData { get; }
    }
}