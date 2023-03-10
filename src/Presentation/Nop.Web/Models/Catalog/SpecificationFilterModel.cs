using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog
{
    /// <summary>
    /// Represents a specification filter model
    /// </summary>
    public partial record SpecificationFilterModel : BaseNopModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether filtering is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the filtrable specification attributes
        /// </summary>
        public IList<SpecificationAttributeFilterModel> Attributes { get; set; }

        #endregion

        #region Ctor

        public SpecificationFilterModel()
        {
            Attributes = new List<SpecificationAttributeFilterModel>();
        }

        #endregion
    }
}
