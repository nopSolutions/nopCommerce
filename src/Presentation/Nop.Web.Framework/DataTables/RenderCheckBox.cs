namespace Nop.Web.Framework.DataTables
{
    public partial class RenderCheckBox : Render
    {
        public RenderCheckBox(string name)
        {
            this.Type = RenderType.Checkbox;
            this.Name = name;
        }

        public string Name { get; set; }
    }
}
