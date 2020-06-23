using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Shipping.ShipStation.Models
{
    public class ShipStationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.ShipStation.Fields.ApiKey")]
        public string ApiKey { get; set; }
        public bool ApiKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.ShipStation.Fields.ApiSecret")]
        public string ApiSecret { get; set; }
        public bool ApiSecret_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.ShipStation.Fields.PackingType")]
        public SelectList PackingTypeValues { get; set; }
        public bool PackingType_OverrideForStore { get; set; }
        public int PackingType { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.ShipStation.Fields.PackingPackageVolume")]
        public int PackingPackageVolume { get; set; }
        public bool PackingPackageVolume_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.ShipStation.Fields.PassDimensions")]
        public bool PassDimensions { get; set; }
        public bool PassDimensions_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.ShipStation.Fields.UserName")]
        public string UserName { get; set; }
        public bool UserName_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.ShipStation.Fields.Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool Password_OverrideForStore { get; set; }

        public string WebhookURL { get; set; }
    }
}
