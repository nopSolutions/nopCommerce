using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;

namespace Nop.Services.ExportImport.Help
{
    /// <summary>
    /// A helper class to access the property by name
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    public class PropertyByName<T>
    {
        private object _propertyValue;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="func">Feature property access</param>
        /// <param name="ignore">Specifies whether the property should be exported</param>
        public PropertyByName(string propertyName, Func<T, object> func = null, bool ignore = false)
        {
            this.PropertyName = propertyName;
            this.GetProperty = func;
            this.PropertyOrderPosition = 1;
            this.Ignore = ignore;
        }

        /// <summary>
        /// Property order position
        /// </summary>
        public int PropertyOrderPosition { get; set; }

        /// <summary>
        /// Feature property access
        /// </summary>
        public Func<T, object> GetProperty { get; private set; }

        /// <summary>
        /// Property name
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Property value
        /// </summary>
        public object PropertyValue {
            get
            {
                return IsDropDownCell ? GetItemId(_propertyValue) : _propertyValue;
            }
            set
            {
                _propertyValue = value;
            } }

        /// <summary>
        /// Converted property value to Int32
        /// </summary>
        public int IntValue
        {
            get
            {
                int rez;
                if (PropertyValue == null || !int.TryParse(PropertyValue.ToString(), out rez))
                    return default(int);
                return rez;
            }
        }

        /// <summary>
        /// Converted property value to bool
        /// </summary>
        public bool BooleanValue
        {
            get
            {
                bool rez;
                if (PropertyValue == null || !bool.TryParse(PropertyValue.ToString(), out rez))
                    return default(bool);
                return rez;
            }
        }

        /// <summary>
        /// Converted property value to string
        /// </summary>
        public string StringValue
        {
            get
            {
                return PropertyValue == null ? string.Empty : Convert.ToString(PropertyValue);
            }
        }

        /// <summary>
        /// Converted property value to decimal
        /// </summary>
        public decimal DecimalValue
        {
            get
            {
                decimal rez;
                if (PropertyValue == null || !decimal.TryParse(PropertyValue.ToString(), out rez))
                    return default(decimal);
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
                decimal rez;
                if (PropertyValue == null || !decimal.TryParse(PropertyValue.ToString(), out rez))
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
                double rez;
                if (PropertyValue == null || !double.TryParse(PropertyValue.ToString(), out rez))
                    return default(double);
                return rez;
            }
        }

        /// <summary>
        /// Converted property value to DateTime?
        /// </summary>
        public DateTime? DateTimeNullable
        {
            get
            {
                return PropertyValue == null ? null : DateTime.FromOADate(DoubleValue) as DateTime?;
            }
        }

        public override string ToString()
        {
            return PropertyName;
        }

        /// <summary>
        /// Specifies whether the property should be exported
        /// </summary>
        public bool Ignore { get; set; }

        public bool IsDropDownCell
        {
            get { return DropDownElements != null; }
        }

        public string[] GetDropDownElements()
        {
            return  IsDropDownCell ? DropDownElements.Select(ev => ev.Text).ToArray() : new string[0];
        }

        public string GetItemText(object id)
        {
            return DropDownElements.FirstOrDefault(ev => ev.Value == id.ToString()).Return(ev => ev.Text, String.Empty);
        }

        public int GetItemId(object name)
        {
            return DropDownElements.FirstOrDefault(ev => ev.Text.Trim() == name.Return(s => s.ToString(), String.Empty).Trim()).Return(ev => Convert.ToInt32(ev.Value), 0);
        }

        /// <summary>
        /// Elements for a drop-down cell
        /// </summary>
        public SelectList DropDownElements { get; set; }

        /// <summary>
        /// Indicates whether the cell can contain an empty value. Makes sense only for a drop-down cells
        /// </summary>
        public bool AllowBlank { get; set; }

        public bool IsCaption
        {
            get { return PropertyName == StringValue || PropertyName == _propertyValue.ToString(); }
        }
    }
}
