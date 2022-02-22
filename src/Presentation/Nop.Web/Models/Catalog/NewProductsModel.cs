using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog
{
    /// <summary>
    /// Represents a new products model
    /// </summary>
    public partial record NewProductsModel : BaseNopEntityModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the catalog products model
        /// </summary>
        public CatalogProductsModel CatalogProductsModel { get; set; }

        #endregion

        #region Ctor

        public NewProductsModel()
        {
            CatalogProductsModel = new CatalogProductsModel();
        }

        #endregion
    }
}
