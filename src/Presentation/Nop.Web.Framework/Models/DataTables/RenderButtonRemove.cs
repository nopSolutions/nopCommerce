namespace Nop.Web.Framework.Models.DataTables
{
    /// <summary>
    /// Represents button render for DataTables column
    /// </summary>
    public partial class RenderButtonRemove : IRender
    {
        #region Ctor

        /// <summary>
        /// Initializes a new instance of the RenderButton class 
        /// </summary>
        /// <param name="title">Button title</param>
        public RenderButtonRemove(string title)
        {
            Title = title;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets button title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets button style
        /// </summary>
        public StyleButton Style { get; set; }

        #endregion
    }
}