using Nop.Data;

namespace Nop.Services.Common
{
    /// <summary>
    /// Full-Text service
    /// </summary>
    public partial class FulltextService : IFulltextService
    {
        #region Fields

        private INopDataProvider _dataProvider;

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
        public virtual bool IsFullTextSupported()
        {
            return _dataProvider.ExecuteStoredProcedure<bool>("FullText_IsSupported");
        }

        /// <summary>
        /// Enable Full-Text support
        /// </summary>
        public virtual void EnableFullText()
        {
            _dataProvider.ExecuteStoredProcedure("FullText_Enable");
        }

        /// <summary>
        /// Disable Full-Text support
        /// </summary>
        public virtual void DisableFullText()
        {
            _dataProvider.ExecuteStoredProcedure("FullText_Disable");
        }

        #endregion
    }
}