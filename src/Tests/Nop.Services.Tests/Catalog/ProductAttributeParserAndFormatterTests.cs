using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using NUnit.Framework;

namespace Nop.Services.Tests.Catalog
{
    [TestFixture]
    public class ProductAttributeParserTests : ServiceTest
    {
        private IProductAttributeParser _productAttributeParser;
        private IProductAttributeFormatter _productAttributeFormatter;
        private IEnumerable<KeyValuePair<ProductAttributeMapping, IList<ProductAttributeValue>>> _productAttributeMappings;

        [SetUp]
        public void SetUp()
        {
            var productAttributeService = GetService<IProductAttributeService>();

            _productAttributeParser = GetService<IProductAttributeParser>();
            _productAttributeFormatter = GetService<IProductAttributeFormatter>();

            _productAttributeMappings =
                productAttributeService.GetProductAttributeMappingsByProductId(GetService<IProductService>()
                    .GetProductBySku("COMP_CUST").Id).Select(p => KeyValuePair.Create(p, productAttributeService.GetProductAttributeValues(p.Id)));
        }

        [Test]
        public void CanAddAndParseProductAttributes()
        {
            var attributes = string.Empty;

            foreach (var productAttributeMapping in _productAttributeMappings)
            {
                var skip = true;

                foreach (var productAttributeValue in productAttributeMapping.Value.OrderBy(p => p.Id))
                {
                    if (skip)
                    {
                        skip = false;
                        continue;
                    }

                    attributes = _productAttributeParser.AddProductAttribute(attributes, productAttributeMapping.Key, productAttributeValue.Id.ToString());
                }
            }

            var parsedAttributeValues = _productAttributeParser.ParseProductAttributeValues(attributes).Select(p => p.Id).ToList();

            foreach (var productAttributeMapping in _productAttributeMappings)
            {
                var skip = true;

                foreach (var productAttributeValue in productAttributeMapping.Value.OrderBy(p => p.Id))
                {
                    if (skip)
                    {
                        parsedAttributeValues.Contains(productAttributeValue.Id).Should().BeFalse();
                        skip = false;
                        continue;
                    }

                    parsedAttributeValues.Contains(productAttributeValue.Id).Should().BeTrue();
                }
            }
        }

        [Test]
        public void CanAddAndRemoveProductAttributes()
        {
            var attributes = string.Empty;

            var delete = false;

            foreach (var productAttributeMapping in _productAttributeMappings)
            {
                foreach (var productAttributeValue in productAttributeMapping.Value.OrderBy(p => p.Id)) 
                    attributes = _productAttributeParser.AddProductAttribute(attributes, productAttributeMapping.Key, productAttributeValue.Id.ToString());

                if (delete)
                    attributes = _productAttributeParser.RemoveProductAttribute(attributes, productAttributeMapping.Key);

                delete = !delete;
            }

            var parsedAttributeValues = _productAttributeParser.ParseProductAttributeValues(attributes).Select(p => p.Id).ToList();

            delete = false;

            foreach (var productAttributeMapping in _productAttributeMappings)
            {
                foreach (var productAttributeValue in productAttributeMapping.Value.OrderBy(p => p.Id)) 
                    parsedAttributeValues.Contains(productAttributeValue.Id).Should().Be(!delete);

                delete = !delete;
            }
        }

        [Test]
        public void CanRenderAttributesWithoutPrices()
        {
            var attributes = string.Empty;

            foreach (var productAttributeMapping in _productAttributeMappings)
                foreach (var productAttributeValue in productAttributeMapping.Value.OrderBy(p => p.Id))
                    attributes = _productAttributeParser.AddProductAttribute(attributes, productAttributeMapping.Key, productAttributeValue.Id.ToString());

            attributes = _productAttributeParser.AddGiftCardAttribute(attributes,
                "recipientName 1", "recipientEmail@gmail.com",
                "senderName 1", "senderEmail@gmail.com", "custom message");
            
            var product = new Product { IsGiftCard = true, GiftCardType = GiftCardType.Virtual };
            var customer = new Customer();

            var formattedAttributes = _productAttributeFormatter.FormatAttributes(product,
                attributes, customer, "<br />", false);

            formattedAttributes.Should().Be(
                "Processor: 2.2 GHz Intel Pentium Dual-Core E2200<br />Processor: 2.5 GHz Intel Pentium Dual-Core E2200 [+$15.00]<br />RAM: 2 GB<br />RAM: 4GB [+$20.00]<br />RAM: 8GB [+$60.00]<br />HDD: 320 GB<br />HDD: 400 GB [+$100.00]<br />OS: Vista Home [+$50.00]<br />OS: Vista Premium [+$60.00]<br />Software: Microsoft Office [+$50.00]<br />Software: Acrobat Reader [+$10.00]<br />Software: Total Commander [+$5.00]<br />From: senderName 1 <senderEmail@gmail.com><br />For: recipientName 1 <recipientEmail@gmail.com>");

            formattedAttributes = _productAttributeFormatter.FormatAttributes(product,
                attributes, customer, "<br />", false, false);

            formattedAttributes.Should().Be(
                "Processor: 2.2 GHz Intel Pentium Dual-Core E2200<br />Processor: 2.5 GHz Intel Pentium Dual-Core E2200<br />RAM: 2 GB<br />RAM: 4GB<br />RAM: 8GB<br />HDD: 320 GB<br />HDD: 400 GB<br />OS: Vista Home<br />OS: Vista Premium<br />Software: Microsoft Office<br />Software: Acrobat Reader<br />Software: Total Commander<br />From: senderName 1 <senderEmail@gmail.com><br />For: recipientName 1 <recipientEmail@gmail.com>");
        }

        [Test]
        public void CanAddAndParseGiftCardAttributes()
        {
            var attributes = string.Empty;
            attributes = _productAttributeParser.AddGiftCardAttribute(attributes,
                "recipientName 1", "recipientEmail@gmail.com",
                "senderName 1", "senderEmail@gmail.com", "custom message");

            _productAttributeParser.GetGiftCardAttribute(attributes,
                out var recipientName,
                out var recipientEmail,
                out var senderName,
                out var senderEmail,
                out var giftCardMessage);
            recipientName.Should().Be("recipientName 1");
            recipientEmail.Should().Be("recipientEmail@gmail.com");
            senderName.Should().Be("senderName 1");
            senderEmail.Should().Be("senderEmail@gmail.com");
            giftCardMessage.Should().Be("custom message");
        }

        [Test]
        public void CanRenderVirtualGiftCart()
        {
            var attributes = _productAttributeParser.AddGiftCardAttribute(string.Empty,
                "recipientName 1", "recipientEmail@gmail.com",
                "senderName 1", "senderEmail@gmail.com", "custom message");

            var product = new Product
            {
                IsGiftCard = true,
                GiftCardType = GiftCardType.Virtual
            };
            var customer = new Customer();
            var formattedAttributes = _productAttributeFormatter.FormatAttributes(product,
                attributes, customer, "<br />", false, false);
            formattedAttributes.Should().Be("From: senderName 1 <senderEmail@gmail.com><br />For: recipientName 1 <recipientEmail@gmail.com>");
        }

        [Test]
        public void CanRenderPhysicalGiftCart()
        {
            var attributes = _productAttributeParser.AddGiftCardAttribute(string.Empty,
                "recipientName 1", "recipientEmail@gmail.com",
                "senderName 1", "senderEmail@gmail.com", "custom message");

            var product = new Product
            {
                IsGiftCard = true,
                GiftCardType = GiftCardType.Physical
            };
            var customer = new Customer();
            var formattedAttributes = _productAttributeFormatter.FormatAttributes(product,
                attributes, customer, "<br />", false, false);
            formattedAttributes.Should().Be("From: senderName 1<br />For: recipientName 1");
        }
    }
}