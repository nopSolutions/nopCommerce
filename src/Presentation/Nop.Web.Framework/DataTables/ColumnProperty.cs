namespace Nop.Web.Framework.DataTables
{
    /// <summary>
    /// Represent DataTables column property
    /// </summary>
    public partial class ColumnProperty
    {
        #region Properties

        /// <summary>
        /// Set the data source for the column from the rows data object / array.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Set the column title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Render (process) the data for use in the table.
        /// </summary>
        public Render Render { get; set; }

        /// <summary>
        /// Column width assignment.
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// Column autowidth assignment.
        /// </summary>
        public bool AutoWidth { get; set; }

        #endregion
    }
}