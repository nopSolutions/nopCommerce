namespace Nop.Web.Framework.DataTables
{
    public partial class RenderDate : Render
    {
        public RenderDate(string format)
        {
            this.Type = RenderType.Date;
            this.Format = format;
        }

        public string Format { get; set; }
    }
}
