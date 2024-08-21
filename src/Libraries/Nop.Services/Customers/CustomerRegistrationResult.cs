namespace Nop.Services.Customers;

/// <summary>
/// Customer registration result
/// </summary>
public partial class CustomerRegistrationResult
{
    public CustomerRegistrationResult()
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
    public void AddError(string error)
    {
        Errors.Add(error);
    }

    /// <summary>
    /// Errors
    /// </summary>
    public IList<string> Errors { get; set; }
}