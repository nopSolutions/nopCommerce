using Nop.Web.Models.Vendors;

namespace Nop.Web.Factories
{
    public partial interface IVendorModelFactory
    {
        ApplyVendorModel PrepareApplyVendorModel(ApplyVendorModel model, bool validateVendor,bool excludeProperties);

        VendorInfoModel PrepareVendorInfoModel(VendorInfoModel model, bool excludeProperties);
    }
}
