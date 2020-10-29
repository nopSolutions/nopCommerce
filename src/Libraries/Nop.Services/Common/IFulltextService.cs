using System.Threading.Tasks;

namespace Nop.Services.Common
{
    /// <summary>
    /// Full-Text service interface
    /// </summary>
    public partial interface IFulltextService
    {
        /// <summary>
        /// Gets value indicating whether Full-Text is supported
        /// </summary>
        /// <returns>Result</returns>
        Task<bool> IsFullTextSupportedAsync();

        /// <summary>
        /// Enable Full-Text support
        /// </summary>
        Task EnableFullTextAsync();

        /// <summary>
        /// Disable Full-Text support
        /// </summary>
        Task DisableFullTextAsync();
    }
}
