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
        Task<bool> IsFullTextSupported();

        /// <summary>
        /// Enable Full-Text support
        /// </summary>
        Task EnableFullText();

        /// <summary>
        /// Disable Full-Text support
        /// </summary>
        Task DisableFullText();
    }
}
