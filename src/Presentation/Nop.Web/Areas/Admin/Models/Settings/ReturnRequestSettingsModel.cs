using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings;

public partial record ReturnRequestSettingsModel : BaseNopModel, ISettingsModel
{
    public int ActiveStoreScopeConfiguration {  get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Order.ReturnRequestsEnabled")]
    public bool ReturnRequestsEnabled { get; set; }
    public bool ReturnRequestsEnabled_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Order.ReturnRequestsAllowFiles")]
    public bool ReturnRequestsAllowFiles { get; set; }
    public bool ReturnRequestsAllowFiles_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Order.ReturnRequestNumberMask")]
    public string ReturnRequestNumberMask { get; set; }
    public bool ReturnRequestNumberMask_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Order.NumberOfDaysReturnRequestAvailable")]
    public int NumberOfDaysReturnRequestAvailable { get; set; }
    public bool NumberOfDaysReturnRequestAvailable_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Order.UseEuWithdrawalLocales")]
    public bool UseEuWithdrawalLocales { get; set; }
    public bool UseEuWithdrawalLocales_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Order.WithdrawalLinkDaysValid")]
    public int WithdrawalLinkDaysValid { get; set; }
    public bool WithdrawalLinkDaysValid_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Order.GuestReturnRequestsAllowed")]
    public bool GuestReturnRequestsAllowed { get; set; }
    public bool GuestReturnRequestsAllowed_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Order.ReturnReasonsEnabled")]
    public bool ReturnReasonsEnabled { get; set; }
    public bool ReturnReasonsEnabled_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Order.ReturnActionsEnabled")]
    public bool ReturnActionsEnabled { get; set; }
    public bool ReturnActionsEnabled_OverrideForStore { get; set; }

    public ReturnRequestReasonSearchModel ReturnRequestReasonSearchModel { get; set; } = new();
    public ReturnRequestActionSearchModel ReturnRequestActionSearchModel { get; set; } = new();
}
