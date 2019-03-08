namespace Nop.Web.Framework.Models.DataTables
{
    /// <summary>
    /// Represents custom render for DataTables column
    /// </summary>
    public partial class RenderCustom : Render
    {
        public RenderCustom(string functionName)
        {
            this.Type = RenderType.Custom;
            this.FunctionName = functionName;
        }

        /// <summary>
        /// Gets or sets custom render function name(js)
        /// See also https://datatables.net/reference/option/columns.render
        /// </summary>
        public string FunctionName { get; set; }
    }
}
