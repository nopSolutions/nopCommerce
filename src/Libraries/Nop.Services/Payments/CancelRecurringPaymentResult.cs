﻿namespace Nop.Services.Payments;

/// <summary>
/// Cancel recurring payment result
/// </summary>
public partial class CancelRecurringPaymentResult
{
    public CancelRecurringPaymentResult()
    {
        Errors = new List<string>();
    }

    /// <summary>
    /// Gets a value indicating whether request has been completed successfully
    /// </summary>
    public bool Success => !Errors.Any();

    /// <summary>
    /// Add error
    /// </summary>
    /// <param name="error">Error</param>
    public virtual void AddError(string error)
    {
        Errors.Add(error);
    }

    /// <summary>
    /// Errors
    /// </summary>
    public IList<string> Errors { get; set; }
}