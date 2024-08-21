using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core.Domain.Attributes;
using Nop.Core.Domain.Catalog;
using Nop.Services.Localization;

namespace Nop.Services.Attributes;

/// <summary>
/// Attribute parser
/// </summary>
/// <typeparam name="TAttribute">Type of the attribute (see <see cref="BaseAttribute"/>)</typeparam>
/// <typeparam name="TAttributeValue">Type of the attribute value (see <see cref="BaseAttributeValue"/>)</typeparam>
public partial class AttributeParser<TAttribute, TAttributeValue> : IAttributeParser<TAttribute, TAttributeValue>
    where TAttribute : BaseAttribute
    where TAttributeValue : BaseAttributeValue
{
    #region Fields

    protected readonly IAttributeService<TAttribute, TAttributeValue> _attributeService;
    protected readonly ILocalizationService _localizationService;

    protected readonly string _attributeName;
    protected readonly string _attributeValueName;
    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public AttributeParser(IAttributeService<TAttribute, TAttributeValue> attributeService,
        ILocalizationService localizationService)
    {
        _attributeName = typeof(TAttribute).Name;
        _attributeValueName = typeof(TAttributeValue).Name;

        _attributeService = attributeService;
        _localizationService = localizationService;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets attribute values
    /// </summary>
    /// <param name="valuesStr">string value attribute identifiers</param>
    /// <returns>Attribute values</returns>
    protected virtual async IAsyncEnumerable<TAttributeValue> GetValuesAsync(IList<string> valuesStr)
    {
        foreach (var valueStr in valuesStr)
        {
            if (string.IsNullOrEmpty(valueStr))
                continue;

            if (!int.TryParse(valueStr, out var id))
                continue;

            var value = await _attributeService.GetAttributeValueByIdAsync(id);
            if (value != null)
                yield return value;
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets selected attribute identifiers
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <returns>Selected attribute identifiers</returns>
    public virtual IEnumerable<int> ParseAttributeIds(string attributesXml)
    {
        var ids = new List<int>();
        if (string.IsNullOrEmpty(attributesXml))
            return ids;

        try
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(attributesXml);

            var nodes = xmlDoc.SelectNodes(@$"//Attributes/{_attributeName}");

            if (nodes == null)
                return Enumerable.Empty<int>();

            foreach (XmlNode node in nodes)
            {
                if (node.Attributes?["ID"] == null)
                    continue;

                var str1 = node.Attributes["ID"].InnerText.Trim();

                if (int.TryParse(str1, out var id))
                    ids.Add(id);
            }
        }
        catch
        {
            //ignore
        }

        return ids;
    }

    /// <summary>
    /// Gets selected attributes
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the selected attributes
    /// </returns>
    public virtual async Task<IList<TAttribute>> ParseAttributesAsync(string attributesXml)
    {
        var result = new List<TAttribute>();
        if (string.IsNullOrEmpty(attributesXml))
            return result;

        var ids = ParseAttributeIds(attributesXml);
        foreach (var id in ids)
        {
            var attribute = await _attributeService.GetAttributeByIdAsync(id);
            if (attribute != null)
                result.Add(attribute);
        }

        return result;
    }

    /// <summary>
    /// Remove an attribute
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="attributeId">Attribute identifier</param>
    /// <returns>Updated result (XML format)</returns>
    public virtual string RemoveAttribute(string attributesXml, int attributeId)
    {
        var result = string.Empty;

        if (string.IsNullOrEmpty(attributesXml))
            return string.Empty;

        try
        {
            var xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(attributesXml);

            var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes");

            if (rootElement == null)
                return string.Empty;

            XmlElement attributeElement = null;
            //find existing
            var childNodes = xmlDoc.SelectNodes($@"//Attributes/{_attributeName}");

            if (childNodes == null)
                return string.Empty;

            var count = childNodes.Count;

            foreach (XmlElement childNode in childNodes)
            {
                if (!int.TryParse(childNode.Attributes["ID"]?.InnerText.Trim(), out var id))
                    continue;

                if (id != attributeId)
                    continue;

                attributeElement = childNode;
                break;
            }

            //found
            if (attributeElement != null)
            {
                rootElement.RemoveChild(attributeElement);
                count -= 1;
            }

            result = count == 0 ? string.Empty : xmlDoc.OuterXml;
        }
        catch
        {
            //ignore
        }

        return result;
    }

    /// <summary>
    /// Gets selected attribute value
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="attributeId">Attribute identifier</param>
    /// <returns>Attribute value</returns>
    public virtual IList<string> ParseValues(string attributesXml, int attributeId)
    {
        var selectedAddressAttributeValues = new List<string>();
        if (string.IsNullOrEmpty(attributesXml))
            return selectedAddressAttributeValues;

        try
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(attributesXml);

            var nodeList1 = xmlDoc.SelectNodes(@$"//Attributes/{_attributeName}");
            if (nodeList1 == null)
                return new List<string>();

            foreach (XmlNode node1 in nodeList1)
            {
                if (node1.Attributes?["ID"] == null)
                    continue;

                var str1 = node1.Attributes["ID"].InnerText.Trim();
                if (!int.TryParse(str1, out var id))
                    continue;

                if (id != attributeId)
                    continue;

                var nodeList2 = node1.SelectNodes(@$"{_attributeValueName}/Value");

                if (nodeList2 == null)
                    continue;

                foreach (XmlNode node2 in nodeList2)
                {
                    var value = node2.InnerText.Trim();
                    selectedAddressAttributeValues.Add(value);
                }
            }
        }
        catch
        {
            //ignore
        }

        return selectedAddressAttributeValues;
    }

    /// <summary>
    /// Get attribute values
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the attribute values
    /// </returns>
    public virtual async Task<IList<TAttributeValue>> ParseAttributeValuesAsync(string attributesXml)
    {
        var values = new List<TAttributeValue>();

        if (string.IsNullOrEmpty(attributesXml))
            return values;

        var attributes = await ParseAttributesAsync(attributesXml);
        foreach (var attribute in attributes)
        {
            if (!attribute.ShouldHaveValues)
                continue;

            var valuesStr = ParseValues(attributesXml, attribute.Id);

            values.AddRange(await GetValuesAsync(valuesStr).ToArrayAsync());
        }

        return values;
    }

    /// <summary>
    /// Get attribute values
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <returns>Attribute values</returns>
    public virtual async IAsyncEnumerable<(TAttribute attribute, IAsyncEnumerable<TAttributeValue> values)> ParseAttributeValues(string attributesXml)
    {
        if (string.IsNullOrEmpty(attributesXml))
            yield break;

        var attributes = await ParseAttributesAsync(attributesXml);

        foreach (var attribute in attributes)
        {
            if (!attribute.ShouldHaveValues)
                continue;

            var valuesStr = ParseValues(attributesXml, attribute.Id);

            yield return (attribute, GetValuesAsync(valuesStr));
        }
    }

    /// <summary>
    /// Adds an attribute
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="attribute">Attribute</param>
    /// <param name="value">Attribute value</param>
    /// <returns>Attributes</returns>
    public virtual string AddAttribute(string attributesXml, TAttribute attribute, string value)
    {
        var result = string.Empty;
        try
        {
            var xmlDoc = new XmlDocument();
            if (string.IsNullOrEmpty(attributesXml))
                xmlDoc.AppendChild(xmlDoc.CreateElement("Attributes"));
            else
                xmlDoc.LoadXml(attributesXml);

            var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes")!;

            XmlElement attributeElement = null;
            //find existing
            var nodeList1 = xmlDoc.SelectNodes(@$"//Attributes/{_attributeName}");

            if (nodeList1 != null)
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes?["ID"] == null)
                        continue;

                    var str1 = node1.Attributes["ID"].InnerText.Trim();
                    if (!int.TryParse(str1, out var id))
                        continue;

                    if (id != attribute.Id)
                        continue;

                    attributeElement = (XmlElement)node1;
                    break;
                }

            //create new one if not found
            if (attributeElement == null)
            {
                attributeElement = xmlDoc.CreateElement(_attributeName);
                attributeElement.SetAttribute("ID", attribute.Id.ToString());
                rootElement.AppendChild(attributeElement);
            }

            var attributeValueElement = xmlDoc.CreateElement(_attributeValueName);
            attributeElement.AppendChild(attributeValueElement);

            var attributeValueValueElement = xmlDoc.CreateElement("Value");
            attributeValueValueElement.InnerText = value;
            attributeValueElement.AppendChild(attributeValueValueElement);

            result = xmlDoc.OuterXml;
        }
        catch
        {
            //ignore
        }

        return result;
    }

    /// <summary>
    /// Validates attributes
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warnings
    /// </returns>
    public virtual async Task<IList<string>> GetAttributeWarningsAsync(string attributesXml)
    {
        var warnings = new List<string>();

        //ensure it's our attributes
        var attributes1 = await ParseAttributesAsync(attributesXml);

        //validate required attributes (whether they're chosen/selected/entered)
        var attributes2 = await _attributeService.GetAllAttributesAsync();

        foreach (var a2 in attributes2)
        {
            if (!a2.IsRequired)
                continue;

            var found = false;
            //selected attributes
            foreach (var a1 in attributes1)
            {
                if (a1.Id != a2.Id)
                    continue;

                var valuesStr = ParseValues(attributesXml, a1.Id);

                found = valuesStr.Any(str1 => !string.IsNullOrEmpty(str1.Trim()));
            }

            if (found)
                continue;

            //if not found
            var notFoundWarning = string.Format(await _localizationService.GetResourceAsync("ShoppingCart.SelectAttribute"), await _localizationService.GetLocalizedAsync(a2, a => a.Name));

            warnings.Add(notFoundWarning);
        }

        return warnings;
    }

    /// <summary>
    /// Get custom attributes from the passed form
    /// </summary>
    /// <param name="form">Form values</param>
    /// <param name="attributeControlName">Name of the attribute control</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the attributes in XML format
    /// </returns>
    public virtual async Task<string> ParseCustomAttributesAsync(IFormCollection form, string attributeControlName)
    {
        ArgumentNullException.ThrowIfNull(form);

        var attributesXml = string.Empty;

        foreach (var attribute in await _attributeService.GetAllAttributesAsync())
        {
            var controlId = string.Format(attributeControlName, attribute.Id);
            var attributeValues = form[controlId];
            switch (attribute.AttributeControlType)
            {
                case AttributeControlType.DropdownList:
                case AttributeControlType.RadioList:
                    if (!StringValues.IsNullOrEmpty(attributeValues) && int.TryParse(attributeValues, out var value) && value > 0)
                        attributesXml = AddAttribute(attributesXml, attribute, value.ToString());
                    break;

                case AttributeControlType.Checkboxes:
                    foreach (var attributeValue in attributeValues.ToString().Split(_separator, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!int.TryParse(attributeValue, out value) || value <= 0)
                            continue;

                        attributesXml = AddAttribute(attributesXml, attribute, value.ToString());
                    }

                    break;

                case AttributeControlType.ReadonlyCheckboxes:
                    //load read-only (already server-side selected) values
                    var readOnlyAttributeValues = await _attributeService.GetAttributeValuesAsync(attribute.Id);
                    foreach (var readOnlyAttributeValue in readOnlyAttributeValues)
                    {
                        if (!readOnlyAttributeValue.IsPreSelected)
                            continue;

                        attributesXml = AddAttribute(attributesXml, attribute, readOnlyAttributeValue.Id.ToString());
                    }

                    break;

                case AttributeControlType.TextBox:
                case AttributeControlType.MultilineTextbox:
                    if (!StringValues.IsNullOrEmpty(attributeValues))
                        attributesXml = AddAttribute(attributesXml, attribute, attributeValues.ToString().Trim());
                    break;

                case AttributeControlType.Datepicker:
                case AttributeControlType.ColorSquares:
                case AttributeControlType.ImageSquares:
                case AttributeControlType.FileUpload:
                default:
                    break;
            }
        }

        return attributesXml;
    }

    /// <summary>
    /// Check whether condition of some attribute is met (if specified). Return "null" if not condition is specified
    /// </summary>
    /// <param name="conditionAttributeXml">Condition attributes (XML format)</param>
    /// <param name="selectedAttributesXml">Selected attributes (XML format)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public async Task<bool?> IsConditionMetAsync(string conditionAttributeXml, string selectedAttributesXml)
    {
        if (string.IsNullOrEmpty(conditionAttributeXml))
            //no condition
            return null;
        
        //load an attribute this one depends on
        var dependOnAttribute = (await ParseAttributesAsync(conditionAttributeXml)).FirstOrDefault();
        if (dependOnAttribute == null)
            return true;

        var valuesThatShouldBeSelected = ParseValues(conditionAttributeXml, dependOnAttribute.Id)
            //a workaround here:
            //ConditionAttributeXml can contain "empty" values (nothing is selected)
            //but in other cases (like below) we do not store empty values
            //that's why we remove empty values here
            .Where(x => !string.IsNullOrEmpty(x))
            .ToList();
        var selectedValues = ParseValues(selectedAttributesXml, dependOnAttribute.Id);
        if (valuesThatShouldBeSelected.Count != selectedValues.Count)
            return false;

        //compare values
        var allFound = true;
        foreach (var t1 in valuesThatShouldBeSelected)
        {
            var found = false;
            foreach (var t2 in selectedValues)
                if (t1 == t2)
                    found = true;
            if (!found)
                allFound = false;
        }

        return allFound;
    }

    #endregion
}