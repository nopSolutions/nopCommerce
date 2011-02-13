using System.ComponentModel.DataAnnotations;
using Nop.Core.Domain.Catalog;

namespace Nop.Web.MVC.Areas.Admin.Models
{
    public class CategoryProductModel
    {
        public CategoryProductModel()
        {
            
        }

        public CategoryProductModel(ProductCategory productCategory)
        {
            Id = productCategory.Id;
            ProductId = productCategory.ProductId;
            CategoryId = productCategory.CategoryId;
            IsFeaturedProduct = productCategory.IsFeaturedProduct;
            DisplayOrder = productCategory.DisplayOrder;
            Category = productCategory.Category;
            Product = productCategory.Product;
        }

        public int Id { get; set; }
        [UIHint("ProductSelector")]
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public bool IsFeaturedProduct { get; set; }
        public int DisplayOrder { get; set; }
        public Category Category { get; set; }
        public Product Product { get; set; }
    }
}