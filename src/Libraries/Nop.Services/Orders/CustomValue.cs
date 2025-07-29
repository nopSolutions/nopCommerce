namespace Nop.Services.Orders;

/// <summary>
/// Represents the custom value
/// </summary>
public partial class CustomValue
{
    #region Ctor

    public CustomValue(string name, string value, CustomValueDisplayLocation displayLocation = CustomValueDisplayLocation.Payment, bool displayToCustomer = true, DateTime? createdOnUtc = null)
    {
        Name = name;
        Value = value;
        DisplayLocation = displayLocation;
        DisplayToCustomer = displayToCustomer;
        CreatedOnUtc = createdOnUtc;
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
    /// Gets or sets a value indicating whether a customer can see a custom value
    /// </summary>
    public bool DisplayToCustomer { get; set; }

    /// <summary>
    /// Gets or sets a display location of custom value
    /// </summary>
    public CustomValueDisplayLocation DisplayLocation { get; set; }

    /// <summary>
    /// Gets or sets a created on date and time
    /// </summary>
    public DateTime? CreatedOnUtc { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Returns a string that represents the current object
    /// </summary>
    /// <returns>A string that represents the current object</returns>
    public override string ToString()
    {
        return Value;
    }

    #endregion
}