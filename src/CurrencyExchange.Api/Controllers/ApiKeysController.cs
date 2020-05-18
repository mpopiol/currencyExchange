using CurrencyExchange.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CurrencyExchange.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApiKeysController : ControllerBase
    {
        private readonly ILogger<ApiKeysController> logger;
        private readonly IApiKeyService apiKeyService;

        public ApiKeysController(ILogger<ApiKeysController> logger,
            IApiKeyService apiKeyService)
        {
            this.logger = logger;
            this.apiKeyService = apiKeyService;
        }

        // GET: ApiKeys
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<string>> GetAsync()
        {
            logger.LogInformation($"Generating new apki key");

            try
            {
                var id = await apiKeyService.GetNewApiKeyAsync();

                logger.LogInformation($"Generated new apki key {id}");

                return Ok(id);
            }
            catch (Exception exception)
            {
                logger.LogError(exception.Message, exception);
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
        }
    }
}