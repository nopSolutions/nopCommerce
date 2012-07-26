using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Tax;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.Catalog
{
    [TestFixture]
    public class ProductAttributeParserTests : ServiceTest
    {
        IRepository<ProductAttribute> _productAttributeRepo;
        IRepository<ProductVariantAttribute> _productVariantAttributeRepo;
        IRepository<ProductVariantAttributeCombination> _productVariantAttributeCombinationRepo;
        IRepository<ProductVariantAttributeValue> _productVariantAttributeValueRepo;
        IProductAttributeService _productAttributeService;
        IProductAttributeParser _productAttributeParser;
        IEventPublisher _eventPublisher;

        IWorkContext _workContext;
        ICurrencyService _currencyService;
        ILocalizationService _localizationService;
        ITaxService _taxService;
        IPriceFormatter _priceFormatter;
        IDownloadService _downloadService;
        IWebHelper _webHelper;
        IProductAttributeFormatter _productAttributeFormatter;

        ProductAttribute pa1, pa2, pa3;
        ProductVariantAttribute pva1_1, pva2_1, pva3_1;
        ProductVariantAttributeValue pvav1_1, pvav1_2, pvav2_1, pvav2_2;
        
        [SetUp]
        public new void SetUp()
        {
            #region Test data

            //color (dropdownlist)
            pa1 = new ProductAttribute
            {
                Id = 1,
                Name = "Color",
            };
            pva1_1 = new ProductVariantAttribute
            {
                Id = 11,
                ProductVariantId = 1,
                TextPrompt = "Select color:",
                IsRequired = true,
                AttributeControlType = AttributeControlType.DropdownList,
                DisplayOrder = 1,
                ProductAttribute = pa1,
                ProductAttributeId = pa1.Id
            };
            pvav1_1 = new ProductVariantAttributeValue
            {
                Id = 11,
                Name = "Green",
                DisplayOrder = 1,
                ProductVariantAttribute = pva1_1,
                ProductVariantAttributeId = pva1_1.Id
            };
            pvav1_2 = new ProductVariantAttributeValue
            {
                Id = 12,
                Name = "Red",
                DisplayOrder = 2,
                ProductVariantAttribute = pva1_1,
                ProductVariantAttributeId = pva1_1.Id
            };
            pva1_1.ProductVariantAttributeValues.Add(pvav1_1);
            pva1_1.ProductVariantAttributeValues.Add(pvav1_2);

            //custom option (checkboxes)
            pa2 = new ProductAttribute
            {
                Id = 2,
                Name = "Some custom option",
            };
            pva2_1 = new ProductVariantAttribute
            {
                Id = 21,
                ProductVariantId = 1,
                TextPrompt = "Select at least one option:",
                IsRequired = true,
                AttributeControlType = AttributeControlType.Checkboxes,
                DisplayOrder = 2,
                ProductAttribute = pa2,
                ProductAttributeId = pa2.Id
            };
            pvav2_1 = new ProductVariantAttributeValue
            {
                Id = 21,
                Name = "Option 1",
                DisplayOrder = 1,
                ProductVariantAttribute = pva2_1,
                ProductVariantAttributeId = pva2_1.Id
            };
            pvav2_2 = new ProductVariantAttributeValue
            {
                Id = 22,
                Name = "Option 2",
                DisplayOrder = 2,
                ProductVariantAttribute = pva2_1,
                ProductVariantAttributeId = pva2_1.Id
            };
            pva2_1.ProductVariantAttributeValues.Add(pvav2_1);
            pva2_1.ProductVariantAttributeValues.Add(pvav2_2);

            //custom text
            pa3 = new ProductAttribute
            {
                Id = 3,
                Name = "Custom text",
            };
            pva3_1 = new ProductVariantAttribute
            {
                Id = 31,
                ProductVariantId = 1,
                TextPrompt = "Enter custom text:",
                IsRequired = true,
                AttributeControlType = AttributeControlType.TextBox,
                DisplayOrder = 1,
                ProductAttribute = pa1,
                ProductAttributeId = pa3.Id
            };


            #endregion
            
            _productAttributeRepo = MockRepository.GenerateMock<IRepository<ProductAttribute>>();
            _productAttributeRepo.Expect(x => x.Table).Return(new List<ProductAttribute>() { pa1, pa2, pa3 }.AsQueryable());
            _productAttributeRepo.Expect(x => x.GetById(pa1.Id)).Return(pa1);
            _productAttributeRepo.Expect(x => x.GetById(pa2.Id)).Return(pa2);
            _productAttributeRepo.Expect(x => x.GetById(pa3.Id)).Return(pa3);

            _productVariantAttributeRepo = MockRepository.GenerateMock<IRepository<ProductVariantAttribute>>();
            _productVariantAttributeRepo.Expect(x => x.Table).Return(new List<ProductVariantAttribute>() { pva1_1, pva2_1, pva3_1 }.AsQueryable());
            _productVariantAttributeRepo.Expect(x => x.GetById(pva1_1.Id)).Return(pva1_1);
            _productVariantAttributeRepo.Expect(x => x.GetById(pva2_1.Id)).Return(pva2_1);
            _productVariantAttributeRepo.Expect(x => x.GetById(pva3_1.Id)).Return(pva3_1);

            _productVariantAttributeCombinationRepo = MockRepository.GenerateMock<IRepository<ProductVariantAttributeCombination>>();
            _productVariantAttributeCombinationRepo.Expect(x => x.Table).Return(new List<ProductVariantAttributeCombination>().AsQueryable());

            _productVariantAttributeValueRepo = MockRepository.GenerateMock<IRepository<ProductVariantAttributeValue>>();
            _productVariantAttributeValueRepo.Expect(x => x.Table).Return(new List<ProductVariantAttributeValue>() { pvav1_1, pvav1_2, pvav2_1, pvav2_2 }.AsQueryable());
            _productVariantAttributeValueRepo.Expect(x => x.GetById(pvav1_1.Id)).Return(pvav1_1);
            _productVariantAttributeValueRepo.Expect(x => x.GetById(pvav1_2.Id)).Return(pvav1_2);
            _productVariantAttributeValueRepo.Expect(x => x.GetById(pvav2_1.Id)).Return(pvav2_1);
            _productVariantAttributeValueRepo.Expect(x => x.GetById(pvav2_2.Id)).Return(pvav2_2);

            _eventPublisher = MockRepository.GenerateMock<IEventPublisher>();
            _eventPublisher.Expect(x => x.Publish(Arg<object>.Is.Anything));

            var cacheManager = new NopNullCache();

            _productAttributeService = new ProductAttributeService(cacheManager, 
                _productAttributeRepo,
                _productVariantAttributeRepo,
                _productVariantAttributeCombinationRepo,
                _productVariantAttributeValueRepo,
                _eventPublisher);

            _productAttributeParser = new ProductAttributeParser(_productAttributeService);




            var workingLanguage = new Language();
            _workContext = MockRepository.GenerateMock<IWorkContext>();
            _workContext.Expect(x => x.WorkingLanguage).Return(workingLanguage);
            _currencyService = MockRepository.GenerateMock<ICurrencyService>();
            _localizationService = MockRepository.GenerateMock<ILocalizationService>();
            _localizationService.Expect(x => x.GetResource("GiftCardAttribute.For")).Return("For: {0}");
            _localizationService.Expect(x => x.GetResource("GiftCardAttribute.From")).Return("From: {0}");
            _taxService = MockRepository.GenerateMock<ITaxService>();
            _priceFormatter = MockRepository.GenerateMock<IPriceFormatter>();
            _downloadService = MockRepository.GenerateMock<IDownloadService>();
            _webHelper = MockRepository.GenerateMock<IWebHelper>();

            _productAttributeFormatter = new ProductAttributeFormatter(_workContext,
                _productAttributeService,
                _productAttributeParser,
                _currencyService,
                _localizationService,
                _taxService,
                _priceFormatter,
                _downloadService,
                _webHelper);
        }
        
        [Test]
        public void Can_add_and_parse_productAttributes()
        {
            string attributes = "";
            //color: green
            attributes = _productAttributeParser.AddProductAttribute(attributes, pva1_1, pvav1_1.Id.ToString());
            //custom option: option 1, option 2
            attributes = _productAttributeParser.AddProductAttribute(attributes, pva2_1, pvav2_1.Id.ToString());
            attributes = _productAttributeParser.AddProductAttribute(attributes, pva2_1, pvav2_2.Id.ToString());
            //custom text
            attributes = _productAttributeParser.AddProductAttribute(attributes, pva3_1, "Some custom text goes here");

            var parsed_pvaValues = _productAttributeParser.ParseProductVariantAttributeValues(attributes);
            parsed_pvaValues.Contains(pvav1_1).ShouldEqual(true);
            parsed_pvaValues.Contains(pvav1_2).ShouldEqual(false);
            parsed_pvaValues.Contains(pvav2_1).ShouldEqual(true);
            parsed_pvaValues.Contains(pvav2_2).ShouldEqual(true);
            parsed_pvaValues.Contains(pvav2_2).ShouldEqual(true);

            var parsedValues = _productAttributeParser.ParseValues(attributes, pva3_1.Id);
            parsedValues.Count.ShouldEqual(1);
            parsedValues.Contains("Some custom text goes here").ShouldEqual(true);
            parsedValues.Contains("Some other custom text").ShouldEqual(false);
        }

        [Test]
        public void Can_add_and_parse_giftCardAttributes()
        {
            string attributes = "";
            attributes = _productAttributeParser.AddGiftCardAttribute(attributes,
                "recipientName 1", "recipientEmail@gmail.com", 
                "senderName 1", "senderEmail@gmail.com", "custom message");

            string recipientName, recipientEmail, senderName, senderEmail, giftCardMessage;
            _productAttributeParser.GetGiftCardAttribute(attributes, 
                out recipientName,
                out recipientEmail,
                out senderName,
                out senderEmail,
                out giftCardMessage);
            recipientName.ShouldEqual("recipientName 1");
            recipientEmail.ShouldEqual("recipientEmail@gmail.com");
            senderName.ShouldEqual("senderName 1");
            senderEmail.ShouldEqual("senderEmail@gmail.com");
            giftCardMessage.ShouldEqual("custom message");
        }

        [Test]
        public void Can_add_render_attributes_withoutPrices()
        {
            string attributes = "";
            //color: green
            attributes = _productAttributeParser.AddProductAttribute(attributes, pva1_1, pvav1_1.Id.ToString());
            //custom option: option 1, option 2
            attributes = _productAttributeParser.AddProductAttribute(attributes, pva2_1, pvav2_1.Id.ToString());
            attributes = _productAttributeParser.AddProductAttribute(attributes, pva2_1, pvav2_2.Id.ToString());
            //custom text
            attributes = _productAttributeParser.AddProductAttribute(attributes, pva3_1, "Some custom text goes here");

            //gift card attributes
            attributes = _productAttributeParser.AddGiftCardAttribute(attributes,
                "recipientName 1", "recipientEmail@gmail.com",
                "senderName 1", "senderEmail@gmail.com", "custom message");

            var productVariant = new ProductVariant()
            {
                IsGiftCard = true
            };
            var customer = new Customer();
            string formattedAttributes = _productAttributeFormatter.FormatAttributes(productVariant,
                attributes, customer, "<br />", false, false, true, true);
            formattedAttributes.ShouldEqual("Color: Green<br />Some custom option: Option 1<br />Some custom option: Option 2<br />Color: Some custom text goes here<br />For: recipientName 1<br />From: senderName 1");
        }
    }
}
