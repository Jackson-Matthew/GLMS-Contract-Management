
namespace GLMS_ST10445830.Services
{
    /*the purpose of this interface is to define a contract for 
     * currency conversion.
     * Can be used for different classes for different strategies 
     * for conversion, such as a fixed rate strategy or api strategy
     * 
     * declares that all contracts must follow a method that converts currency
     */

    public interface ICurrencyConverter
    {
        Task<decimal> ConvertUsdToZar(decimal usdAmount);
    }
}