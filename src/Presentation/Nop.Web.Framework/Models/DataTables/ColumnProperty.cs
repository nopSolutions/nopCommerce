namespace Nop.Web.Framework.Models.DataTables
{
    /// <summary>
    /// Represent DataTables column property
    /// </summary>
    public partial class ColumnProperty
    {
        #region Ctor

        /// <summary>
        /// Initializes a new instance of the ColumnProperty class
        /// </summary>
        /// <param name="data">The data source for the column from the rows data object</param>
        public ColumnProperty(string data)
        {
            Data = data;
            //set default values
            Visible = true;
            Encode = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Set the data source for the column from the rows data object / array.
        /// See also "https://datatables.net/reference/option/columns.data"
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Set the column title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Render (process) the data for use in the table. This property will modify the data that is used by DataTables for various operations.
        /// </summary>
        public IRender Render { get; set; }

        /// <summary>
        /// Column width assignment. This parameter can be used to define the width of a column, and may take any CSS value (3em, 20px etc).
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// Column autowidth assignment. This can be disabled as an optimisation (it takes a finite amount of time to calculate the widths) if the tables widths are passed in using "width".
        /// </summary>
        public bool AutoWidth { get; set; }

        /// <summary>
        /// Indicate whether the column is master check box
        /// </summary>
        public bool IsMasterCheckBox { get; set; }

        /// <summary>
        /// Class to assign to each cell in the column.
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Enable or disable the display of this column.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Enable or disable filtering on the data in this column.
        /// </summary>
        public bool Searchable { get; set; }

        /// <summary>
        /// Enable or disable editing on the data in this column.
        /// </summary>
        public bool Editable { get; set; }

        /// <summary>
        /// Data column type
        /// </summary>
        public EditType EditType { get; set; }

        /// <summary>
        /// Enable or disable encode on the data in this column.
        /// </summary>
        public bool Encode { get; set; }

        #endregion
    }
}