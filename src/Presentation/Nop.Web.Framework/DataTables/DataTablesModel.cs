using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Nop.Web.Framework.DataTables
{
    /// <summary>
    /// Represents base DataTables model
    /// </summary>
    public partial class DataTablesModel
    {
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public DataTablesModel()
        {
            this.Dom = "lrtip";
            this.FixedHeader = false;
            this.Ordering = false;
            this.Paging = true;            
            this.CustomProperties = new Dictionary<string, object>();
            PostInitialize();
        }

        #endregion

        #region Methods
       
        /// <summary>
        /// Perform additional actions for the model initialization
        /// </summary>
        /// <remarks>Developers can override this method in custom partial classes in order to add some custom initialization code to constructors</remarks>
        protected virtual void PostInitialize()
        {            
        }

        #endregion

        #region Properties

        //MVC is suppressing further validation if the IFormCollection is passed to a controller method. That's why we add it to the model
        public IFormCollection Form { get; set; }

        /// <summary>
        /// Gets or sets property to store any custom values for models 
        /// </summary>
        public Dictionary<string, object> CustomProperties { get; set; }
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
        public dynamic Data { get; set; }
        /// <summary>
        /// Gets or sets 
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