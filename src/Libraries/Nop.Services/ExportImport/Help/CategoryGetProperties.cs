using System;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Media;

namespace Nop.Services.ExportImport.Help
{
    /// <summary>
    /// Class management category using the properties
    /// </summary>
    public class CategoryGetProperties : BaseGetProperties, IGetProperties<Category>
    {
        public CategoryGetProperties(IPictureService pictureService) : base(pictureService)
        {
        }

        /// <summary>
        /// Get property array
        /// </summary>
        public PropertyByName<Category>[] GetProperties
        {
            get
            {
                return new[]
                {
                    new PropertyByName<Category>("Id", p => p.Id),
                    new PropertyByName<Category>("Name", p => p.Name),
                    new PropertyByName<Category>("Description", p => p.Description),
                    new PropertyByName<Category>("CategoryTemplateId", p => p.CategoryTemplateId),
                    new PropertyByName<Category>("MetaKeywords", p => p.MetaKeywords),
                    new PropertyByName<Category>("MetaDescription", p => p.MetaDescription),
                    new PropertyByName<Category>("MetaTitle", p => p.MetaTitle),
                    new PropertyByName<Category>("ParentCategoryId", p => p.ParentCategoryId),
                    new PropertyByName<Category>("Picture", GetPictures),
                    new PropertyByName<Category>("PageSize", p => p.PageSize),
                    new PropertyByName<Category>("AllowCustomersToSelectPageSize", p => p.AllowCustomersToSelectPageSize),
                    new PropertyByName<Category>("PageSizeOptions", p => p.PageSizeOptions),
                    new PropertyByName<Category>("PriceRanges", p => p.PriceRanges),
                    new PropertyByName<Category>("ShowOnHomePage", p => p.ShowOnHomePage),
                    new PropertyByName<Category>("IncludeInTopMenu", p => p.IncludeInTopMenu),
                    new PropertyByName<Category>("Published", p => p.Published),
                    new PropertyByName<Category>("DisplayOrder", p => p.DisplayOrder)
                };
            }
        }

        /// <summary>
        /// Fills the specified object
        /// </summary>
        /// <param name="objectToFill">The object to fill</param>
        /// <param name="isNew">Is new object flag</param>
        /// <param name="manager">Property manager</param>
        public void FillObject(BaseEntity objectToFill, bool isNew, PropertyManager<Category> manager)
        {
            var category = objectToFill as Category;

            if (category == null)
                return;
            if (isNew)
                category.CreatedOnUtc = DateTime.UtcNow;

            category.Name = manager.GetProperty("Name").StringValue;
            category.Description = manager.GetProperty("Description").StringValue;

            category.CategoryTemplateId = manager.GetProperty("CategoryTemplateId").Int32Value;
            category.MetaKeywords = manager.GetProperty("MetaKeywords").StringValue;
            category.MetaDescription = manager.GetProperty("MetaDescription").StringValue;
            category.MetaTitle = manager.GetProperty("MetaTitle").StringValue;
            category.ParentCategoryId = manager.GetProperty("ParentCategoryId").Int32Value;
            var picture = LoadPicture(manager.GetProperty("Picture").StringValue, category.Name,
                isNew ? null : (int?)category.PictureId);
            category.PageSize = manager.GetProperty("PageSize").Int32Value;
            category.AllowCustomersToSelectPageSize =
                manager.GetProperty("AllowCustomersToSelectPageSize").BooleanValue;
            category.PageSizeOptions = manager.GetProperty("PageSizeOptions").StringValue;
            category.PriceRanges = manager.GetProperty("PriceRanges").StringValue;
            category.ShowOnHomePage = manager.GetProperty("PriceRanges").BooleanValue;
            category.IncludeInTopMenu = manager.GetProperty("PriceRanges").BooleanValue;
            category.Published = manager.GetProperty("Published").BooleanValue;
            category.DisplayOrder = manager.GetProperty("DisplayOrder").Int32Value;

            if (picture != null)
                category.PictureId = picture.Id;

            category.UpdatedOnUtc = DateTime.UtcNow;
        }
    }
}
