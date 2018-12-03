namespace Nop.Web.Framework.DataTables
{
    /// <summary>
    /// Represent DataTables column definition
    /// </summary>
    public partial class ColumnDefinition
    {
        #region Ctor

        public ColumnDefinition()
        {
            this.Visible = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Assign a column definition to one or more columns.
        /// </summary>
        public string Targets { get; set; }

        /// <summary>
        /// Enable or disable the display of this column.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Enable or disable filtering on the data in this column.
        /// </summary>
        public bool Searchable { get; set; }

        /// <summary>
        /// Column width assignment.
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// Class to assign to each cell in the column.
        /// </summary>
        public string ClassName { get; set; }

        #endregion
    }
}