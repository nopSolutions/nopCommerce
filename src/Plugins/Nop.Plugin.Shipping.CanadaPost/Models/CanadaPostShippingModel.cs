using System.ComponentModel;

namespace Nop.Plugin.Shipping.CanadaPost.Models
{
    public class CanadaPostShippingModel
    {
        [DisplayName("Canada Post URL")]
        public string Url { get; set; }

        [DisplayNameAttribute("Canada Post Port")]
        public int Port { get; set; }

        [DisplayNameAttribute("Canada Post Customer ID")]
        public string CustomerId { get; set; }
    }
}