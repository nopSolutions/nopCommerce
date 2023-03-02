namespace Nop.Web.Framework.Models.DataTables
{
    /// <summary>
    /// Represents date render for DataTables column
    /// </summary>
    public partial class RenderDate : IRender
    {
        #region Constants

        /// <summary>
        /// Default date format
        /// </summary>
        /// <remarks>For example english culture: [MM/DD/YYYY h:mm:ss PM/AM] [09/04/1986 8:30:25 PM]</remarks>
        protected const string DEFAULT_DATE_FORMAT = "L LTS";

        #endregion

        #region Ctor

        public RenderDate()
        {
            //set default values
            Format = DEFAULT_DATE_FORMAT;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets format date (moment.js)
        /// See also "http://momentjs.com/"
        /// </summary>
        public string Format { get; set; }

        #endregion
    }
}