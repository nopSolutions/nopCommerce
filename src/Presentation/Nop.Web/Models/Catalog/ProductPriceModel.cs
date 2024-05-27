using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

public record ProductPriceModel : BaseNopModel
{
    public string OldPrice { get; set; }
    public decimal? OldPriceValue { get; set; }
    public string Price { get; set; }
    public decimal? PriceValue { get; set; }
    /// <summary>
    /// PAngV baseprice (used in Germany)
    /// </summary>
    public string BasePricePAngV { get; set; }
    public decimal? BasePricePAngVValue { get; set; }

    public bool DisableBuyButton { get; set; }
    public bool DisableWishlistButton { get; set; }
    public bool DisableAddToCompareListButton { get; set; }

    public bool AvailableForPreOrder { get; set; }
    public DateTime? PreOrderAvailabilityStartDateTimeUtc { get; set; }

    public bool IsRental { get; set; }

    public bool ForceRedirectionAfterAddingToCart { get; set; }

    /// <summary>
    /// A value indicating whether we should display tax/shipping info (used in Germany)
    /// </summary>
    public bool DisplayTaxShippingInfo { get; set; }

    /// <summary>
    /// The currency (in 3-letter ISO 4217 format) of the offer price 
    /// </summary>
    public string CurrencyCode { get; set; }
    
    public string PriceWithDiscount { get; set; }
    public decimal? PriceWithDiscountValue { get; set; }

    public bool CustomerEntersPrice { get; set; }

    public bool CallForPrice { get; set; }

    public int ProductId { get; set; }

    public bool HidePrices { get; set; }
    
    public string RentalPrice { get; set; }
    public decimal? RentalPriceValue { get; set; }

}
