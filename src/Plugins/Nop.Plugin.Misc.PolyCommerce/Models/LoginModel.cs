using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Misc.PolyCommerce.Models
{
    public class LoginModel
    {
        public string StoreToken { get; set; }
        public string UserId { get; set; }
        public int StoreIntegrationTypeId { get; set; }
        public string UserEmail { get; set; }
        public string Username { get; set; }
        public string StoreName { get; set; }
        public string CustomerGuid { get; set; }
        public string StoreUrl { get; set; }
    }
}
