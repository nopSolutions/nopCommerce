using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Category (for caching)
    /// </summary>
    [Serializable]
    //Entity Framework will assume that any class that inherits from a POCO class that is mapped to a table on the database requires a Discriminator column
    //That's why we have to add [NotMapped] as an attribute of the derived class.
    [NotMapped]
    public class CategoryForCaching : Category, IEntityForCaching
    {
        public CategoryForCaching()
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="c">Category to copy</param>
        public CategoryForCaching(Category c)
        {
            Id = c.Id;
            Name = c.Name;
            Description = c.Description;
            CategoryTemplateId = c.CategoryTemplateId;
            MetaKeywords = c.MetaKeywords;
            MetaDescription = c.MetaDescription;
            MetaTitle = c.MetaTitle;
            ParentCategoryId = c.ParentCategoryId;
            PictureId = c.PictureId;
            PageSize = c.PageSize;
            AllowCustomersToSelectPageSize = c.AllowCustomersToSelectPageSize;
            PageSizeOptions = c.PageSizeOptions;
            PriceRanges = c.PriceRanges;
            ShowOnHomepage = c.ShowOnHomepage;
            IncludeInTopMenu = c.IncludeInTopMenu;
            SubjectToAcl = c.SubjectToAcl;
            LimitedToStores = c.LimitedToStores;
            Published = c.Published;
            Deleted = c.Deleted;
            DisplayOrder = c.DisplayOrder;
            CreatedOnUtc = c.CreatedOnUtc;
            UpdatedOnUtc = c.UpdatedOnUtc;
        }

        [JsonIgnore]
        public override ICollection<DiscountCategoryMapping> DiscountCategoryMappings => throw new Exception("Entity for caching doesn't support navigation properties");

        [JsonIgnore]
        public override IList<Discount> AppliedDiscounts => throw new Exception("Entity for caching doesn't support navigation properties");
    }
}