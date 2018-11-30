namespace Nop.Web.Framework.DataTables
{
    public partial class RenderButtonEdit : Render
    {
        public RenderButtonEdit(DataUrl url)
        {
            this.Type = RenderType.ButtonEdit;
            this.Url = url;
        }

        public DataUrl Url { get; set; }
    }
}
