using Microsoft.AspNetCore.Http;

namespace Million.Application.DTOs
{
    public class CreateOwnerRequestDTO
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public IFormFile Photo { get; set; }
        public DateTime Birthday { get; set; }
    }
}
