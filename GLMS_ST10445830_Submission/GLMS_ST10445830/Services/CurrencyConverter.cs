using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


// deals with the currency conversion for ZAR TO USD by callling an api
namespace GLMS_ST10445830.Services
{
    public class CurrencyConverter : ICurrencyConverter
    {
        /*creates the http client factory to call the api
        *logger is used for debug purposes checking if the api was being called
        *
        *Dependancy injection , dependacnies come from outside
        */
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CurrencyConverter> _logger;
        private readonly string _apiKey; //stores the api key
        private readonly bool _useTestMode;
        private readonly ICurrencyConverter _fallback;

        //IConfiguration gives access to api key from appsettings
        //this retrives the api key
        public CurrencyConverter(IHttpClientFactory httpClientFactory, ILogger<CurrencyConverter> logger, IConfiguration configuration, ICurrencyConverter fallback)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _apiKey = configuration["ExchangeRateApi:ApiKey"] ?? "";
            _fallback = fallback;

            if (string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogWarning("ExchangeRate API key is missing. Using test mode.");
                _useTestMode = true;
            }
        }
        //contains fallback if the api fails,does the conversion usin g the api
        public async Task<decimal> ConvertUsdToZar(decimal usdAmount)
        {
            if (usdAmount < 0)
                throw new ArgumentException("Amount cannot be negative.", nameof(usdAmount));

            if (_useTestMode)
                return await _fallback.ConvertUsdToZar(usdAmount);

            var client = _httpClientFactory.CreateClient();
            string url = $"https://v6.exchangerate-api.com/v6/{_apiKey}/pair/USD/ZAR";

            try
            {
                var response = await client.GetAsync(url);
                string json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return await _fallback.ConvertUsdToZar(usdAmount);

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var data = JsonSerializer.Deserialize<ExchangeRateApiResponse>(json, options);

                if (data?.Result != "success")
                    return await _fallback.ConvertUsdToZar(usdAmount);

                return usdAmount * data.ConversionRate;
            }
            catch
            {
                return await _fallback.ConvertUsdToZar(usdAmount);
            }
        }
    }
}