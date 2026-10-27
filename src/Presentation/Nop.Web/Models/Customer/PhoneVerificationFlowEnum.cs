namespace Nop.Web.Models.Customer;

/// <summary>
/// Represents phone verification flow types
/// </summary>
public enum PhoneVerificationFlowEnum
{
    Login = 1,
    RegisterEmailValidation = 2,
    RegisterAdminApproval = 3,
    RegisterStandard = 4,
    ChangePhoneNumber = 5
}
