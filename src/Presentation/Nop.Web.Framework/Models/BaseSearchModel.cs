
namespace Nop.Web.Framework.Models
{
    /// <summary>
    /// Represents base search model
    /// </summary>
    public abstract partial class BaseSearchModel : BaseNopModel, IPagingRequestModel
    {
        #region Ctor
        
        public BaseSearchModel()
        {
            //set the default values
            this.Page = 1;
            this.PageSize = 10;
        }

        #endregion
        
        #region Properties

        /// <summary>
        /// Gets or sets a page number
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets a page size
        /// </summary>
        public int PageSize { get; set; }

        #endregion
    }
}