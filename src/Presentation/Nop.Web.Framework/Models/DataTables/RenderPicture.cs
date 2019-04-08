namespace Nop.Web.Framework.Models.DataTables
{
    /// <summary>
    /// Represents picture render for DataTables column
    /// </summary>
    public partial class RenderPicture : IRender
    {
        #region Ctor

        public RenderPicture(string srcPrefix = "")
        {
            SrcPrefix = srcPrefix;
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

        #endregion
    }
}