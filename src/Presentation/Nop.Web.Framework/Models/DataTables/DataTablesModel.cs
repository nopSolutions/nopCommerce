using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Nop.Web.Framework.Models.DataTables
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

        public string Name { get; set; }
        public DataUrl UrlRead { get; set; }
        public DataUrl UrlAction { get; set; }        

        public string SearchButtonId { get; set; }
        public List<string> Filters { get; set; }

        public dynamic Data { get; set; }
        public bool Processing { get; set; }
        public bool ServerSide { get; set; }
        public bool Paging { get; set; }
        public string LengthMenu { get; set; }
        public bool Ordering { get; set; }
        public bool FixedHeader { get; set; }
        public string Dom { get; set; }

        public List<ColumnProperty> ColumnCollection { get; set; }
        public List<ColumnDefinition> ColumnDefs { get; set; }

        #endregion

    }

    public partial class DataUrl
    {
        public DataUrl(string actionName, string controllerName)
        {
            this.ActionName = actionName;
            this.ControllerName = controllerName;
        }

        public string ActionName { get; set; }

        public string ControllerName { get; set; }
    }

}