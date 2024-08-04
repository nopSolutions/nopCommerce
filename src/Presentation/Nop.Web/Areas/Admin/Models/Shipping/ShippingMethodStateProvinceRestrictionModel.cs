using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Shipping;

/// <summary>
/// Represents a shipping method state province restriction model
/// </summary>
public partial record ShippingMethodStateProvinceRestrictionModel : BaseNopModel
{
    #region Ctor

    public ShippingMethodStateProvinceRestrictionModel()
    {
        AvailableShippingMethods = new List<ShippingMethodModel>();
        AvailableStateProvinces = new List<StateProvinceModel>();
        Restricted = new Dictionary<int, IDictionary<int, bool>>();
    }

    #endregion

    #region Properties

    public int CountryId { get; set; }

    public IList<ShippingMethodModel> AvailableShippingMethods { get; set; }

    public IList<StateProvinceModel> AvailableStateProvinces { get; set; }

    //[state province id] / [shipping method id] / [restricted]
    public IDictionary<int, IDictionary<int, bool>> Restricted { get; set; }

    #endregion
}