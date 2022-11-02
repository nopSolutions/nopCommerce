using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;

namespace Nop.Services.ExportImport
{
    public partial class ExportSpecificationAttribute : ProductSpecificationAttribute
    {
        protected ExportSpecificationAttribute() { }

        /// <summary>
        /// Gets or sets the specification attribute identifier
        /// </summary>
        public int SpecificationAttributeId { get; set; }

        /// <summary>
        /// Create data to export the product specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The product specification attribute to export</param>
        /// <param name="specificationAttributeService">Specification attribute service</param>
        /// <param name="localizationService"></param>
        /// <returns></returns>
        public static async Task<ExportSpecificationAttribute> CreateAsync(ProductSpecificationAttribute specificationAttribute, ISpecificationAttributeService specificationAttributeService)
        {
            var specificationAttributeOption = await specificationAttributeService.GetSpecificationAttributeOptionByIdAsync(specificationAttribute.SpecificationAttributeOptionId);

            var attribute = new ExportSpecificationAttribute
            {
                Id = specificationAttribute.Id,
                AttributeTypeId = specificationAttribute.AttributeTypeId,
                AllowFiltering = specificationAttribute.AllowFiltering,
                ShowOnProductPage = specificationAttribute.ShowOnProductPage,
                DisplayOrder = specificationAttribute.DisplayOrder,
                SpecificationAttributeOptionId = specificationAttribute.SpecificationAttributeOptionId,
                SpecificationAttributeId = specificationAttributeOption.SpecificationAttributeId
            };

            switch (attribute.AttributeType)
            {
                case SpecificationAttributeType.Option:
                    attribute.CustomValue = specificationAttributeOption.Name;
                    break;
                case SpecificationAttributeType.CustomText:
                case SpecificationAttributeType.CustomHtmlText:
                case SpecificationAttributeType.Hyperlink:
                    attribute.CustomValue = specificationAttribute.CustomValue;
                    break;
            }

            return attribute;
        }
    }
}