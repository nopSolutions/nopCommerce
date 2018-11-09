using Nop.Core.Domain.Common;
using Nop.Core.Infrastructure;

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

            //set the default values for DataTables
            this.Draw = "";
            this.Start = 0;
            this.Length = 0;
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

        /// <summary>
        /// Gets or sets a comma-separated list of available page sizes
        /// </summary>
        public string AvailablePageSizes { get; set; }

        public string Draw { get; set; }

        public int Start { get; set; }

        public int Length { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Set grid page parameters
        /// </summary>
        public void SetGridPageSize()
        {
            var adminAreaSettings = EngineContext.Current.Resolve<AdminAreaSettings>();

            this.Page = 1;
            this.PageSize = adminAreaSettings.DefaultGridPageSize;
            this.AvailablePageSizes = adminAreaSettings.GridPageSizes;
        }

        /// <summary>
        /// Set popup grid page parameters
        /// </summary>
        public void SetPopupGridPageSize()
        {
            var adminAreaSettings = EngineContext.Current.Resolve<AdminAreaSettings>();

            this.Page = 1;
            this.PageSize = adminAreaSettings.PopupGridPageSize;
            this.AvailablePageSizes = adminAreaSettings.GridPageSizes;
        }

        #endregion
    }
}