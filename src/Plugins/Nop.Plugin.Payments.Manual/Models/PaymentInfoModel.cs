using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.Manual.Models;

public record PaymentInfoModel : BaseNopModel
{
    public PaymentInfoModel()
    {
        CreditCardTypes = new List<SelectListItem>();
        ExpireMonths = new List<SelectListItem>();
        ExpireYears = new List<SelectListItem>();
    }

    [NopResourceDisplayName("Plugins.Payments.Manual.Public.SelectCreditCard")]
    public string CreditCardType { get; set; }

    [NopResourceDisplayName("Plugins.Payments.Manual.Public.SelectCreditCard")]
    public IList<SelectListItem> CreditCardTypes { get; set; }

    [NopResourceDisplayName("Plugins.Payments.Manual.Public.CardholderName")]
    public string CardholderName { get; set; }

    [NopResourceDisplayName("Plugins.Payments.Manual.Public.CardNumber")]
    public string CardNumber { get; set; }

    [NopResourceDisplayName("Plugins.Payments.Manual.Public.ExpirationDate")]
    public string ExpireMonth { get; set; }

    [NopResourceDisplayName("Plugins.Payments.Manual.Public.ExpirationDate")]
    public string ExpireYear { get; set; }

    public IList<SelectListItem> ExpireMonths { get; set; }

    public IList<SelectListItem> ExpireYears { get; set; }

    [NopResourceDisplayName("Plugins.Payments.Manual.Public.CardCode")]
    public string CardCode { get; set; }
}