using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Converters;
using Nop.Plugin.Api.DTO;
using Nop.Services.Catalog;
using Nop.Services.Media;

namespace Nop.Plugin.Api.Services
{
    public class ProductAttributeConverter : IProductAttributeConverter
    {
        private readonly IDownloadService _downloadService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeService _productAttributeService;

        public ProductAttributeConverter(
            IProductAttributeService productAttributeService,
            IProductAttributeParser productAttributeParser,
            IDownloadService downloadService)
        {
            _productAttributeService = productAttributeService;
            _productAttributeParser = productAttributeParser;
            _downloadService = downloadService;
        }

        public async Task<string> ConvertToXmlAsync(List<ProductItemAttributeDto> attributeDtos, int productId)
        {
            var attributesXml = "";

            if (attributeDtos is not { Count: > 0 })
            {
                return attributesXml;
            }

            var productAttributes = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(productId);
            foreach (var attribute in productAttributes)
            {
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    {
                        // there should be only one selected value for this attribute
                        var selectedAttribute = attributeDtos.Where(x => x.Id == attribute.Id).FirstOrDefault();
                        if (selectedAttribute != null)
                        {
                            int selectedAttributeValue;
                            var isInt = int.TryParse(selectedAttribute.Value, out selectedAttributeValue);
                            if (isInt && selectedAttributeValue > 0)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                                                                            attribute, selectedAttributeValue.ToString());
                            }
                        }
                    }
                        break;
                    case AttributeControlType.Checkboxes:
                    {
                        // there could be more than one selected value for this attribute
                        var selectedAttributes = attributeDtos.Where(x => x.Id == attribute.Id);
                        foreach (var selectedAttribute in selectedAttributes)
                        {
                            int selectedAttributeValue;
                            var isInt = int.TryParse(selectedAttribute.Value, out selectedAttributeValue);
                            if (isInt && selectedAttributeValue > 0)
                            {
                                // currently there is no support for attribute quantity
                                var quantity = 1;

                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                                                                            attribute, selectedAttributeValue.ToString(), quantity);
                            }
                        }
                    }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                    {
                        //load read-only(already server - side selected) values
                        var attributeValues = await _productAttributeService.GetProductAttributeValuesAsync(attribute.Id);
                        foreach (var selectedAttributeId in attributeValues
                                                            .Where(v => v.IsPreSelected)
                                                            .Select(v => v.Id)
                                                            .ToList())
                        {
                            attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                                                                        attribute, selectedAttributeId.ToString());
                        }
                    }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                    {
                        var selectedAttribute = attributeDtos.Where(x => x.Id == attribute.Id).FirstOrDefault();

                        if (selectedAttribute != null)
                        {
                            attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                                                                        attribute, selectedAttribute.Value);
                        }
                    }
                        break;
                    case AttributeControlType.Datepicker:
                    {
                        var selectedAttribute = attributeDtos.Where(x => x.Id == attribute.Id).FirstOrDefault();

                        if (selectedAttribute != null)
                        {
                            DateTime selectedDate;

                            // Since nopCommerce uses this format to keep the date in the database to keep it consisten we will expect the same format to be passed
                            var validDate = DateTime.TryParseExact(selectedAttribute.Value, "D", CultureInfo.CurrentCulture,
                                                                   DateTimeStyles.None, out selectedDate);

                            if (validDate)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                                                                            attribute, selectedDate.ToString("D"));
                            }
                        }
                    }
                        break;
                    case AttributeControlType.FileUpload:
                    {
                        var selectedAttribute = attributeDtos.Where(x => x.Id == attribute.Id).FirstOrDefault();

                        if (selectedAttribute != null)
                        {
                            Guid downloadGuid;
                            Guid.TryParse(selectedAttribute.Value, out downloadGuid);
                            var download = await _downloadService.GetDownloadByGuidAsync(downloadGuid);
                            if (download != null)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                                                                            attribute, download.DownloadGuid.ToString());
                            }
                        }
                    }
                        break;
                }
            }

            // No Gift Card attributes support yet

            return attributesXml;
        }

        public List<ProductItemAttributeDto> Parse(string attributesXml)
        {
            var attributeDtos = new List<ProductItemAttributeDto>();
            if (string.IsNullOrEmpty(attributesXml))
            {
                return attributeDtos;
            }

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(attributesXml);

                foreach (XmlNode attributeNode in xmlDoc.SelectNodes(@"//Attributes/ProductAttribute"))
                {
                    if (attributeNode.Attributes?["ID"] != null)
                    {
                        int attributeId;
                        if (int.TryParse(attributeNode.Attributes["ID"].InnerText.Trim(), out attributeId))
                        {
                            foreach (XmlNode attributeValue in attributeNode.SelectNodes("ProductAttributeValue"))
                            {
                                var value = attributeValue.SelectSingleNode("Value").InnerText.Trim();
                                // no support for quantity yet
                                //var quantityNode = attributeValue.SelectSingleNode("Quantity");
                                attributeDtos.Add(new ProductItemAttributeDto
                                                  {
                                                      Id = attributeId,
                                                      Value = value
                                                  });
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            return attributeDtos;
        }
    }
}
