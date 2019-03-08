namespace Nop.Web.Framework.Models.DataTables
{
    /// <summary>
    /// Represents link render for DataTables column
    /// </summary>
    public partial class RenderLink : Render
    {
        public RenderLink(DataUrl url, string title)
        {
            this.Type = RenderType.Link;
            this.Url = url;
            this.Title = title;
        }

        /// <summary>
        /// Gets or sets Url
        /// </summary>
        public DataUrl Url { get; set; }
        /// <summary>
        /// Gets or sets link title 
        /// </summary>
        public string Title { get; set; }
    }
}
