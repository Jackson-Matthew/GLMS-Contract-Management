using GLMS_ST10445830.Models;

namespace GLMS_ST10445830.API.DTOs
{
    public class ClientDto
    {
        public int ClientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;

        public ClientDto() { }

        public ClientDto(Client client)
        {
            ClientId = client.ClientId;
            Name = client.Name;
            Email = client.Email;
            PhoneNumber = client.PhoneNumber;
            Region = client.Region;
        }
    }
}