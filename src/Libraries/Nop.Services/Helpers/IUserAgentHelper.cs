
namespace Nop.Services.Helpers
{
    /// <summary>
    /// User agent helper interface
    /// </summary>
    public partial interface IUserAgentHelper
    {
        /// <summary>
        /// Get a value indicating whether the request is made by search engine (web crawler)
        /// </summary>
        /// <returns>Result</returns>
        bool IsSearchEngine();
    }
}