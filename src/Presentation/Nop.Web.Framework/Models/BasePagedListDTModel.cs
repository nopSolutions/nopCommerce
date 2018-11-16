using System.Collections.Generic;

namespace Nop.Web.Framework.Models
{
    /// <summary>
    /// Represents the base paged list model (implementation for DataTables grids)
    /// </summary>
    public abstract partial class BasePagedListDTModel<T> : BaseNopModel, IPagedModel<T> where T : BaseNopModel
    {      
        /// <summary>
        /// Gets or sets data records
        /// </summary>
        public IEnumerable<T> Data { get; set; } 

        public string Draw { get; set; }

        public int RecordsFiltered { get; set; }

        public int RecordsTotal { get; set; }
    }
}