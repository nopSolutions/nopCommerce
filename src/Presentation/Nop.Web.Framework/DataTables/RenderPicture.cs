namespace Nop.Web.Framework.DataTables
{
    public partial class RenderPicture : Render
    {
        public RenderPicture(string src)
        {
            this.Type = RenderType.Picture;
            this.Src = src;
        }

        public string Src { get; set; }
    }
}
