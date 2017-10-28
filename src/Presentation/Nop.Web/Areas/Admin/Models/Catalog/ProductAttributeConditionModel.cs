using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public partial class ProductAttributeConditionModel : BaseNopModel
    {
        public ProductAttributeConditionModel()
        {
            ProductAttributes = new List<ProductAttributeModel>();
        }

        //MVC is suppressing further validation if the IFormCollection is passed to a controller method. That's why we add to the model
        public IFormCollection Form { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Condition.EnableCondition")]
        public bool EnableCondition { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Condition.Attributes")]
        public int SelectedProductAttributeId { get; set; }
        public IList<ProductAttributeModel> ProductAttributes { get; set; }

        public int ProductAttributeMappingId { get; set; }

        #region Nested classes

        public partial class ProductAttributeModel : BaseNopEntityModel
        {
            public ProductAttributeModel()
            {
                Values = new List<ProductAttributeValueModel>();
            }

            public int ProductAttributeId { get; set; }

            public string Name { get; set; }

            public string TextPrompt { get; set; }

            public bool IsRequired { get; set; }

            public AttributeControlType AttributeControlType { get; set; }

            public IList<ProductAttributeValueModel> Values { get; set; }
        }

        public partial class ProductAttributeValueModel : BaseNopEntityModel
        {
            public string Name { get; set; }

            public bool IsPreSelected { get; set; }
        }

        #endregion
    }
}