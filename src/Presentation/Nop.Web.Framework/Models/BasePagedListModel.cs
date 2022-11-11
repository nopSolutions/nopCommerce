using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Web.Framework.Models
{
    /// <summary>
    /// Represents the base paged list model (implementation for DataTables grids)
    /// </summary>
    public abstract partial record BasePagedListModel<T> : BaseNopModel, IPagedModel<T> where T : BaseNopModel
    {
        /// <summary>
        /// Gets or sets data records
        /// </summary>
        public IEnumerable<T> Data { get; set; }

        /// <summary>
        /// Gets or sets draw
        /// </summary>
        [JsonProperty(PropertyName = "draw")]
        public string Draw { get; set; }

        /// <summary>
        /// Gets or sets a number of filtered data records
        /// </summary>
        [JsonProperty(PropertyName = "recordsFiltered")]
        public int RecordsFiltered { get; set; }

        /// <summary>
        /// Gets or sets a number of total data records
        /// </summary>
        [JsonProperty(PropertyName = "recordsTotal")]
        public int RecordsTotal { get; set; }
    }
}