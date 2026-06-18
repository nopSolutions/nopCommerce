using Nop.Core.Configuration;

namespace Nop.Core.Domain.Orders;

/// <summary>
/// Return request settings
/// </summary>
public partial class ReturnRequestSettings : ISettings
{
    /// <summary>
    /// Gets or sets a value indicating whether "Return requests" are allowed
    /// </summary>
    public bool ReturnRequestsEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether customers are allowed to upload files
    /// </summary>
    public bool ReturnRequestsAllowFiles { get; set; }

    /// <summary>
    /// Gets or sets maximum file size for upload file (return request). Set 0 to allow any file size
    /// </summary>
    public int ReturnRequestsFileMaximumSize { get; set; }

    /// <summary>
    /// Gets or sets a value "Return requests" number mask
    /// </summary>
    public string ReturnRequestNumberMask { get; set; }

    /// <summary>
    /// Gets or sets a number of days that the Return Request Link will be available for customers after order placing.
    /// </summary>
    public int NumberOfDaysReturnRequestAvailable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use EU-style locales for the Withdrawal button
    /// </summary>
    public bool UseEuWithdrawalLocales { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether return requests are allowed for guests
    /// </summary>
    public bool GuestReturnRequestsAllowed { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether return reasons are enabled 
    /// </summary>
    public bool ReturnReasonsEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether return actions are enabled 
    /// </summary>
    public bool ReturnActionsEnabled { get; set; }

    /// <summary>
    /// Gets or sets a number of days for withdrawal link. Set to 0 if it doesn't expire.
    /// </summary>
    public int WithdrawalLinkDaysValid { get; set; }
}
