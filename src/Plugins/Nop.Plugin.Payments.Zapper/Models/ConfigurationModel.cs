using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.Zapper.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Merchant ID")]
        public string MerchantId { get; set; }
        public bool MerchantId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Site ID")]
        public string SiteId { get; set; }
        public bool SiteId_OverrideForStore { get; set; }

        [NopResourceDisplayName("POS Key")]
        public string PosKey { get; set; }
        public bool PosKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("POS Token")]
        public string PosToken { get; set; }
        public bool PosToken_OverrideForStore { get; set; }

    }
}
