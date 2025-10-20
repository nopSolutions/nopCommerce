namespace Nop.Web.Framework.Models.DataTables;

/// <summary>
/// Represents type of the parameters in layout option
/// </summary>
public enum LayoutOptionType
{
    /// <summary>
    /// An array of any options, providing the ability to show multiple items next to each other.
    /// </summary>
    Array = 1,
    
    /// <summary>
    /// Show nothing in this position.
    /// </summary>
    Null = 2,

    /// <summary>
    /// A string that represents a feature provided by DataTables.
    /// </summary>
    String = 3,

    /// <summary>
    /// A plain object where the parameter keys are the feature to be used and the value is passed to the feature. 
    /// This is normally an object with a list of options.
    /// </summary>
    Object = 4
}
