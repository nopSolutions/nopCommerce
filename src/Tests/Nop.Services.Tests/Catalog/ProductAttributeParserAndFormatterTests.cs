using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
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
        private IRepository<ProductAttribute> _productAttributeRepo;
        private IRepository<ProductAttributeMapping> _productAttributeMappingRepo;
        private IRepository<ProductAttributeCombination> _productAttributeCombinationRepo;
        private IRepository<ProductAttributeValue> _productAttributeValueRepo;
        private IRepository<PredefinedProductAttributeValue> _predefinedProductAttributeValueRepo;
        private IProductAttributeService _productAttributeService;
        private IProductAttributeParser _productAttributeParser;
        private IEventPublisher _eventPublisher;

        private IWorkContext _workContext;
        private ICurrencyService _currencyService;
        private ILocalizationService _localizationService;
        private ITaxService _taxService;
        private IPriceFormatter _priceFormatter;
        private IPriceCalculationService _priceCalculationService;
        private IDownloadService _downloadService;
        private IWebHelper _webHelper;
        private ShoppingCartSettings _shoppingCartSettings;
        private IProductAttributeFormatter _productAttributeFormatter;

        private ProductAttribute pa1, pa2, pa3;
        private ProductAttributeMapping pam1_1, pam2_1, pam3_1;
        private ProductAttributeValue pav1_1, pav1_2, pav2_1, pav2_2;
        
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
            pam1_1 = new ProductAttributeMapping
            {
                Id = 11,
                ProductId = 1,
                TextPrompt = "Select color:",
                IsRequired = true,
                AttributeControlType = AttributeControlType.DropdownList,
                DisplayOrder = 1,
                ProductAttribute = pa1,
                ProductAttributeId = pa1.Id
            };
            pav1_1 = new ProductAttributeValue
            {
                Id = 11,
                Name = "Green",
                DisplayOrder = 1,
                ProductAttributeMapping= pam1_1,
                ProductAttributeMappingId = pam1_1.Id
            };
            pav1_2 = new ProductAttributeValue
            {
                Id = 12,
                Name = "Red",
                DisplayOrder = 2,
                ProductAttributeMapping = pam1_1,
                ProductAttributeMappingId = pam1_1.Id
            };
            pam1_1.ProductAttributeValues.Add(pav1_1);
            pam1_1.ProductAttributeValues.Add(pav1_2);

            //custom option (checkboxes)
            pa2 = new ProductAttribute
            {
                Id = 2,
                Name = "Some custom option",
            };
            pam2_1 = new ProductAttributeMapping
            {
                Id = 21,
                ProductId = 1,
                TextPrompt = "Select at least one option:",
                IsRequired = true,
                AttributeControlType = AttributeControlType.Checkboxes,
                DisplayOrder = 2,
                ProductAttribute = pa2,
                ProductAttributeId = pa2.Id
            };
            pav2_1 = new ProductAttributeValue
            {
                Id = 21,
                Name = "Option 1",
                DisplayOrder = 1,
                ProductAttributeMapping = pam2_1,
                ProductAttributeMappingId = pam2_1.Id
            };
            pav2_2 = new ProductAttributeValue
            {
                Id = 22,
                Name = "Option 2",
                DisplayOrder = 2,
                ProductAttributeMapping = pam2_1,
                ProductAttributeMappingId = pam2_1.Id
            };
            pam2_1.ProductAttributeValues.Add(pav2_1);
            pam2_1.ProductAttributeValues.Add(pav2_2);

            //custom text
            pa3 = new ProductAttribute
            {
                Id = 3,
                Name = "Custom text",
            };
            pam3_1 = new ProductAttributeMapping
            {
                Id = 31,
                ProductId = 1,
                TextPrompt = "Enter custom text:",
                IsRequired = true,
                AttributeControlType = AttributeControlType.TextBox,
                DisplayOrder = 1,
                ProductAttribute = pa1,
                ProductAttributeId = pa3.Id
            };


            #endregion
            
            _productAttributeRepo = MockRepository.GenerateMock<IRepository<ProductAttribute>>();
            _productAttributeRepo.Expect(x => x.Table).Return(new List<ProductAttribute> { pa1, pa2, pa3 }.AsQueryable());
            _productAttributeRepo.Expect(x => x.GetById(pa1.Id)).Return(pa1);
            _productAttributeRepo.Expect(x => x.GetById(pa2.Id)).Return(pa2);
            _productAttributeRepo.Expect(x => x.GetById(pa3.Id)).Return(pa3);

            _productAttributeMappingRepo = MockRepository.GenerateMock<IRepository<ProductAttributeMapping>>();
            _productAttributeMappingRepo.Expect(x => x.Table).Return(new List<ProductAttributeMapping> { pam1_1, pam2_1, pam3_1 }.AsQueryable());
            _productAttributeMappingRepo.Expect(x => x.GetById(pam1_1.Id)).Return(pam1_1);
            _productAttributeMappingRepo.Expect(x => x.GetById(pam2_1.Id)).Return(pam2_1);
            _productAttributeMappingRepo.Expect(x => x.GetById(pam3_1.Id)).Return(pam3_1);

            _productAttributeCombinationRepo = MockRepository.GenerateMock<IRepository<ProductAttributeCombination>>();
            _productAttributeCombinationRepo.Expect(x => x.Table).Return(new List<ProductAttributeCombination>().AsQueryable());

            _productAttributeValueRepo = MockRepository.GenerateMock<IRepository<ProductAttributeValue>>();
            _productAttributeValueRepo.Expect(x => x.Table).Return(new List<ProductAttributeValue> { pav1_1, pav1_2, pav2_1, pav2_2 }.AsQueryable());
            _productAttributeValueRepo.Expect(x => x.GetById(pav1_1.Id)).Return(pav1_1);
            _productAttributeValueRepo.Expect(x => x.GetById(pav1_2.Id)).Return(pav1_2);
            _productAttributeValueRepo.Expect(x => x.GetById(pav2_1.Id)).Return(pav2_1);
            _productAttributeValueRepo.Expect(x => x.GetById(pav2_2.Id)).Return(pav2_2);

            _predefinedProductAttributeValueRepo = MockRepository.GenerateMock<IRepository<PredefinedProductAttributeValue>>();

            _eventPublisher = MockRepository.GenerateMock<IEventPublisher>();
            _eventPublisher.Expect(x => x.Publish(Arg<object>.Is.Anything));

            var cacheManager = new NopNullCache();

            _productAttributeService = new ProductAttributeService(cacheManager, 
                _productAttributeRepo,
                _productAttributeMappingRepo,
                _productAttributeCombinationRepo,
                _productAttributeValueRepo,
                _predefinedProductAttributeValueRepo,
                _eventPublisher);

            _productAttributeParser = new ProductAttributeParser(_productAttributeService);

            _priceCalculationService = MockRepository.GenerateMock<IPriceCalculationService>();


            var workingLanguage = new Language();
            _workContext = MockRepository.GenerateMock<IWorkContext>();
            _workContext.Expect(x => x.WorkingLanguage).Return(workingLanguage);
            _currencyService = MockRepository.GenerateMock<ICurrencyService>();
            _localizationService = MockRepository.GenerateMock<ILocalizationService>();
            _localizationService.Expect(x => x.GetResource("GiftCardAttribute.For.Virtual")).Return("For: {0} <{1}>");
            _localizationService.Expect(x => x.GetResource("GiftCardAttribute.From.Virtual")).Return("From: {0} <{1}>");
            _localizationService.Expect(x => x.GetResource("GiftCardAttribute.For.Physical")).Return("For: {0}");
            _localizationService.Expect(x => x.GetResource("GiftCardAttribute.From.Physical")).Return("From: {0}");
            _taxService = MockRepository.GenerateMock<ITaxService>();
            _priceFormatter = MockRepository.GenerateMock<IPriceFormatter>();
            _downloadService = MockRepository.GenerateMock<IDownloadService>();
            _webHelper = MockRepository.GenerateMock<IWebHelper>();
            _shoppingCartSettings = MockRepository.GenerateMock<ShoppingCartSettings>();

            _productAttributeFormatter = new ProductAttributeFormatter(_workContext,
                _productAttributeService,
                _productAttributeParser,
                _currencyService,
                _localizationService,
                _taxService,
                _priceFormatter,
                _downloadService,
                _webHelper,
                _priceCalculationService,
                _shoppingCartSettings);
        }
        
        [Test]
        public void Can_add_and_parse_productAttributes()
        {
            string attributes = "";
            //color: green
            attributes = _productAttributeParser.AddProductAttribute(attributes, pam1_1, pav1_1.Id.ToString());
            //custom option: option 1, option 2
            attributes = _productAttributeParser.AddProductAttribute(attributes, pam2_1, pav2_1.Id.ToString());
            attributes = _productAttributeParser.AddProductAttribute(attributes, pam2_1, pav2_2.Id.ToString());
            //custom text
            attributes = _productAttributeParser.AddProductAttribute(attributes, pam3_1, "Some custom text goes here");

            var parsed_attributeValues = _productAttributeParser.ParseProductAttributeValues(attributes);
            parsed_attributeValues.Contains(pav1_1).ShouldEqual(true);
            parsed_attributeValues.Contains(pav1_2).ShouldEqual(false);
            parsed_attributeValues.Contains(pav2_1).ShouldEqual(true);
            parsed_attributeValues.Contains(pav2_2).ShouldEqual(true);
            parsed_attributeValues.Contains(pav2_2).ShouldEqual(true);

            var parsedValues = _productAttributeParser.ParseValues(attributes, pam3_1.Id);
            parsedValues.Count.ShouldEqual(1);
            parsedValues.Contains("Some custom text goes here").ShouldEqual(true);
            parsedValues.Contains("Some other custom text").ShouldEqual(false);
        }

        [Test]
        public void Can_add_and_remove_productAttributes()
        {
            string attributes = "";
            //color: green
            attributes = _productAttributeParser.AddProductAttribute(attributes, pam1_1, pav1_1.Id.ToString());
            //custom option: option 1, option 2
            attributes = _productAttributeParser.AddProductAttribute(attributes, pam2_1, pav2_1.Id.ToString());
            attributes = _productAttributeParser.AddProductAttribute(attributes, pam2_1, pav2_2.Id.ToString());
            //custom text
            attributes = _productAttributeParser.AddProductAttribute(attributes, pam3_1, "Some custom text goes here");
            //delete some of them
            attributes = _productAttributeParser.RemoveProductAttribute(attributes, pam2_1);
            attributes = _productAttributeParser.RemoveProductAttribute(attributes, pam3_1);

            var parsed_attributeValues = _productAttributeParser.ParseProductAttributeValues(attributes);
            parsed_attributeValues.Contains(pav1_1).ShouldEqual(true);
            parsed_attributeValues.Contains(pav1_2).ShouldEqual(false);
            parsed_attributeValues.Contains(pav2_1).ShouldEqual(false);
            parsed_attributeValues.Contains(pav2_2).ShouldEqual(false);
            parsed_attributeValues.Contains(pav2_2).ShouldEqual(false);

            var parsedValues = _productAttributeParser.ParseValues(attributes, pam3_1.Id);
            parsedValues.Count.ShouldEqual(0);
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
        public void Can_render_virtual_gift_cart()
        {
            string attributes = _productAttributeParser.AddGiftCardAttribute("",
                "recipientName 1", "recipientEmail@gmail.com",
                "senderName 1", "senderEmail@gmail.com", "custom message");

            var product = new Product
            {
                IsGiftCard = true,
                GiftCardType = GiftCardType.Virtual,
            };
            var customer = new Customer();
            string formattedAttributes = _productAttributeFormatter.FormatAttributes(product,
                attributes, customer, "<br />", false, false, true, true);
            formattedAttributes.ShouldEqual("From: senderName 1 <senderEmail@gmail.com><br />For: recipientName 1 <recipientEmail@gmail.com>");
        }

        [Test]
        public void Can_render_physical_gift_cart()
        {
            string attributes = _productAttributeParser.AddGiftCardAttribute("",
                "recipientName 1", "recipientEmail@gmail.com",
                "senderName 1", "senderEmail@gmail.com", "custom message");

            var product = new Product
            {
                IsGiftCard = true,
                GiftCardType = GiftCardType.Physical,
            };
            var customer = new Customer();
            string formattedAttributes = _productAttributeFormatter.FormatAttributes(product,
                attributes, customer, "<br />", false, false, true, true);
            formattedAttributes.ShouldEqual("From: senderName 1<br />For: recipientName 1");
        }

        [Test]
        public void Can_render_attributes_withoutPrices()
        {
            string attributes = "";
            //color: green
            attributes = _productAttributeParser.AddProductAttribute(attributes, pam1_1, pav1_1.Id.ToString());
            //custom option: option 1, option 2
            attributes = _productAttributeParser.AddProductAttribute(attributes, pam2_1, pav2_1.Id.ToString());
            attributes = _productAttributeParser.AddProductAttribute(attributes, pam2_1, pav2_2.Id.ToString());
            //custom text
            attributes = _productAttributeParser.AddProductAttribute(attributes, pam3_1, "Some custom text goes here");

            //gift card attributes
            attributes = _productAttributeParser.AddGiftCardAttribute(attributes,
                "recipientName 1", "recipientEmail@gmail.com",
                "senderName 1", "senderEmail@gmail.com", "custom message");

            var product = new Product
            {
                IsGiftCard = true,
                GiftCardType = GiftCardType.Virtual,
            };
            var customer = new Customer();
            string formattedAttributes = _productAttributeFormatter.FormatAttributes(product,
                attributes, customer, "<br />", false, false, true, true);
            formattedAttributes.ShouldEqual("Color: Green<br />Some custom option: Option 1<br />Some custom option: Option 2<br />Color: Some custom text goes here<br />From: senderName 1 <senderEmail@gmail.com><br />For: recipientName 1 <recipientEmail@gmail.com>");
        }
    }
}
