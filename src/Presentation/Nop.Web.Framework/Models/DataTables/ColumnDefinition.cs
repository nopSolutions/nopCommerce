namespace Nop.Web.Framework.Models.DataTables
{
    /// <summary>
    /// Represent DataTables column definition
    /// </summary>
    public partial class ColumnDefinition
    {
        #region Ctor

        public ColumnDefinition()
        {
            //set default values
            Visible = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Assign a column definition to one or more columns.        
        /// Possible uses:
        ///  1. [0..n) - column index counting from the left
        ///  2. (-n..0) - column index counting from the right
        ///  3. A string - class name will be matched on the TH for the column (without a leading .)
        ///  4. "_all" - all columns (i.e. assign a default)
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
        /// Column width assignment. This parameter can be used to define the width of a column, and may take any CSS value (3em, 20px etc).
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// Class to assign to each cell in the column.
        /// </summary>
        public StyleColumn? ClassName { get; set; }

        #endregion
    }
}