using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a specification attribute group model
    /// </summary>
    public partial record SpecificationAttributeGroupModel : BaseNopEntityModel, ILocalizedModel<SpecificationAttributeGroupLocalizedModel>
    {
        #region Ctor

        public SpecificationAttributeGroupModel()
        {
            Locales = new List<SpecificationAttributeGroupLocalizedModel>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<SpecificationAttributeGroupLocalizedModel> Locales { get; set; }

        #endregion
    }
}
