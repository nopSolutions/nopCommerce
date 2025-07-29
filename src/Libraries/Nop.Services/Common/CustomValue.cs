namespace Nop.Services.Common;

/// <summary>
/// Represents the custom value
/// </summary>
public partial class CustomValue
{
    #region Ctor

    public CustomValue(string name, string value, CustomValueDisplayLocation displayLocation = CustomValueDisplayLocation.Payment, bool displayToCustomer = true)
    {
        Name = name;
        Value = value;
        DisplayLocation = displayLocation;
        DisplayToCustomer = displayToCustomer;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets a value of custom value
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Gets or sets a name of custom value
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a customer can see an custom value
    /// </summary>
    public bool DisplayToCustomer { get; set; }

    /// <summary>
    /// Gets or sets a display location of custom value
    /// </summary>
    public CustomValueDisplayLocation DisplayLocation { get; set; }

    #endregion
}