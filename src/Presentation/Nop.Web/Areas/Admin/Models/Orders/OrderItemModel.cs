using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Orders;

/// <summary>
/// Represents an order item model
/// </summary>
public partial record OrderItemModel : BaseNopEntityModel
{
    #region Ctor

    public OrderItemModel()
    {
        PurchasedGiftCardIds = new List<int>();
        ReturnRequests = new List<ReturnRequestBriefModel>();
    }

    #endregion

    #region Properties

    public int ProductId { get; set; }

    public string ProductName { get; set; }

    public string VendorName { get; set; }

    public string Sku { get; set; }

    public string PictureThumbnailUrl { get; set; }

    public string UnitPriceInclTax { get; set; }

    public string UnitPriceExclTax { get; set; }

    public decimal UnitPriceInclTaxValue { get; set; }

    public decimal UnitPriceExclTaxValue { get; set; }

    public int Quantity { get; set; }

    public string DiscountInclTax { get; set; }

    public string DiscountExclTax { get; set; }

    public decimal DiscountInclTaxValue { get; set; }

    public decimal DiscountExclTaxValue { get; set; }

    public string SubTotalInclTax { get; set; }

    public string SubTotalExclTax { get; set; }

    public decimal SubTotalInclTaxValue { get; set; }

    public decimal SubTotalExclTaxValue { get; set; }

    public string AttributeInfo { get; set; }

    public string RecurringInfo { get; set; }

    public string RentalInfo { get; set; }

    public IList<ReturnRequestBriefModel> ReturnRequests { get; set; }

    public IList<int> PurchasedGiftCardIds { get; set; }

    public bool IsDownload { get; set; }

    public int DownloadCount { get; set; }

    public DownloadActivationType DownloadActivationType { get; set; }

    public bool IsDownloadActivated { get; set; }

    public Guid LicenseDownloadGuid { get; set; }

    #endregion

    #region Nested Classes

    public partial record ReturnRequestBriefModel : BaseNopEntityModel
    {
        public string CustomNumber { get; set; }
    }

    #endregion
}