using System;
using System.Collections.Generic;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Nop.Services.ExportImport.Help
{
    /// <summary>
    /// Class for working with list objects PropertyByName
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    public class PropertyManager<T>
    {
        /// <summary>
        /// Curent object to acsess
        /// </summary>
        public T CurentObject { get; set; }

        /// <summary>
        /// All properties
        /// </summary>
        private readonly Dictionary<string, PropertyByName<T>> _propertys;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="propertys">All acsess properties</param>
        public PropertyManager(params PropertyByName<T>[] propertys)
        {
            _propertys=new Dictionary<string, PropertyByName<T>>();

            var poz = 1;
            foreach (var propertyByName in propertys)
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
        /// Access object by key
        /// </summary>
        /// <param name="properyName"></param>
        /// <returns></returns>
        public object this[string properyName] => _propertys.ContainsKey(properyName) && CurentObject != null
            ? _propertys[properyName].GetProperty(CurentObject)
            : null;

        /// <summary>
        /// Write object data to XLSX worksheet
        /// </summary>
        /// <param name="worksheet">worksheet</param>
        /// <param name="row">Row index</param>
        public void WriteToXlsx(ExcelWorksheet worksheet, int row)
        {
            if(CurentObject==null)
                return;

            foreach (var prop in _propertys.Values)
            {
                worksheet.Cells[row, prop.PropertyOrderPosition].Value = prop.GetProperty(CurentObject);
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
    }
}
