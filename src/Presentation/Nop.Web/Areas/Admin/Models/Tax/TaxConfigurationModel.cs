using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Tax;

/// <summary>
/// Represents a tax configuration model
/// </summary>
public partial record TaxConfigurationModel : BaseNopModel
{
    #region Ctor

    public TaxConfigurationModel()
    {
        TaxProviders = new TaxProviderSearchModel();
        TaxCategories = new TaxCategorySearchModel();
    }

    #endregion

    #region Properties

    public TaxProviderSearchModel TaxProviders { get; set; }

    public TaxCategorySearchModel TaxCategories { get; set; }

    #endregion
}