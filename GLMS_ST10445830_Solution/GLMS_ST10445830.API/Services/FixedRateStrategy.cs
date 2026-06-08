namespace GLMS_ST10445830.API.Services
{

    /* this class serves as a fallback should the api not work properly,
     * follows the ideology of a strategy pattern
     * takes the convert to zar to usd and does the calulation manually
     * 
     */
    public class FixedRateStrategy : ICurrencyConverter
    {
        public Task<decimal> ConvertUsdToZar(decimal usdAmount)
        {
            decimal fallbackRate = 18.50m;
            return Task.FromResult(usdAmount * fallbackRate);
        }
    }
}
