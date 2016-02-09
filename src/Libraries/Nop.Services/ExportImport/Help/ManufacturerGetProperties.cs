using Nop.Core.Domain.Catalog;
using Nop.Services.Media;
using System;
using Nop.Core;

namespace Nop.Services.ExportImport.Help
{
    /// <summary>
    /// Class management manufacturer using the properties
    /// </summary>
    public class ManufacturerGetProperties:BaseGetProperties, IGetProperties<Manufacturer>
    {
        public ManufacturerGetProperties(IPictureService pictureService) : base(pictureService)
        {
        }

        /// <summary>
        /// Get property array
        /// </summary>
        public PropertyByName<Manufacturer>[] GetPropertys
        {
            get
            {
                return new[]
                {
                    new PropertyByName<Manufacturer>("Id", p => p.Id),
                    new PropertyByName<Manufacturer>("Name", p => p.Name),
                    new PropertyByName<Manufacturer>("Description", p => p.Description),
                    new PropertyByName<Manufacturer>("ManufacturerTemplateId", p => p.ManufacturerTemplateId),
                    new PropertyByName<Manufacturer>("MetaKeywords", p => p.MetaKeywords),
                    new PropertyByName<Manufacturer>("MetaDescription", p => p.MetaDescription),
                    new PropertyByName<Manufacturer>("MetaTitle", p => p.MetaTitle),
                    new PropertyByName<Manufacturer>("Picture", GetPictures),
                    new PropertyByName<Manufacturer>("PageSize", p => p.PageSize),
                    new PropertyByName<Manufacturer>("AllowCustomersToSelectPageSize", p => p.AllowCustomersToSelectPageSize),
                    new PropertyByName<Manufacturer>("PageSizeOptions", p => p.PageSizeOptions),
                    new PropertyByName<Manufacturer>("PriceRanges", p => p.PriceRanges),
                    new PropertyByName<Manufacturer>("Published", p => p.Published),
                    new PropertyByName<Manufacturer>("DisplayOrder", p => p.DisplayOrder)
                };
            }
        }

        /// <summary>
        /// Fills the specified object
        /// </summary>
        /// <param name="objectToFill">The object to fill</param>
        /// <param name="isNew">Is new object flag</param>
        /// <param name="manager">Property manager</param>
        public void FillObject(BaseEntity objectToFill, bool isNew, PropertyManager<Manufacturer> manager)
        {
            var manufacturer = objectToFill as Manufacturer;

            if (manufacturer == null)
                return;
            if (isNew)
                manufacturer.CreatedOnUtc = DateTime.UtcNow;

            manufacturer.Name = manager.GetProperty("Name").StringValue;
            manufacturer.Description = manager.GetProperty("Description").StringValue;
            manufacturer.ManufacturerTemplateId = manager.GetProperty("ManufacturerTemplateId").Int32Value;
            manufacturer.MetaKeywords = manager.GetProperty("MetaKeywords").StringValue;
            manufacturer.MetaDescription = manager.GetProperty("MetaDescription").StringValue;
            manufacturer.MetaTitle = manager.GetProperty("MetaTitle").StringValue;
            var picture = LoadPicture(manager.GetProperty("Picture").StringValue, manufacturer.Name,
                isNew ? null : (int?) manufacturer.PictureId);
            manufacturer.PageSize = manager.GetProperty("PageSize").Int32Value;
            manufacturer.AllowCustomersToSelectPageSize =
                manager.GetProperty("AllowCustomersToSelectPageSize").BooleanValue;
            manufacturer.PageSizeOptions = manager.GetProperty("PageSizeOptions").StringValue;
            manufacturer.PriceRanges = manager.GetProperty("PriceRanges").StringValue;
            manufacturer.Published = manager.GetProperty("Published").BooleanValue;
            manufacturer.DisplayOrder = manager.GetProperty("DisplayOrder").Int32Value;

            if (picture != null)
                manufacturer.PictureId = picture.Id;

            manufacturer.UpdatedOnUtc = DateTime.UtcNow;
        }
    }
}
