namespace Nop.Web.Framework.DataTables
{
    /// <summary>
    /// Represents picture render for DataTables column
    /// </summary>
    public partial class RenderPicture : Render
    {
        public RenderPicture(string src)
        {
            this.Type = RenderType.Picture;
            this.Src = src;
        }

        /// <summary>
        /// Gets or sets picture source
        /// </summary>
        public string Src { get; set; }
    }
}
