namespace Nop.Web.Framework.Models.DataTables
{
    /// <summary>
    /// Represents render (process) the data for use in the DataTables.
    /// </summary>
    public abstract class Render
    {
        /// <summary>
        /// Gets or sets type of render
        /// </summary>
        public RenderType Type { get; set; }
    };    
}
