﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Localization;

namespace Nop.Services.ExportImport.Help;

/// <summary>
/// A helper class to access the property by name
/// </summary>
/// <typeparam name="T">Object type</typeparam>
public partial class PropertyByName<T>
{
    protected object _propertyValue;

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="propertyName">Property name</param>
    /// <param name="func">Feature property access</param>
    /// <param name="ignore">Specifies whether the property should be exported</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public PropertyByName(string propertyName, Func<T, Language, Task<object>> func, bool ignore = false)
    {
        PropertyName = propertyName;
        GetProperty = func;
        PropertyOrderPosition = 1;
        Ignore = ignore;
    }

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="propertyName">Property name</param>
    /// <param name="func">Feature property access</param>
    /// <param name="ignore">Specifies whether the property should be exported</param>
    public PropertyByName(string propertyName, Func<T, Language, object> func = null, bool ignore = false)
    {
        PropertyName = propertyName;

        if (func != null)
            GetProperty = (obj, lang) => Task.FromResult(func(obj, lang));

        PropertyOrderPosition = 1;
        Ignore = ignore;
    }

    /// <summary>
    /// Property order position
    /// </summary>
    public int PropertyOrderPosition { get; set; }

    /// <summary>
    /// Feature property access
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public Func<T, Language, Task<object>> GetProperty { get; }

    /// <summary>
    /// Property name
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Property value
    /// </summary>
    public object PropertyValue
    {
        get => IsDropDownCell ? GetItemId(_propertyValue) : _propertyValue;
        set => _propertyValue = value;
    }

    /// <summary>
    /// Converted property value to Int32
    /// </summary>
    public int IntValue
    {
        get
        {
            if (PropertyValue == null || !int.TryParse(PropertyValue.ToString(), out var rez))
                return default;
            return rez;
        }
    }

    /// <summary>
    /// Converted property value to Int32
    /// </summary>
    public int? IntValueNullable
    {
        get
        {
            if (PropertyValue == null || !int.TryParse(PropertyValue.ToString(), out var rez))
                return null;

            return rez;
        }
    }

    /// <summary>
    /// Converted property value to boolean
    /// </summary>
    public bool BooleanValue
    {
        get
        {
            if (PropertyValue == null || !bool.TryParse(PropertyValue.ToString(), out var rez))
                return default;
            return rez;
        }
    }

    /// <summary>
    /// Converted property value to string
    /// </summary>
    public string StringValue => PropertyValue == null ? string.Empty : Convert.ToString(PropertyValue);

    /// <summary>
    /// Converted property value to decimal
    /// </summary>
    public decimal DecimalValue
    {
        get
        {
            if (PropertyValue == null || !decimal.TryParse(PropertyValue.ToString(), out var rez))
                return default;
            return rez;
        }
    }

    /// <summary>
    /// Converted property value to decimal?
    /// </summary>
    public decimal? DecimalValueNullable
    {
        get
        {
            if (PropertyValue == null || !decimal.TryParse(PropertyValue.ToString(), out var rez))
                return null;
            return rez;
        }
    }

    /// <summary>
    /// Converted property value to double
    /// </summary>
    public double DoubleValue
    {
        get
        {
            if (PropertyValue == null || !double.TryParse(PropertyValue.ToString(), out var rez))
                return default;
            return rez;
        }
    }

    /// <summary>
    /// Converted property value to DateTime?
    /// </summary>
    public DateTime? DateTimeNullable => string.IsNullOrWhiteSpace(StringValue) ? null : PropertyValue as DateTime?;

    /// <summary>
    /// Converted property value to guid
    /// </summary>
    public Guid GuidValue
    {
        get
        {
            if (PropertyValue == null || !Guid.TryParse(PropertyValue.ToString(), out var rez))
                return default;
            return rez;
        }
    }

    /// <summary>
    /// To string
    /// </summary>
    /// <returns>String</returns>
    public override string ToString()
    {
        return PropertyName;
    }

    /// <summary>
    /// Specifies whether the property should be exported
    /// </summary>
    public bool Ignore { get; set; }

    /// <summary>
    /// Is drop down cell
    /// </summary>
    public bool IsDropDownCell => DropDownElements != null;

    /// <summary>
    /// Get DropDown elements
    /// </summary>
    /// <returns>Result</returns>
    public string[] GetDropDownElements()
    {
        return IsDropDownCell ? DropDownElements.Select(ev => ev.Text).ToArray() : Array.Empty<string>();
    }

    /// <summary>
    /// Get item text
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <returns>Text</returns>
    public string GetItemText(object id)
    {
        return DropDownElements.FirstOrDefault(ev => ev.Value == id.ToString())?.Text ?? string.Empty;
    }

    /// <summary>
    /// Get item identifier
    /// </summary>
    /// <param name="name">Name</param>
    /// <returns>Identifier</returns>
    public int GetItemId(object name)
    {
        if (string.IsNullOrEmpty(name?.ToString()))
            return 0;

        if (!int.TryParse(name.ToString(), out var id))
            id = 0;

        return Convert.ToInt32(DropDownElements.FirstOrDefault(ev => ev.Text.Trim() == name.ToString().Trim())?.Value ?? id.ToString());
    }

    /// <summary>
    /// Elements for a drop-down cell
    /// </summary>
    public SelectList DropDownElements { get; set; }

    /// <summary>
    /// Indicates whether the cell can contain an empty value. Makes sense only for a drop-down cells
    /// </summary>
    public bool AllowBlank { get; set; }

    /// <summary>
    /// Is caption
    /// </summary>
    public bool IsCaption => PropertyName == StringValue || PropertyName == _propertyValue.ToString();
}