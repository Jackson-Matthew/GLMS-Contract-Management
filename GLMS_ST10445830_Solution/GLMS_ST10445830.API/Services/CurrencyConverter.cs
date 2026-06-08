namespace GLMS_ST10445830.API.Services
{
    public class CurrencyConverter : ICurrencyConverter
    {
        // Fixed exchange rate - simple and reliable
        private const decimal ExchangeRate = 18.50m;

        public Task<decimal> ConvertUsdToZar(decimal usdAmount)
        {
            if (usdAmount < 0)
                throw new ArgumentException("Amount cannot be negative.", nameof(usdAmount));

            return Task.FromResult(usdAmount * ExchangeRate);
        }
    }
}