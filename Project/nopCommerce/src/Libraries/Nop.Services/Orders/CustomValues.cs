using System.Xml.Linq;

namespace Nop.Services.Orders;

/// <summary>
/// Represents the list of custom value
/// </summary>
public partial class CustomValues : List<CustomValue>
{
    #region Methods

    /// <summary>
    /// Get custom values by specified display location
    /// </summary>
    /// <param name="displayLocation">Display location</param>
    /// <returns>List of custom values filtered by display location</returns>
    public virtual List<CustomValue> GetValuesByDisplayLocation(CustomValueDisplayLocation displayLocation)
    {
        return this.Where(cv => cv.DisplayLocation == displayLocation).ToList();
    }

    /// <summary>
    /// Serialize list of custom values into XML format
    /// </summary>
    /// <returns>XML of custom values</returns>
    public virtual string SerializeToXml()
    {
        if (!this.Any())
            return string.Empty;

        using var textWriter = new StringWriter();

        var root = new XElement("DictionarySerializer");
        root.Add(this.Select(value => new XElement("item",
            new XElement("key", value.Name),
            new XElement("value", value.Value),
            new XElement("displayToCustomer", value.DisplayToCustomer),
            new XElement("location", (int)value.DisplayLocation),
            new XElement("createdOn", value.CreatedOnUtc?.Ticks ?? 0))));

        var document = new XDocument();
        document.Add(root);
        document.Save(textWriter, SaveOptions.DisableFormatting);

        var result = textWriter.ToString();

        return result;
    }

    /// <summary>
    /// Fill order custom values by XML
    /// </summary>
    /// <param name="customValuesXml">XML of custom values</param>
    /// <param name="displayToCustomerOnly">The flag indicates whether we should use only values that are allowed for customers</param>
    /// <returns>Custom values</returns>
    public virtual void FillByXml(string customValuesXml, bool displayToCustomerOnly = false)
    {
        Clear();

        if (string.IsNullOrWhiteSpace(customValuesXml))
            return;

        //use the 'Payment' value as default for compatibility with previous versions
        var defaultLocation = CustomValueDisplayLocation.Payment;

        //display a custom value by default for compatibility with previous versions
        var defaultDisplayToCustomer = true;

        try
        {
            using var textReader = new StringReader(customValuesXml);
            var rootElement = XDocument.Load(textReader).Root;
            AddRange(rootElement!.Elements("item").Select(i =>
            {
                var name = i.Element("key")!.Value;
                var value = i.Element("value")!.Value;
                var displayLocation = int.TryParse(i.Element("location")?.Value, out var location) && Enum.IsDefined(typeof(CustomValueDisplayLocation), location)
                    ? (CustomValueDisplayLocation)location
                    : defaultLocation;
                var displayToCustomer = bool.TryParse(i.Element("displayToCustomer")?.Value, out var display)
                    ? display
                    : defaultDisplayToCustomer;
                var createdOn = long.TryParse(i.Element("createdOn")?.Value, out var ticks)
                    ? (ticks > 0 ? new DateTime(ticks) : (DateTime?)null)
                    : null;

                return new CustomValue(name, value, displayLocation, displayToCustomer, createdOn);
            }).Where(value => value.DisplayToCustomer || !displayToCustomerOnly));
        }
        catch
        {
            //ignore
        }
    }

    /// <summary>
    /// Removes the custom value with the specified name
    /// </summary>
    /// <param name="name">The name of the custom value.</param>
    /// <returns>
    /// <see langword="true" /> if the element is successfully removed; otherwise, <see langword="false" />.  This method also returns <see langword="false" /> if <paramref name="name" /> was not found.
    /// </returns>
    public virtual bool Remove(string name)
    {
        return TryGetValue(name, out var value) && Remove(value);
    }

    /// <summary>
    /// Gets the value associated with the specified name
    /// </summary>
    /// <param name="name">The name whose value to get.</param>
    /// <param name="customValue">When this method returns, the value associated with the specified name, if the name is found; otherwise, the null value</param>
    /// <returns>
    /// <see langword="true" /> if the custom values list contains an element with the specified name; otherwise, <see langword="false" />.
    /// </returns>
    public virtual bool TryGetValue(string name, out CustomValue customValue)
    {
        customValue = this.FirstOrDefault(cv => cv.Name.Equals(name));

        return customValue != null;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the value of custom value with the specified name
    /// </summary>
    /// <param name="name">The name of the custom value to get or set</param>
    /// <returns>The value of custom value with the specified name</returns>
    public virtual string this[string name]
    {
        get
        {
            var value = this.First(cv => cv.Name.Equals(name));

            return value.Value;
        }
        set
        {
            Remove(name);
            Add(new CustomValue(name, value));
        }
    }

    #endregion
}