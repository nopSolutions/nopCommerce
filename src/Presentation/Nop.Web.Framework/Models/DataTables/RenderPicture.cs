namespace Nop.Web.Framework.Models.DataTables
{
    /// <summary>
    /// Represents picture render for DataTables column
    /// </summary>
    public partial class RenderPicture : IRender
    {
        #region Properties

        /// <summary>
        /// Gets or sets picture source
        /// </summary>
        public string Src { get; set; }

        #endregion
    }
}