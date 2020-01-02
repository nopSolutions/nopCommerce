using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a search model of products that use the specification attribute
    /// </summary>
    public partial class SpecificationAttributeProductSearchModel : BaseSearchModel
    {
        #region Properties

        public int SpecificationAttributeId { get; set; }

        #endregion
    }
}