namespace Nop.Services.Media.ElFinder;

/// <summary>
/// elFinder file provider interface
/// </summary>
public partial interface IElFinderFileProvider
{
    /// <summary>
    /// Initialize file provider
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InitializeAsync();

    /// <summary>
    /// Get file provider root path
    /// </summary>
    /// <returns>Root path</returns>
    string GetRootPath();

    /// <summary>
    /// Get URL base for file access
    /// </summary>
    /// <returns>URL base</returns>
    string GetUrlBase();

    /// <summary>
    /// Get URL thumb
    /// </summary>
    /// <returns>URL thumb</returns>
    string GetUrlThumb();
}
