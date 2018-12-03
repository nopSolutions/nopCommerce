namespace Nop.Web.Framework.DataTables
{
    /// <summary>
    /// Represets button edit render for DataTables column
    /// </summary>
    public partial class RenderButtonEdit : Render
    {
        public RenderButtonEdit(DataUrl url)
        {
            this.Type = RenderType.ButtonEdit;
            this.Url = url;
        }

        /// <summary>
        /// Gets or sets Url to action edit
        /// </summary>
        public DataUrl Url { get; set; }
    }
}
