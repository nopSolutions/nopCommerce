﻿namespace Nop.Services.Customers;

/// <summary>
/// Change password result
/// </summary>
public partial class ChangePasswordResult
{
    public ChangePasswordResult()
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