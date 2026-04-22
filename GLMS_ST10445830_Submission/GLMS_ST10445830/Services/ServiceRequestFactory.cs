using GLMS_ST10445830.Models;
using static GLMS_ST10445830.Models.Contract;
using static GLMS_ST10445830.Models.ServiceRequest;

namespace GLMS_ST10445830.Services
{

    /*new service request factory that implements the interface, 
     * this is used to create new service requests in a standardized way, 
     * it also allows for easy changes to the creation process without affecting the rest of the code
     * 
     * calls the interface metjpd to make a service request
     */
    public class ServiceRequestFactory : IServiceRequestFactory
    {
        public ServiceRequest Create(int contractId, string description, decimal USDAmount)
        {
            return new ServiceRequest
            {
                ContractId = contractId,
                Description = description,
                USDAmount = USDAmount,           
                Status = RequestStatus.Pending
            };
        }
    }
}
