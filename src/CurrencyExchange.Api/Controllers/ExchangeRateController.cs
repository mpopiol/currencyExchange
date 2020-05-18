using CurrencyExchange.Application.DTO;
using CurrencyExchange.Application.Services.Interfaces;
using CurrencyExchange.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchange.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExchangeRateController : ControllerBase
    {
        private readonly ILogger<ExchangeRateController> logger;
        private readonly ICurrencyRateApiService currencyApiService;

        public ExchangeRateController(ILogger<ExchangeRateController> logger,
            ICurrencyRateApiService currencyApiService)
        {
            this.logger = logger;
            this.currencyApiService = currencyApiService;
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = int.MaxValue)]
        public async Task<ActionResult<CurrencyRateDto[]>> GetQueryAsync(
            [FromQuery] Dictionary<string, string> currencyCodes,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] string apiKey)
        {
            var currencyRateQueryDto = new CurrencyRateQueryDto
            {
                CurrencyCodes = currencyCodes,
                StartDate = startDate,
                EndDate = endDate,
                ApiKey = apiKey
            };

            return await GetCurrencyRatesAsync(currencyRateQueryDto);
        }

        private async Task<ActionResult<CurrencyRateDto[]>> GetCurrencyRatesAsync(CurrencyRateQueryDto currencyRateQueryDto)
        {
            logger.LogInformation($"{currencyRateQueryDto.ApiKey} called for: " +
                $"{string.Join(", ", currencyRateQueryDto.CurrencyCodes.Select(currencies => $"{currencies.Value}/{currencies.Key}"))} " +
                $"in time range: {currencyRateQueryDto.StartDate} - {currencyRateQueryDto.EndDate}");

            try
            {
                var result = await currencyApiService.GetCurrencyRatesAsync(currencyRateQueryDto);

                return Ok(result);
            }
            catch (RequestException exception)
            {
                return NotFound(exception.Message);
            }
            catch (Exception exception)
            {
                logger.LogError(exception.Message, exception);
                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
            }
        }
    }
}