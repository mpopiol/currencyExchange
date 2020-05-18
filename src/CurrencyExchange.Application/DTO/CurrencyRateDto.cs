using System;

namespace CurrencyExchange.Application.DTO
{
    public class CurrencyRateDto
    {
        public string SourceCurrency { get; set; }
        public string TargetCurrency { get; set; }
        public decimal ExchangeRate { get; set; }
        public DateTime Date { get; set; }
    }
}