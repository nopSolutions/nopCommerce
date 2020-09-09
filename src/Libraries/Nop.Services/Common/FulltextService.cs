using System.Threading.Tasks;
using Nop.Data;

namespace Nop.Services.Common
{
    /// <summary>
    /// Full-Text service
    /// </summary>
    public partial class FulltextService : IFulltextService
    {
        #region Fields

        private readonly INopDataProvider _dataProvider;

        #endregion

        #region Ctor

        public FulltextService(INopDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets value indicating whether Full-Text is supported
        /// </summary>
        /// <returns>Result</returns>
        public virtual async Task<bool> IsFullTextSupported()
        {
            return await _dataProvider.ExecuteStoredProcedure<bool>("FullText_IsSupported");
        }

        /// <summary>
        /// Enable Full-Text support
        /// </summary>
        public virtual async Task EnableFullText()
        {
            await _dataProvider.ExecuteStoredProcedure("FullText_Enable");
        }

        /// <summary>
        /// Disable Full-Text support
        /// </summary>
        public virtual async Task DisableFullText()
        {
            await _dataProvider.ExecuteStoredProcedure("FullText_Disable");
        }

        #endregion
    }
}