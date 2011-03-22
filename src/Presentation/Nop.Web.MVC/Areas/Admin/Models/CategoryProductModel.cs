using System;
using System.ComponentModel.DataAnnotations;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework;
using Nop.Web.MVC.Areas.Admin.Validators;
using FluentValidation.Attributes;
namespace Nop.Web.MVC.Areas.Admin.Models
{
    [Validator(typeof(CategoryProductValidator))]
    public class CategoryProductModel : IEquatable<CategoryProductModel>
    {
        [UIHint("ProductSelector")]
        [NopResourceDisplayName("Admin.Catalog.Categories.Products.Fields.Product")]
        public int ProductId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Products.Fields.ProductName")]
        public string ProductName {get;set;}

        [NopResourceDisplayName("Admin.Catalog.Categories.Products.Fields.Category")]
        public int CategoryId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Products.Fields.IsFeaturedProduct")]
        public bool IsFeaturedProduct { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Products.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public override bool Equals(object obj)
        {
            var productCategory = obj as ProductCategory;
            if (productCategory != null)
            {
                return Equals(productCategory);
            }
            return false;
        }

        public bool Equals(CategoryProductModel other)
        {
            return other.ProductId.Equals(ProductId);
        }
    }
}