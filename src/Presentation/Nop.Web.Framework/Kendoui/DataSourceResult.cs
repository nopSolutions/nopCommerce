using System.Collections;

namespace Nop.Web.Framework.Kendoui
{
    /// <summary>
    /// DataSource result
    /// </summary>
    public class DataSourceResult
    {
        /// <summary>
        /// Extra data
        /// </summary>
        public object ExtraData { get; set; }

        /// <summary>
        /// Data
        /// </summary>
        public IEnumerable Data { get; set; }

        /// <summary>
        /// Errors
        /// </summary>
        public object Errors { get; set; }

        /// <summary>
        /// Total records
        /// </summary>
        public int Total { get; set; }
    }
}
