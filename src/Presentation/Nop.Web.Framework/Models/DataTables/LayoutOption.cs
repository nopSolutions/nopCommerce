namespace Nop.Web.Framework.Models.DataTables;

/// <summary>
/// Represent DataTables layout option
/// </summary>
public partial class LayoutOption
{
    /// <summary>
    /// Option name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Option type
    /// </summary>
    public LayoutOptionType OptionType { get; set; }

    /// <summary>
    /// Option value
    /// </summary>
    public object Value { get; set; }
}
