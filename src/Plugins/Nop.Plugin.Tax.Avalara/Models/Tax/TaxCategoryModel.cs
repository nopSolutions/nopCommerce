using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Tax.Avalara.Models.Tax
{
    /// <summary>
    /// Represents an extended tax category model
    /// </summary>
    public class TaxCategoryModel : Nop.Web.Areas.Admin.Models.Tax.TaxCategoryModel
    {
        #region Properties

        public string Description { get; set; }

        [NopResourceDisplayName("Plugins.Tax.Avalara.Fields.TaxCodeType")]
        public string TypeId { get; set; }
        public string Type { get; set; }

        #endregion
    }
}