namespace Nop.Web.Framework.Models.DataTables
{
    /// <summary>
    /// Represents link render for DataTables column
    /// </summary>
    public partial class RenderLink : IRender
    {
        #region Ctor

        /// <summary>
        /// Initializes a new instance of the RenderButton class 
        /// </summary>
        /// <param name="url">URL</param>
        public RenderLink(DataUrl url)
        {
            Url = url;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets Url
        /// </summary>
        public DataUrl Url { get; set; }

        /// <summary>
        /// Gets or sets link title 
        /// </summary>
        public string Title { get; set; }

        #endregion
    }
}