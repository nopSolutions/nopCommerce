namespace Nop.Web.Framework.Models.DataTables
{
    /// <summary>
    /// Represents button edit render for DataTables column
    /// </summary>
    public partial class RenderButtonEdit : Render
    {
        public RenderButtonEdit(DataUrl url)
        {
            this.Type = RenderType.ButtonEdit;
            this.Url = url;
            this.Style = StyleButton.defaultStyle;
        }

        /// <summary>
        /// Gets or sets Url to action edit
        /// </summary>
        public DataUrl Url { get; set; }

        /// <summary>
        /// Gets or sets button style
        /// </summary>
        public StyleButton Style { get; set; }
    }
}
