using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Nop.Services.ExportImport.Help
{
    /// <summary>
    /// Class for working with PropertyByName object list
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    public class PropertyManager<T>
    {
        /// <summary>
        /// Curent object to acsess
        /// </summary>
        public T CurrentObject { get; set; }

        /// <summary>
        /// All properties
        /// </summary>
        private readonly Dictionary<string, PropertyByName<T>> _propertys;
        private readonly IGetProperties<T> _fill_object;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="propertys">All acsess properties</param>
        public PropertyManager(IGetProperties<T> propertys)
        {
            _propertys=new Dictionary<string, PropertyByName<T>>();
            _fill_object = propertys;

            var poz = 1;
            foreach (var propertyByName in propertys.GetProperties)
            {
                propertyByName.PropertyOrderPosition = poz;
                poz++;
                _propertys.Add(propertyByName.PropertyName, propertyByName);
            }
        }

        /// <summary>
        /// Return properti index
        /// </summary>
        /// <param name="properyName">Property name</param>
        /// <returns></returns>
        public int GetIndex(string properyName)
        {
            if (!_propertys.ContainsKey(properyName))
                return -1;

            return _propertys[properyName].PropertyOrderPosition;
        }

        /// <summary>
        /// Access object by property name
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <returns>Property value</returns>
        public object this[string propertyName]
        {
            get
            {
                return _propertys.ContainsKey(propertyName) && CurrentObject != null
                    ? _propertys[propertyName].GetProperty(CurrentObject)
                    : null;
            }
        }

        /// <summary>
        /// Write object data to XLSX worksheet
        /// </summary>
        /// <param name="worksheet">worksheet</param>
        /// <param name="row">Row index</param>
        public void WriteToXlsx(ExcelWorksheet worksheet, int row)
        {
            if (CurrentObject == null)
                return;

            foreach (var prop in _propertys.Values)
            {
                worksheet.Cells[row, prop.PropertyOrderPosition].Value = prop.GetProperty(CurrentObject);
            }
        }

        /// <summary>
        /// Read object data from XLSX worksheet
        /// </summary>
        /// <param name="worksheet">worksheet</param>
        /// <param name="row">Row index</param>
        public void ReadFromXlsx(ExcelWorksheet worksheet, int row)
        {
            if (worksheet == null || worksheet.Cells == null)
                return;

            foreach (var prop in _propertys.Values)
            {
                prop.PropertyValue = worksheet.Cells[row, prop.PropertyOrderPosition].Value;
            }
        }

        /// <summary>
        /// Write caption (first row) to XLSX worksheet
        /// </summary>
        /// <param name="worksheet">worksheet</param>
        /// <param name="setStyle">Detection of cell style</param>
        public void WriteCaption(ExcelWorksheet worksheet, Action<ExcelStyle> setStyle)
        {
            foreach (var caption in _propertys.Values)
            {
                var cell = worksheet.Cells[1, caption.PropertyOrderPosition];
                cell.Value = caption;
                setStyle(cell.Style);
            }
            
        }

        /// <summary>
        /// Count of properties
        /// </summary>
        public int Count
        {
            get { return _propertys.Count; }
        }

        /// <summary>
        /// Get property by name
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public PropertyByName<T> GetProperty(string propertyName)
        {
            return _propertys.ContainsKey(propertyName) ? _propertys[propertyName] : null;
        }


        /// <summary>
        /// Get property array
        /// </summary>
        public PropertyByName<T>[] GetProperties
        {
            get { return _propertys.Values.ToArray(); }
        }

        public void FillObject(BaseEntity objectToFill, bool isNew, PropertyManager<T> manager)
        {
            _fill_object.FillObject(objectToFill, isNew, this);
        }
    }
}
