using Nop.Core.Domain.Common;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Models.DataTables;

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

        /// <summary>
        /// Gets or sets draw
        /// </summary>
        public string Draw { get; set; }

        /// <summary>
        /// Gets or sets skiping number of rows count
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// Gets or sets paging length 
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Gets or sets grid model (DataTables)
        /// </summary>
        public DataTablesModel Grid { get; set; }

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