namespace Nop.Web.Framework.Models.DataTables
{
    /// <summary>
    /// Represents picture render for DataTables column
    /// </summary>
    public partial class RenderPicture : IRender
    {
        #region Ctor

        public RenderPicture(string srcPrefix = "", int width = 0)
        {
            SrcPrefix = srcPrefix;
            Width = width;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets picture URL prefix
        /// </summary>
        public string SrcPrefix { get; set; }

        /// <summary>
        /// Gets or sets picture source
        /// </summary>
        public string Src { get; set; }

        /// <summary>
        /// Gets or sets picture width
        /// </summary>
        public int Width { get; set; }

        #endregion
    }
}