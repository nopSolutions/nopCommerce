namespace Nop.Web.Framework.DataTables
{
    /// <summary>
    /// Represent DataTables column property
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
}