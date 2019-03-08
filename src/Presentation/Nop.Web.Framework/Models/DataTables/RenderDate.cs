namespace Nop.Web.Framework.Models.DataTables
{
    /// <summary>
    /// Represents date render for DataTables column
    /// </summary>
    public partial class RenderDate : Render
    {
        public RenderDate(string format)
        {
            this.Type = RenderType.Date;
            this.Format = format;
        }

        /// <summary>
        /// Gets or sets format date (moment.js)
        /// See also "http://momentjs.com/"
        /// </summary>
        public string Format { get; set; }
    }
}
