namespace Nop.Core.Domain.Customers;

/// <summary>
/// Represents the customer registration type formatting enumeration
/// </summary>
public enum UserRegistrationType
{
    /// <summary>
    /// Standard account creation
    /// </summary>
    Standard = 1,

    /// <summary>
    /// Email validation is required after registration
    /// </summary>
    EmailValidation = 2,

    /// <summary>
    /// A customer should be approved by administrator
    /// </summary>
    AdminApproval = 3,

    /// <summary>
    /// Registration is disabled
    /// </summary>
    Disabled = 4
}