using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Framework.Models.DataTables
{
    /// <summary>
    /// Represents base DataTables model
    /// </summary>
    public partial class DataTablesModel : BaseNopModel
    {
        #region Const

        private const string DEFAULT_DOM = "lrtip";

        private const string DEFAULT_PAGING_TYPE = "full_numbers";

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public DataTablesModel()
        {
            this.Dom = DEFAULT_DOM;
            this.Ordering = false;
            this.Paging = true;
            this.PagingType = DEFAULT_PAGING_TYPE;
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets or sets table name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets Url for data read (ajax)
        /// </summary>
        public DataUrl UrlRead { get; set; }
        /// <summary>
        /// Gets or Url for custom action
        /// </summary>
        public DataUrl UrlAction { get; set; }
        /// <summary>
        /// Gets or sets url for delete selected items
        /// </summary>
        public DataUrl DeleteSelected { get; set; }
        /// <summary>
        /// Gets or sets search button Id
        /// </summary>
        public string SearchButtonId { get; set; }
        /// <summary>
        /// Gets or set filters name controls
        /// </summary>
        public List<string> Filters { get; set; }
        /// <summary>
        /// Gets or sets data for table (ajax, json, array)
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// Enable or disable the display of a 'processing' indicator when the table is being processed 
        /// </summary>
        public bool Processing { get; set; }
        /// <summary>
        /// Feature control DataTables' server-side processing mode.
        /// </summary>
        public bool ServerSide { get; set; }
        /// <summary>
        /// Enable or disable table pagination.
        /// </summary>
        public bool Paging { get; set; }
        /// <summary>
        /// Pagination button display options.
        /// </summary>
        public string PagingType { get; set; }
        /// <summary>
        /// This parameter allows you to readily specify the entries in the length drop down select list that DataTables shows when pagination is enabled.
        /// </summary>
        public string LengthMenu { get; set; }
        /// <summary>
        /// Feature control ordering (sorting) abilities in DataTables.
        /// </summary>
        public bool Ordering { get; set; }
        /// <summary>
        /// Determines whether the table header should be fixed when scrolling
        /// </summary>
        public bool FixedHeader { get; set; }
        /// <summary>
        /// Define the table control elements to appear on the page and in what order.
        /// </summary>
        public string Dom { get; set; }
        /// <summary>
        /// Gets or sets custom render header function name(js)
        /// See also https://datatables.net/reference/option/headerCallback
        /// </summary>
        public string HeaderCallback { get; set; }
        /// <summary>
        /// Gets or sets custom render footer function name(js)
        /// See also https://datatables.net/reference/option/footerCallback
        /// </summary>
        public string FooterCallback { get; set; }
        /// <summary>
        /// Gets or set column collection 
        /// </summary>
        public List<ColumnProperty> ColumnCollection { get; set; }
        /// <summary>
        /// Gets or sets column defininition
        /// </summary>
        public List<ColumnDefinition> ColumnDefs { get; set; }

        #endregion
    }
}