using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Tax.Avalara.Models.Settings
{
    /// <summary>
    /// Represents a tax origin address type model
    /// </summary>
    public class TaxOriginAddressTypeModel : BaseNopModel
    {
        #region Ctor

        public TaxOriginAddressTypeModel()
        {
            TaxOriginAddressTypes = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        public string PrecedingElementId { get; set; }

        [NopResourceDisplayName("Plugins.Tax.Avalara.Fields.TaxOriginAddressType")]
        public int AvalaraTaxOriginAddressType { get; set; }
        public IList<SelectListItem> TaxOriginAddressTypes { get; set; }

        #endregion
    }
}