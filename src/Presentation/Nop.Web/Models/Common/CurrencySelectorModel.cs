using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common;

public partial record CurrencySelectorModel : BaseNopModel
{
    public CurrencySelectorModel()
    {
        AvailableCurrencies = new List<CurrencyModel>();
    }

    public IList<CurrencyModel> AvailableCurrencies { get; set; }

    public int CurrentCurrencyId { get; set; }
}