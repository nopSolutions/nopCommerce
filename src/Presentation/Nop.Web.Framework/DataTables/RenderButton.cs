namespace Nop.Web.Framework.DataTables
{
    public partial class RenderButton : Render
    {
        public RenderButton(string title)
        {
            this.Type = RenderType.Button;
            this.Title = title;
        }

        public string Title { get; set; }
    }
}
