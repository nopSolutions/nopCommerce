namespace Nop.Web.Framework.DataTables
{
    /// <summary>
    /// Represents button render for DataTables column
    /// </summary>
    public partial class RenderButton : Render
    {
        public RenderButton(string title, StyleButton style)
        {
            this.Type = RenderType.Button;
            this.Title = title;
            this.Style = style;
        }

        /// <summary>
        /// Gets or sets button title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets button style
        /// </summary>
        public StyleButton Style {get; set;}
    }
}
