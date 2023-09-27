using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a specification attribute model
    /// </summary>
    public partial record SpecificationAttributeModel : BaseNopEntityModel, ILocalizedModel<SpecificationAttributeLocalizedModel>
    {
        #region Ctor

        public SpecificationAttributeModel()
        {
            AvailableGroups = new List<SelectListItem>();
            Locales = new List<SpecificationAttributeLocalizedModel>();
            SpecificationAttributeOptionSearchModel = new SpecificationAttributeOptionSearchModel();
            SpecificationAttributeProductSearchModel = new SpecificationAttributeProductSearchModel();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Fields.SpecificationAttributeGroup")]
        public int? SpecificationAttributeGroupId { get; set; }

        public IList<SelectListItem> AvailableGroups { get; set; }

        public IList<SpecificationAttributeLocalizedModel> Locales { get; set; }

        public SpecificationAttributeOptionSearchModel SpecificationAttributeOptionSearchModel { get; set; }

        public SpecificationAttributeProductSearchModel SpecificationAttributeProductSearchModel { get; set; }

        #endregion
    }
}