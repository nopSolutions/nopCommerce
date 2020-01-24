using System;
using Nop.Data;

//TODO: 239 try to implement FulltextService
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
            using (var dataContext = _dataProvider.CreateDataContext())
            {
                return dataContext.ExecuteStoredProcedure<bool>("FullText_IsSupported");
            }
        }

        /// <summary>
        /// Enable Full-Text support
        /// </summary>
        public virtual void EnableFullText()
        {
            using (var dataContext = _dataProvider.CreateDataContext())
            {
                dataContext.ExecuteStoredProcedure("FullText_Enable");
            }
        }

        /// <summary>
        /// Disable Full-Text support
        /// </summary>
        public virtual void DisableFullText()
        {
            using (var dataContext = _dataProvider.CreateDataContext())
            {
                dataContext.ExecuteStoredProcedure("FullText_Disable");
            }
        }

        #endregion
    }
}