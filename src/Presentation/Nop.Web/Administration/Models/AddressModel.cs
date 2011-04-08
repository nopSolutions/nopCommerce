using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Core.Domain.Directory;

namespace Nop.Admin.Models
{
    public class AddressModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public int? CountryId { get; set; }
        public int? StateProvinceId { get; set; }
        public string City { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string ZipPostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public virtual Country Country { get; set; }
        public virtual StateProvince StateProvince { get; set; }
    }
}