namespace Nop.Web.Framework.DataTables
{
    public partial class RenderLink : Render
    {
        public RenderLink(DataUrl url, string title)
        {
            this.Type = RenderType.Link;
            this.Url = url;
            this.Title = title;
        }

        public DataUrl Url { get; set; }
        public string Title { get; set; }
    }
}
