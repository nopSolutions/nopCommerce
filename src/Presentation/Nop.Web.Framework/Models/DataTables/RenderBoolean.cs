namespace Nop.Web.Framework.Models.DataTables
{
    /// <summary>
    /// Represents boolean render for DataTables column
    /// </summary>
    public partial class RenderBoolean : Render
    {
        public RenderBoolean()
        {
            this.Type = RenderType.Boolean;
        }
    }
}
