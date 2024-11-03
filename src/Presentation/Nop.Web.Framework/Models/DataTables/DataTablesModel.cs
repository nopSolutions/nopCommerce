namespace Nop.Web.Framework.Models.DataTables;

/// <summary>
/// Represents base DataTables model
/// </summary>
public partial record DataTablesModel : BaseNopModel
{
    #region Const

    protected const string DEFAULT_PAGING_TYPE = "simple_numbers";

    #endregion

    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    public DataTablesModel()
    {
        //set default values
        Info = true;
        RefreshButton = true;
        ServerSide = true;
        Processing = true;
        Paging = true;
        PagingType = DEFAULT_PAGING_TYPE;

        Filters = new List<FilterParameter>();
        ColumnCollection = new List<ColumnProperty>();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets table name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets URL for data read (ajax)
    /// </summary>
    public DataUrl UrlRead { get; set; }

    /// <summary>
    /// Gets or sets URL for delete action (ajax)
    /// </summary>
    public DataUrl UrlDelete { get; set; }

    /// <summary>
    /// Gets or sets URL for update action (ajax)
    /// </summary>
    public DataUrl UrlUpdate { get; set; }

    /// <summary>
    /// Gets or sets search button Id
    /// </summary>
    public string SearchButtonId { get; set; }

    /// <summary>
    /// Gets or set filters controls
    /// </summary>
    public IList<FilterParameter> Filters { get; set; }

    /// <summary>
    /// Gets or sets data for table (ajax, json, array)
    /// </summary>
    public object Data { get; set; }

    /// <summary>
    /// Enable or disable the display of a 'processing' indicator when the table is being processed 
    /// </summary>
    public bool Processing { get; set; }

    /// <summary>
    /// Feature control DataTables' server-side processing mode
    /// </summary>
    public bool ServerSide { get; set; }

    /// <summary>
    /// Enable or disable table pagination.
    /// </summary>
    public bool Paging { get; set; }

    /// <summary>
    /// Enable or disable information ("1 to n of n entries")
    /// </summary>
    public bool Info { get; set; }

    /// <summary>
    /// If set, populate the row id based on the specified field
    /// </summary>
    public string RowIdBasedOnField { get; set; }

    /// <summary>
    /// Enable or disable refresh button
    /// </summary>
    public bool RefreshButton { get; set; }

    /// <summary>
    /// Pagination button display options.
    /// </summary>
    public string PagingType { get; set; }

    /// <summary>
    /// Number of rows to display on a single page when using pagination
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    /// This parameter allows you to readily specify the entries in the length drop down select list that DataTables shows when pagination is enabled
    /// </summary>
    public string LengthMenu { get; set; }

    /// <summary>
    /// Feature control ordering (sorting) abilities in DataTables
    /// </summary>
    public bool Ordering { get; set; }

    /// <summary>
    /// Gets or sets custom render header function name(js)
    /// See also https://datatables.net/reference/option/headerCallback
    /// </summary>
    public string HeaderCallback { get; set; }

    /// <summary>
    /// Gets or sets custom render function name(js) that is called every time DataTables performs a draw
    /// See also https://datatables.net/reference/option/drawCallback
    /// </summary>
    public string DrawCallback { get; set; }

    /// <summary>
    /// Gets or sets a number of columns to generate in a footer. Set 0 to disable footer
    /// </summary>
    public int FooterColumns { get; set; }

    /// <summary>
    /// Gets or sets custom render footer function name(js)
    /// See also https://datatables.net/reference/option/footerCallback
    /// </summary>
    public string FooterCallback { get; set; }

    /// <summary>
    /// Gets or sets indicate of child table
    /// </summary>
    public bool IsChildTable { get; set; }

    /// <summary>
    /// Gets or sets child table
    /// </summary>
    public DataTablesModel ChildTable { get; set; }

    /// <summary>
    /// Gets or sets primary key column name for parent table
    /// </summary>
    public string PrimaryKeyColumn { get; set; }

    /// <summary>
    /// Gets or sets bind column name for delete action. If this field is not specified, the default will be the alias "id" for the delete action
    /// </summary>
    public string BindColumnNameActionDelete { get; set; }

    /// <summary>
    /// Gets or set column collection 
    /// </summary>
    public IList<ColumnProperty> ColumnCollection { get; set; }

    #endregion
}