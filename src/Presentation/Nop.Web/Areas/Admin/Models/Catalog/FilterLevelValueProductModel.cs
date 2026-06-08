using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a filter level value product model
/// </summary>
public partial record FilterLevelValueProductModel : BaseNopEntityModel
{
    #region Properties

    public int FilterLevelValueId { get; set; }

    public int ProductId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Categories.Products.Fields.Product")]
    public string ProductName { get; set; }    

    #endregion
}