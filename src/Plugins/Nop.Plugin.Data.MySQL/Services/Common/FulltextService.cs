using Nop.Data;

namespace Nop.Plugin.Data.MySQL.Services.Common
{
    public class FulltextService : Nop.Services.Common.FulltextService
    {
        #region Ctor

        public FulltextService(IDbContext dbContext) : base(dbContext)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets value indicating whether Full-Text is supported
        /// </summary>
        /// <returns>Result</returns>
        public override bool IsFullTextSupported()
        {
            return false;
        }

        #endregion
    }
}
