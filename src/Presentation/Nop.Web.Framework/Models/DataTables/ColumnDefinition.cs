namespace Nop.Web.Framework.Models.DataTables
{
    /// <summary>
    /// DataTables column definition
    /// </summary>
    public partial class ColumnDefinition
    {
        public ColumnDefinition()
        {
            this.Visible = true;
        }

        public string Targets { get; set; }

        public bool Visible { get; set; }

        public bool Searchable { get; set; }

        public string Width { get; set; }

        public string ClassName { get; set; }        
    }    
}