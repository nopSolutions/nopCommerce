using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Templates
{
    /// <summary>
    /// Represents a category template model
    /// </summary>
    public partial class CategoryTemplateModel : BaseNopEntityModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.System.Templates.Category.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.System.Templates.Category.ViewPath")]
        public string ViewPath { get; set; }

        [NopResourceDisplayName("Admin.System.Templates.Category.DisplayOrder")]
        public int DisplayOrder { get; set; }

        #endregion
    }
}