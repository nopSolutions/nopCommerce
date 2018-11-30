namespace Nop.Web.Framework.DataTables
{
    /// <summary>
    /// Represent DataTables column definition
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