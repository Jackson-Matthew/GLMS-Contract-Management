namespace GLMS_ST10445830.Models
{
    public class Client
    {

        // this model class is for the clients, conatins name, email, phone number and region /;
        public int ClientId { get; set; }
        public string Name { get; set; } 

        public string Email { get; set; }

        public string PhoneNumber { get; set; } 

        public string Region { get; set; }

        // client object can access all contracts linked to that client
        //flexible collection that is an editable list 
        public ICollection<Contract> Contracts { get; set;} = new List<Contract>();
    }
}
