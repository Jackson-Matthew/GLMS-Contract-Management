using GLMS_ST10445830.Models;

namespace GLMS_ST10445830.Services
{

    /* used to not confuse the business logic with the process
     *  service requests all follow the same logic through as the factory
     *  creates a centralized object, no dependancy on new constructor
     *  standardizes the object
     */
    public interface IServiceRequestFactory
    {
        ServiceRequest Create(int contractId, string description, decimal USDAmount);
    }


}
