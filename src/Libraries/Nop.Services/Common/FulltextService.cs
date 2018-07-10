using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Data;

namespace Nop.Services.Common
{
    /// <summary>
    /// Full-Text service
    /// </summary>
    public partial class FulltextService : IFulltextService
    {
        #region Fields

        private readonly CommonSettings _commonSettings;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;

        #endregion

        #region Ctor

        public FulltextService(CommonSettings commonSettings,
            IDataProvider dataProvider,
            IDbContext dbContext)
        {
            this._commonSettings = commonSettings;
            this._dataProvider = dataProvider;
            this._dbContext = dbContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets value indicating whether Full-Text is supported
        /// </summary>
        /// <returns>Result</returns>
        public virtual bool IsFullTextSupported()
        {
            var result = _dbContext
                .QueryFromSql<IntQueryType>("EXEC [FullText_IsSupported]")
                .Select(intValue => intValue.Value).FirstOrDefault();
            return result > 0;
        }

        /// <summary>
        /// Enable Full-Text support
        /// </summary>
        public virtual void EnableFullText()
        {
            _dbContext.ExecuteSqlCommand("EXEC [FullText_Enable]", true);

        }

        /// <summary>
        /// Disable Full-Text support
        /// </summary>
        public virtual void DisableFullText()
        {
            _dbContext.ExecuteSqlCommand("EXEC [FullText_Disable]", true);
        }

        #endregion
    }
}