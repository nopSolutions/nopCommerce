using System.Drawing;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;

namespace Nop.Services.ExportImport.Help
{
    /// <summary>
    /// Class for working with PropertyByName object list
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    /// <typeparam name="L">Language</typeparam>
    public partial class PropertyManager<T, L> where L : Language
    {
        /// <summary>
        /// Default properties
        /// </summary>
        protected readonly Dictionary<string, PropertyByName<T, L>> _defaultProperties;

        /// <summary>
        /// Localized properties
        /// </summary>
        protected readonly Dictionary<string, PropertyByName<T, L>> _localizedProperties;

        /// <summary>
        /// Catalog settings
        /// </summary>
        protected readonly CatalogSettings _catalogSettings;

        /// <summary>
        /// Languages
        /// </summary>
        protected readonly IList<L> _languages;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="defaultProperties">Default access properties</param>
        /// <param name="catalogSettings">Catalog settings</param>
        /// <param name="localizedProperties">Localized access properties</param>
        /// <param name="languages">Languages</param>
        public PropertyManager(IEnumerable<PropertyByName<T, L>> defaultProperties, CatalogSettings catalogSettings, IEnumerable<PropertyByName<T, L>> localizedProperties = null, IList<L> languages = null)
        {
            _defaultProperties = new Dictionary<string, PropertyByName<T, L>>();
            _catalogSettings = catalogSettings;
            _localizedProperties = new Dictionary<string, PropertyByName<T, L>>();
            _languages = new List<L>();

            if (languages != null)
                _languages = languages;

            var poz = 1;
            foreach (var propertyByName in defaultProperties.Where(p => !p.Ignore))
            {
                propertyByName.PropertyOrderPosition = poz;
                poz++;
                _defaultProperties.Add(propertyByName.PropertyName, propertyByName);
            }

            if (_languages.Count >= 2 && localizedProperties != null)
            {
                var lpoz = 1;
                foreach (var propertyByName in localizedProperties.Where(p => !p.Ignore))
                {
                    propertyByName.PropertyOrderPosition = lpoz;
                    lpoz++;
                    _localizedProperties.Add(propertyByName.PropertyName, propertyByName);
                }
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
                WriteDefaultCaption(worksheet);

                var lwss = new Dictionary<L, IXLWorksheet>();

                if (_languages.Count >= 2)
                {
                    foreach (var language in _languages)
                    {
                        var lws = workbook.Worksheets.Add(language.UniqueSeoCode);
                        lwss.Add(language, lws);
                        WriteLocalizedCaption(lws);
                    }
                }

                var row = 2;
                foreach (var items in itemsToExport)
                {
                    CurrentObject = items;
                    await WriteDefaultToXlsxAsync(worksheet, row, fWorksheet: fWorksheet);

                    foreach (var lws in lwss)
                    {
                        CurrentLanguage = lws.Key;
                        await WriteLocalizedToXlsxAsync(lws.Value, row, fWorksheet: fWorksheet);
                    }

                    row++;
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
        /// Current language to access
        /// </summary>
        public L CurrentLanguage { get; set; }

        /// <summary>
        /// Return property index
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <returns></returns>
        public int GetIndex(string propertyName)
        {
            if (!_defaultProperties.ContainsKey(propertyName))
                return -1;

            return _defaultProperties[propertyName].PropertyOrderPosition;
        }

        /// <summary>
        /// Remove object by property name
        /// </summary>
        /// <param name="propertyName">Property name</param>
        public void Remove(string propertyName)
        {
            _defaultProperties.Remove(propertyName);
        }

        /// <summary>
        /// Write default object data to XLSX worksheet
        /// </summary>
        /// <param name="worksheet">Data worksheet</param>
        /// <param name="row">Row index</param>
        /// <param name="cellOffset">Cell offset</param>
        /// <param name="fWorksheet">Filters worksheet</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task WriteDefaultToXlsxAsync(IXLWorksheet worksheet, int row, int cellOffset = 0, IXLWorksheet fWorksheet = null)
        {
            if (CurrentObject == null)
                return;

            var xlRrow = worksheet.Row(row);
            xlRrow.Style.Alignment.WrapText = false;
            foreach (var prop in _defaultProperties.Values)
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

                    cell.Value = prop.GetItemText(await prop.GetProperty(CurrentObject, CurrentLanguage));

                    if (!UseDropdownLists)
                        continue;

                    var validator = cell.GetDataValidation();
                    validator.InCellDropdown = true;

                    validator.IgnoreBlanks = prop.AllowBlank;

                    if (fWorksheet == null)
                        continue;

                    var fRow = 1;
                    foreach (var dropDownElement in dropDownElements)
                    {
                        var fCell = fWorksheet.Row(fRow++).Cell(prop.PropertyOrderPosition);

                        if (!fCell.IsEmpty() && fCell.Value.ToString() == dropDownElement)
                            break;

                        fCell.Value = dropDownElement;
                    }

                    validator.List(fWorksheet.Range(1, prop.PropertyOrderPosition, dropDownElements.Length, prop.PropertyOrderPosition), true);
                }
                else
                {
                    var value = await prop.GetProperty(CurrentObject, CurrentLanguage);
                    cell.SetValue((XLCellValue)value?.ToString());
                }
                cell.Style.Alignment.WrapText = false;
            }
        }

        /// <summary>
        /// Write localized data to XLSX worksheet
        /// </summary>
        /// <param name="worksheet">Data worksheet</param>
        /// <param name="row">Row index</param>
        /// <param name="cellOffset">Cell offset</param>
        /// <param name="fWorksheet">Filters worksheet</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task WriteLocalizedToXlsxAsync(IXLWorksheet worksheet, int row, int cellOffset = 0, IXLWorksheet fWorksheet = null)
        {
            if (CurrentObject == null)
                return;

            var xlRrow = worksheet.Row(row);
            xlRrow.Style.Alignment.WrapText = false;
            foreach (var prop in _localizedProperties.Values)
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

                    cell.Value = prop.GetItemText(await prop.GetProperty(CurrentObject, CurrentLanguage));

                    if (!UseDropdownLists)
                        continue;

                    var validator = cell.GetDataValidation();
                    validator.InCellDropdown = true;

                    validator.IgnoreBlanks = prop.AllowBlank;

                    if (fWorksheet == null)
                        continue;

                    var fRow = 1;
                    foreach (var dropDownElement in dropDownElements)
                    {
                        var fCell = fWorksheet.Row(fRow++).Cell(prop.PropertyOrderPosition);

                        if (!fCell.IsEmpty() && fCell.Value.ToString() == dropDownElement)
                            break;

                        fCell.Value = dropDownElement;
                    }

                    validator.List(fWorksheet.Range(1, prop.PropertyOrderPosition, dropDownElements.Length, prop.PropertyOrderPosition), true);
                }
                else
                {
                    var value = await prop.GetProperty(CurrentObject, CurrentLanguage);
                    cell.SetValue((XLCellValue)value?.ToString());
                }
                cell.Style.Alignment.WrapText = false;
            }
        }

        /// <summary>
        /// Read object data from default XLSX worksheet
        /// </summary>
        /// <param name="worksheet">worksheet</param>
        /// <param name="row">Row index</param>
        /// /// <param name="cellOffset">Cell offset</param>
        public virtual void ReadDefaultFromXlsx(IXLWorksheet worksheet, int row, int cellOffset = 0)
        {
            if (worksheet?.Cells() == null)
                return;

            foreach (var prop in _defaultProperties.Values)
            {
                prop.PropertyValue = worksheet.Row(row).Cell(prop.PropertyOrderPosition + cellOffset).Value;
            }
        }

        /// <summary>
        /// Read object data from localized XLSX worksheet
        /// </summary>
        /// <param name="worksheet">worksheet</param>
        /// <param name="row">Row index</param>
        /// /// <param name="cellOffset">Cell offset</param>
        public virtual void ReadLocalizedFromXlsx(IXLWorksheet worksheet, int row, int cellOffset = 0)
        {
            if (worksheet?.Cells() == null)
                return;

            foreach (var prop in _localizedProperties.Values)
            {
                prop.PropertyValue = worksheet.Row(row).Cell(prop.PropertyOrderPosition + cellOffset).Value;
            }
        }

        /// <summary>
        /// Write caption (first row) to default XLSX worksheet
        /// </summary>
        /// <param name="worksheet">worksheet</param>
        /// <param name="row">Row number</param>
        /// <param name="cellOffset">Cell offset</param>
        public virtual void WriteDefaultCaption(IXLWorksheet worksheet, int row = 1, int cellOffset = 0)
        {
            foreach (var caption in _defaultProperties.Values)
            {
                var cell = worksheet.Row(row).Cell(caption.PropertyOrderPosition + cellOffset);
                cell.Value = caption.ToString();

                SetCaptionStyle(cell);
            }
        }

        /// <summary>
        /// Write caption (first row) to localized XLSX worksheet
        /// </summary>
        /// <param name="worksheet">worksheet</param>
        /// <param name="row">Row number</param>
        /// <param name="cellOffset">Cell offset</param>
        public virtual void WriteLocalizedCaption(IXLWorksheet worksheet, int row = 1, int cellOffset = 0)
        {
            foreach (var caption in _localizedProperties.Values)
            {
                var cell = worksheet.Row(row).Cell(caption.PropertyOrderPosition + cellOffset);
                cell.Value = caption.ToString();

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
        /// Count of default properties
        /// </summary>
        public int Count => _defaultProperties.Count;

        /// <summary>
        /// Get default property by name
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public PropertyByName<T, L> GetDefaultProperty(string propertyName)
        {
            return _defaultProperties.TryGetValue(propertyName, out var value) ? value : null;
        }

        /// <summary>
        /// Get localized property by name
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public PropertyByName<T, L> GetLocalizedProperty(string propertyName)
        {
            return _localizedProperties.TryGetValue(propertyName, out var value) ? value : null;
        }

        /// <summary>
        /// Get default property array
        /// </summary>
        public PropertyByName<T, L>[] GetDefaultProperties => _defaultProperties.Values.ToArray();

        /// <summary>
        /// Get localized property array
        /// </summary>
        public PropertyByName<T, L>[] GetLocalizedProperties => _localizedProperties.Values.ToArray();

        /// <summary>
        /// Set SelectList
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="list">SelectList</param>
        public void SetSelectList(string propertyName, SelectList list)
        {
            var tempProperty = GetDefaultProperty(propertyName);
            if (tempProperty != null)
                tempProperty.DropDownElements = list;
        }

        /// <summary>
        /// Is caption
        /// </summary>
        public bool IsCaption => _defaultProperties.Values.All(p => p.IsCaption);

        /// <summary>
        /// Gets a value indicating whether need create dropdown list for export
        /// </summary>
        public bool UseDropdownLists => _catalogSettings.ExportImportUseDropdownlistsForAssociatedEntities && _catalogSettings.ExportImportRelatedEntitiesByName;
    }
}
