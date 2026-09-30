using System;

namespace Nop.Plugin.Api.Rest.Models
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}
