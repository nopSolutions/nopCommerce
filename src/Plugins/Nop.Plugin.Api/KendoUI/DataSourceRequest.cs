namespace Nop.Plugin.Api.Kendoui
{
    /// <summary>
    /// DataSource request
    /// </summary>
    public class DataSourceRequest
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public DataSourceRequest()
        {
            Page = 1;
            PageSize = 10;
        }

        /// <summary>
        /// Page number
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Page size
        /// </summary>
        public int PageSize { get; set; }
    }
}
