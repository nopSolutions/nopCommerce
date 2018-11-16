namespace Nop.Web.Framework.Models.DataTables
{
    /// <summary>
    /// DataTables column property
    /// </summary>
    public partial class ColumnProperty
    {
        public string Data { get; set; }

        public string Title { get; set; }

        public Render Render { get; set; }

        public string Width { get; set; }

        public bool AutoWidth { get; set; }

        public string UrlDelete { get; set; }
    }

    public abstract class Render
    {
        public RenderType Type { get; set; }
    };

    public partial class RenderLink : Render
    {
        public RenderLink(string url, string urlId, string title)
        {
            this.Type = RenderType.Link;
            this.Url = url;
            this.Title = title;
            this.UrlId = urlId;
        }        

        public string Url { get; set; }

        public string UrlId { get; set; }

        public string Title { get; set; }
    }

    public partial class RenderDate : Render
    {
        public RenderDate(string format)
        {
            this.Type = RenderType.Date;
            this.Format = format;
        }

        public string Format { get; set; }
    }

    public partial class RenderButton : Render
    {
        public RenderButton(string title)
        {
            this.Type = RenderType.Button;
            this.Title = title;
        }

        public string Title { get; set; }
    }

    public partial class RenderCheckBox : Render
    {
        public RenderCheckBox(string name)
        {
            this.Type = RenderType.Checkbox;
            this.Name = name;
        }

        public string Name { get; set; }
    }

    public enum RenderType
    {
        Checkbox,
        Date,
        Link,
        Button,
        Custom
    }
}