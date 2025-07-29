using System.Xml.Linq;

namespace Nop.Services.Common;

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
        root.Add(this.Select(p => new XElement("item", new XElement("key", p.Name), new XElement("value", p.Value), new XElement("displayToCustomer", p.DisplayToCustomer), new XElement("location", (int)p.DisplayLocation))));

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
    /// <returns>Custom values</returns>
    public virtual void FillByXml(string customValuesXml)
    {
        Clear();

        if (string.IsNullOrWhiteSpace(customValuesXml))
            return;

        try
        {
            using var textReader = new StringReader(customValuesXml);
            var rootElement = XDocument.Load(textReader).Root;
            AddRange(rootElement!.Elements("item")
                .Select(i =>
                {
                    var name = i.Element("key")!.Value;
                    var value = i.Element("value")!.Value;
                    var displayLocation = int.TryParse(i.Element("location")?.Value, out var location)
                        ? (CustomValueDisplayLocation)location
                        : CustomValueDisplayLocation.Payment;
                    var displayToCustomer = !bool.TryParse(i.Element("displayToCustomer")?.Value, out var display) || display;

                    return new CustomValue(name, value, displayLocation, displayToCustomer);
                }));
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