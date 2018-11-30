namespace Nop.Web.Framework.DataTables
{
    public partial class RenderCustom : Render
    {
        public RenderCustom(string function)
        {
            this.Type = RenderType.Custom;
            this.Function = function;
        }

        public string Function { get; set; }
    }
}
