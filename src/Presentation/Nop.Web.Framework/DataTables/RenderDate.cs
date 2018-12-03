namespace Nop.Web.Framework.DataTables
{
    /// <summary>
    /// Represets date render for DataTables column
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
        /// </summary>
        public string Format { get; set; }
    }
}
