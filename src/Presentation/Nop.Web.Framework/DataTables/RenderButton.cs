namespace Nop.Web.Framework.DataTables
{
    /// <summary>
    /// Represets button render for DataTables column
    /// </summary>
    public partial class RenderButton : Render
    {
        public RenderButton(string title)
        {
            this.Type = RenderType.Button;
            this.Title = title;
        }

        /// <summary>
        /// Gets or sets button title
        /// </summary>
        public string Title { get; set; }
    }
}
