using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Misc.PolyCommerce.Models
{
    public class PolyCommerceAddress
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public string TwoLetterCountryCode { get; set; }
        public string ZipPostalCode { get; set; }
    }
}
