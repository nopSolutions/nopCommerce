namespace Nop.Web.Framework.Models.DataTables;

/// <summary>
/// Represents button render for DataTables column
/// </summary>
public partial class RenderButtonsInlineEdit : IRender
{
    #region Ctor

    /// <summary>
    /// Initializes a new instance of the RenderButton class 
    /// </summary>
    public RenderButtonsInlineEdit()
    {
        ClassName = NopButtonClassDefaults.Default;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets button class name
    /// </summary>
    public string ClassName { get; set; }

    #endregion
}