namespace Nop.Web.Framework.DataTables
{
    /// <summary>
    /// Represents custom render for DataTables column
    /// </summary>
    public partial class RenderCustom : Render
    {
        public RenderCustom(string function)
        {
            this.Type = RenderType.Custom;
            this.Function = function;
        }

        /// <summary>
        /// Gets or sets custom render function (js)
        /// </summary>
        public string Function { get; set; }
    }
}
