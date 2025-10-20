namespace Nop.Services;

/// <summary>
/// Represents a base class for results and response
/// </summary>
public abstract partial class BaseNopResult
{
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
    public IList<string> Errors { get; set; } = new List<string>();
}
