namespace Nop.Web.Infrastructure
{
    /// <summary>
    /// Interface for custom RouteValues objects
    /// </summary>
    public partial interface IRouteValues
    {
        /// <summary>
        /// The page number
        /// </summary>
        int PageNumber { get; set; }
    }
}