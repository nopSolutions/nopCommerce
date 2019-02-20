using Nop.Core.Domain.Common;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.DataTables;

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
            this.Start = 0;
            this.Length = 10;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a page number
        /// </summary>
        public int Page
        {
            set { }
            get => (this.Start / this.Length) + 1;
        }

        /// <summary>
        /// Gets or sets a page size
        /// </summary>
        public int PageSize
        {
            set { }
            get => this.Length;
        }

        /// <summary>
        /// Gets or sets a comma-separated list of available page sizes
        /// </summary>
        public string AvailablePageSizes { get; set; }

        /// <summary>
        /// Gets or sets draw. Draw counter. This is used by DataTables to ensure that the Ajax returns from server-side processing requests are drawn in sequence by DataTables (Ajax requests are asynchronous and thus can return out of sequence).
        /// </summary>
        public string Draw { get; set; }

        /// <summary>
        /// Gets or sets skiping number of rows count. Paging first record indicator.
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// Gets or sets paging length. Number of records that the table can display in the current draw. 
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

            this.Start = 0;
            this.Length = adminAreaSettings.DefaultGridPageSize;
            this.AvailablePageSizes = adminAreaSettings.GridPageSizes;
        }

        /// <summary>
        /// Set popup grid page parameters
        /// </summary>
        public void SetPopupGridPageSize()
        {
            var adminAreaSettings = EngineContext.Current.Resolve<AdminAreaSettings>();

            this.Start = 0;
            this.Length = adminAreaSettings.PopupGridPageSize;
            this.AvailablePageSizes = adminAreaSettings.GridPageSizes;
        }

        #endregion
    }
}