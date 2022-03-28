using Nop.Services.Tax;

namespace Nop.Plugin.Misc.AbcFrontend.Services
{
    public interface ICustomTaxProvider : ITaxProvider
    {
        bool GetCustomerInTaxableState(int taxCategoryId, int countryId, int stateProvinceId, string zip);
    }
}
