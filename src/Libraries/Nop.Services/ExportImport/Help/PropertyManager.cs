using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.ExportImport.Help
{
    /// <summary>
    /// Class for working with PropertyByName object list
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    public class PropertyManager<T>
    {
        /// <summary>
        /// All properties
        /// </summary>
        private readonly Dictionary<string, PropertyByName<T>> _properties;

        /// <summary>
        /// Catalog settings
        /// </summary>
        private readonly CatalogSettings _catalogSettings;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="properties">All access properties</param>
        /// <param name="catalogSettings">Catalog settings</param>
        public PropertyManager(IEnumerable<PropertyByName<T>> properties, CatalogSettings catalogSettings)
        {
            _properties = new Dictionary<string, PropertyByName<T>>();
            _catalogSettings = catalogSettings;

            var poz = 1;
            foreach (var propertyByName in properties.Where(p => !p.Ignore))
            {
                propertyByName.PropertyOrderPosition = poz;
                poz++;
                _properties.Add(propertyByName.PropertyName, propertyByName);
            }
        }

        /// <summary>
        /// Export objects to XLSX
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="itemsToExport">The objects to export</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the 
        /// </returns>
        public virtual async Task<byte[]> ExportToXlsxAsync(IEnumerable<T> itemsToExport)
        {
            await using var stream = new MemoryStream();
            // ok, we can run the real code of the sample now
            using (var workbook = new XLWorkbook())
            {
                // uncomment this line if you want the XML written out to the outputDir
                //xlPackage.DebugMode = true; 

                // get handles to the worksheets
                var worksheet = workbook.Worksheets.Add(typeof(T).Name);
                var fWorksheet = workbook.Worksheets.Add("DataForFilters");
                fWorksheet.Visibility = XLWorksheetVisibility.VeryHidden;

                //create Headers and format them 
                WriteCaption(worksheet);

                var row = 2;
                foreach (var items in itemsToExport)
                {
                    CurrentObject = items;
                    await WriteToXlsxAsync(worksheet, row++, fWorksheet: fWorksheet);
                }

                workbook.SaveAs(stream);
            }

            CurrentObject = default;
            return stream.ToArray();
        }

        /// <summary>
        /// Current object to access
        /// </summary>
        public T CurrentObject { get; set; }

        /// <summary>
        /// Return property index
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <returns></returns>
        public int GetIndex(string propertyName)
        {
            if (!_properties.ContainsKey(propertyName))
                return -1;

            return _properties[propertyName].PropertyOrderPosition;
        }

        /// <summary>
        /// Remove object by property name
        /// </summary>
        /// <param name="propertyName">Property name</param>
        public void Remove(string propertyName)
        {
            _properties.Remove(propertyName);
        }

        /// <summary>
        /// Write object data to XLSX worksheet
        /// </summary>
        /// <param name="worksheet">Data worksheet</param>
        /// <param name="row">Row index</param>
        /// <param name="cellOffset">Cell offset</param>
        /// <param name="fWorksheet">Filters worksheet</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task WriteToXlsxAsync(IXLWorksheet worksheet, int row, int cellOffset = 0, IXLWorksheet fWorksheet = null)
        {
            if (CurrentObject == null)
                return;

            var xlRrow = worksheet.Row(row);
            xlRrow.Style.Alignment.WrapText = false;
            foreach (var prop in _properties.Values)
            {
                var cell = xlRrow.Cell(prop.PropertyOrderPosition + cellOffset);
                if (prop.IsDropDownCell && _catalogSettings.ExportImportRelatedEntitiesByName)
                {
                    var dropDownElements = prop.GetDropDownElements();
                    if (!dropDownElements.Any())
                    {
                        cell.Value = string.Empty;
                        continue;
                    }

                    cell.Value = prop.GetItemText(await prop.GetProperty(CurrentObject));

                    if (!UseDropdownLists)
                        continue;

                    var validator = cell.DataValidation;
                    validator.InCellDropdown = true;

                    validator.IgnoreBlanks = prop.AllowBlank;

                    if (fWorksheet == null)
                        continue;

                    var fRow = 1;
                    foreach (var dropDownElement in dropDownElements)
                    {
                        var fCell = fWorksheet.Row(fRow++).Cell(prop.PropertyOrderPosition);

                        if (fCell.Value != null && fCell.Value.ToString() == dropDownElement)
                            break;

                        fCell.Value = dropDownElement;
                    }

                    validator.List(fWorksheet.Range(1, prop.PropertyOrderPosition, dropDownElements.Length, prop.PropertyOrderPosition), true);
                }
                else
                {
                    var value = await prop.GetProperty(CurrentObject);
                    if (value is string stringValue)
                    {
                        cell.SetValue(stringValue);
                    }
                    else if (value is char charValue)
                    {
                        cell.SetValue(charValue);
                    }
                    else if (value is Guid guidValue)
                    {
                        cell.SetValue(guidValue);
                    }
                    else if (value is Enum enumValue)
                    {
                        cell.SetValue(enumValue);
                    }
                    else
                    {
                        cell.Value = value;
                    }
                }
                cell.Style.Alignment.WrapText = false;
            }
        }

        /// <summary>
        /// Read object data from XLSX worksheet
        /// </summary>
        /// <param name="worksheet">worksheet</param>
        /// <param name="row">Row index</param>
        /// /// <param name="cellOffset">Cell offset</param>
        public virtual void ReadFromXlsx(IXLWorksheet worksheet, int row, int cellOffset = 0)
        {
            if (worksheet?.Cells() == null)
                return;

            foreach (var prop in _properties.Values)
            {
                prop.PropertyValue = worksheet.Row(row).Cell(prop.PropertyOrderPosition + cellOffset).Value;
            }
        }

        /// <summary>
        /// Write caption (first row) to XLSX worksheet
        /// </summary>
        /// <param name="worksheet">worksheet</param>
        /// <param name="row">Row number</param>
        /// <param name="cellOffset">Cell offset</param>
        public virtual void WriteCaption(IXLWorksheet worksheet, int row = 1, int cellOffset = 0)
        {
            foreach (var caption in _properties.Values)
            {
                var cell = worksheet.Row(row).Cell(caption.PropertyOrderPosition + cellOffset);
                cell.Value = caption;

                SetCaptionStyle(cell);
            }
        }

        /// <summary>
        /// Set caption style to excel cell
        /// </summary>
        /// <param name="cell">Excel cell</param>
        public void SetCaptionStyle(IXLCell cell)
        {
            cell.Style.Fill.PatternType = XLFillPatternValues.Solid;
            cell.Style.Fill.BackgroundColor = XLColor.FromColor(Color.FromArgb(184, 204, 228));
            cell.Style.Font.Bold = true;
        }

        /// <summary>
        /// Count of properties
        /// </summary>
        public int Count => _properties.Count;

        /// <summary>
        /// Get property by name
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public PropertyByName<T> GetProperty(string propertyName)
        {
            return _properties.ContainsKey(propertyName) ? _properties[propertyName] : null;
        }

        /// <summary>
        /// Get property array
        /// </summary>
        public PropertyByName<T>[] GetProperties => _properties.Values.ToArray();

        /// <summary>
        /// Set SelectList
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="list">SelectList</param>
        public void SetSelectList(string propertyName, SelectList list)
        {
            var tempProperty = GetProperty(propertyName);
            if (tempProperty != null)
                tempProperty.DropDownElements = list;
        }

        /// <summary>
        /// Is caption
        /// </summary>
        public bool IsCaption => _properties.Values.All(p => p.IsCaption);

        /// <summary>
        /// Gets a value indicating whether need create dropdown list for export
        /// </summary>
        public bool UseDropdownLists => _catalogSettings.ExportImportUseDropdownlistsForAssociatedEntities && _catalogSettings.ExportImportRelatedEntitiesByName;
    }
}
