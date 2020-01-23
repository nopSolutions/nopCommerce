using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using Nop.Core;
using Nop.Data;
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
using NUnit.Framework;

namespace Nop.Services.Tests.Catalog
{
    [TestFixture]
    public class ProductAttributeParserTests : ServiceTest
    {
        private Mock<IRepository<ProductAttribute>> _productAttributeRepo;
        private Mock<IRepository<ProductAttributeMapping>> _productAttributeMappingRepo;
        private Mock<IRepository<ProductAttributeCombination>> _productAttributeCombinationRepo;
        private Mock<IRepository<ProductAttributeValue>> _productAttributeValueRepo;
        private Mock<IRepository<PredefinedProductAttributeValue>> _predefinedProductAttributeValueRepo;
        private IProductAttributeService _productAttributeService;
        private IProductAttributeParser _productAttributeParser;
        private Mock<IEventPublisher> _eventPublisher;
        private Mock<IWorkContext> _workContext;
        private Mock<ICurrencyService> _currencyService;
        private ILocalizationService _localizationService;
        private Mock<ITaxService> _taxService;
        private Mock<IPriceFormatter> _priceFormatter;
        private Mock<IPriceCalculationService> _priceCalculationService;
        private Mock<IDownloadService> _downloadService;
        private Mock<IWebHelper> _webHelper;
        private ShoppingCartSettings _shoppingCartSettings;
        private IProductAttributeFormatter _productAttributeFormatter;

        private ProductAttribute pa1, pa2, pa3, pa4;
        private ProductAttributeMapping pam1_1, pam2_1, pam3_1, pam4_1;
        private ProductAttributeValue pav1_1, pav1_2, pav2_1, pav2_2, pav4_1;

        [SetUp]
        public new void SetUp()
        {
            #region Test data

            //color (dropdownlist)
            pa1 = new ProductAttribute
            {
                Id = 1,
                Name = "Color"
            };
            pam1_1 = new ProductAttributeMapping
            {
                Id = 11,
                ProductId = 1,
                TextPrompt = "Select color:",
                IsRequired = true,
                AttributeControlType = AttributeControlType.DropdownList,
                DisplayOrder = 1,
                ProductAttributeId = pa1.Id
            };
            pav1_1 = new ProductAttributeValue
            {
                Id = 11,
                Name = "Green",
                DisplayOrder = 1,
                ProductAttributeMappingId = pam1_1.Id
            };
            pav1_2 = new ProductAttributeValue
            {
                Id = 12,
                Name = "Red",
                DisplayOrder = 2,
                ProductAttributeMappingId = pam1_1.Id
            };

            //custom option (checkboxes)
            pa2 = new ProductAttribute
            {
                Id = 2,
                Name = "Some custom option"
            };
            pam2_1 = new ProductAttributeMapping
            {
                Id = 21,
                ProductId = 1,
                TextPrompt = "Select at least one option:",
                IsRequired = true,
                AttributeControlType = AttributeControlType.Checkboxes,
                DisplayOrder = 2,
                ProductAttributeId = pa2.Id
            };
            pav2_1 = new ProductAttributeValue
            {
                Id = 21,
                Name = "Option 1",
                DisplayOrder = 1,
                ProductAttributeMappingId = pam2_1.Id
            };
            pav2_2 = new ProductAttributeValue
            {
                Id = 22,
                Name = "Option 2",
                DisplayOrder = 2,
                ProductAttributeMappingId = pam2_1.Id
            };

            //custom text
            pa3 = new ProductAttribute
            {
                Id = 3,
                Name = "Custom text"
            };
            pam3_1 = new ProductAttributeMapping
            {
                Id = 31,
                ProductId = 1,
                TextPrompt = "Enter custom text:",
                IsRequired = true,
                AttributeControlType = AttributeControlType.TextBox,
                DisplayOrder = 1,
                ProductAttributeId = pa3.Id
            };

            //option radio
            pa4 = new ProductAttribute
            {
                Id = 4,
                Name = "Radio list"
            };
            pam4_1 = new ProductAttributeMapping
            {
                Id = 41,
                ProductId = 1,
                TextPrompt = "Select option and enter the quantity:",
                IsRequired = true,
                AttributeControlType = AttributeControlType.RadioList,
                DisplayOrder = 2,
                ProductAttributeId = pa4.Id
            };
            pav4_1 = new ProductAttributeValue
            {
                Id = 41,
                Name = "Option with quantity",
                DisplayOrder = 1,
                ProductAttributeMappingId = pam4_1.Id
            };

            #endregion

            _productAttributeRepo = new Mock<IRepository<ProductAttribute>>();
            _productAttributeRepo.Setup(x => x.Table).Returns(new List<ProductAttribute> { pa1, pa2, pa3, pa4 }.AsQueryable());
            _productAttributeRepo.Setup(x => x.GetById(pa1.Id)).Returns(pa1);
            _productAttributeRepo.Setup(x => x.GetById(pa2.Id)).Returns(pa2);
            _productAttributeRepo.Setup(x => x.GetById(pa3.Id)).Returns(pa3);
            _productAttributeRepo.Setup(x => x.GetById(pa4.Id)).Returns(pa4);

            _productAttributeMappingRepo = new Mock<IRepository<ProductAttributeMapping>>();
            _productAttributeMappingRepo.Setup(x => x.Table).Returns(new List<ProductAttributeMapping> { pam1_1, pam2_1, pam3_1, pam4_1 }.AsQueryable());
            _productAttributeMappingRepo.Setup(x => x.GetById(pam1_1.Id)).Returns(pam1_1);
            _productAttributeMappingRepo.Setup(x => x.GetById(pam2_1.Id)).Returns(pam2_1);
            _productAttributeMappingRepo.Setup(x => x.GetById(pam3_1.Id)).Returns(pam3_1);
            _productAttributeMappingRepo.Setup(x => x.GetById(pam4_1.Id)).Returns(pam4_1);

            _productAttributeCombinationRepo = new Mock<IRepository<ProductAttributeCombination>>();
            _productAttributeCombinationRepo.Setup(x => x.Table).Returns(new List<ProductAttributeCombination>().AsQueryable());

            _productAttributeValueRepo = new Mock<IRepository<ProductAttributeValue>>();
            _productAttributeValueRepo.Setup(x => x.Table).Returns(new List<ProductAttributeValue> { pav1_1, pav1_2, pav2_1, pav2_2, pav4_1 }.AsQueryable());
            _productAttributeValueRepo.Setup(x => x.GetById(pav1_1.Id)).Returns(pav1_1);
            _productAttributeValueRepo.Setup(x => x.GetById(pav1_2.Id)).Returns(pav1_2);
            _productAttributeValueRepo.Setup(x => x.GetById(pav2_1.Id)).Returns(pav2_1);
            _productAttributeValueRepo.Setup(x => x.GetById(pav2_2.Id)).Returns(pav2_2);
            _productAttributeValueRepo.Setup(x => x.GetById(pav4_1.Id)).Returns(pav4_1);

            _predefinedProductAttributeValueRepo = new Mock<IRepository<PredefinedProductAttributeValue>>();

            _eventPublisher = new Mock<IEventPublisher>();
            _eventPublisher.Setup(x => x.Publish(It.IsAny<object>()));

            _productAttributeService = new ProductAttributeService(_eventPublisher.Object,
                _predefinedProductAttributeValueRepo.Object,
                _productAttributeRepo.Object,
                _productAttributeCombinationRepo.Object,
                _productAttributeMappingRepo.Object,
                _productAttributeValueRepo.Object);

            _priceCalculationService = new Mock<IPriceCalculationService>();

            var workingLanguage = new Language();
            _workContext = new Mock<IWorkContext>();
            _workContext.Setup(x => x.WorkingLanguage).Returns(workingLanguage);
            _currencyService = new Mock<ICurrencyService>();
            _localizationService = TestLocalizationService.Init();
            
            _taxService = new Mock<ITaxService>();
            _priceFormatter = new Mock<IPriceFormatter>();
            _downloadService = new Mock<IDownloadService>();
            _webHelper = new Mock<IWebHelper>();
            _shoppingCartSettings = new ShoppingCartSettings();

            _productAttributeParser = new ProductAttributeParser(_currencyService.Object,
                _downloadService.Object,
                _localizationService,
                _productAttributeService,
                _productAttributeValueRepo.Object,
                _workContext.Object);

            _productAttributeFormatter = new ProductAttributeFormatter(_currencyService.Object,
                _downloadService.Object,
                _localizationService,
                _priceCalculationService.Object,
                _priceFormatter.Object,
                _productAttributeParser,
                _productAttributeService,
                _taxService.Object,
                _webHelper.Object,
                _workContext.Object,
                _shoppingCartSettings);
        }

        [Test]
        public void Can_add_and_parse_productAttributes()
        {
            var attributes = "";
            //color: green
            attributes = _productAttributeParser.AddProductAttribute(attributes, pam1_1, pav1_1.Id.ToString());
            //custom option: option 1, option 2
            attributes = _productAttributeParser.AddProductAttribute(attributes, pam2_1, pav2_1.Id.ToString());
            attributes = _productAttributeParser.AddProductAttribute(attributes, pam2_1, pav2_2.Id.ToString());
            //custom text
            attributes = _productAttributeParser.AddProductAttribute(attributes, pam3_1, "Some custom text goes here");

            RunWithTestServiceProvider(() =>
            {
                var parsedAttributeValues = _productAttributeParser.ParseProductAttributeValues(attributes);
                parsedAttributeValues.Contains(pav1_1).Should().BeTrue();
                parsedAttributeValues.Contains(pav1_2).Should().BeFalse();
                parsedAttributeValues.Contains(pav2_1).Should().BeTrue();
                parsedAttributeValues.Contains(pav2_2).Should().BeTrue();
                parsedAttributeValues.Contains(pav2_2).Should().BeTrue();
            });

            var parsedValues = _productAttributeParser.ParseValues(attributes, pam3_1.Id);
            parsedValues.Count.Should().Be(1);
            parsedValues.Contains("Some custom text goes here").Should().BeTrue();
            parsedValues.Contains("Some other custom text").Should().BeFalse();
        }

        [Test]
        public void Can_add_and_remove_productAttributes()
        {
            var attributes = "";
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

            RunWithTestServiceProvider(() =>
            {
                var parsedAttributeValues = _productAttributeParser.ParseProductAttributeValues(attributes);
                parsedAttributeValues.Contains(pav1_1).Should().BeTrue();
                parsedAttributeValues.Contains(pav1_2).Should().BeFalse();
                parsedAttributeValues.Contains(pav2_1).Should().BeFalse();
                parsedAttributeValues.Contains(pav2_2).Should().BeFalse();
                parsedAttributeValues.Contains(pav2_2).Should().BeFalse();
            });

            var parsedValues = _productAttributeParser.ParseValues(attributes, pam3_1.Id);
            parsedValues.Count.Should().Be(0);
        }

        [Test]
        public void Can_add_and_parse_giftCardAttributes()
        {
            var attributes = "";
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
        public void Can_render_virtual_gift_cart()
        {
            var attributes = _productAttributeParser.AddGiftCardAttribute("",
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
        public void Can_render_physical_gift_cart()
        {
            var attributes = _productAttributeParser.AddGiftCardAttribute("",
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

        [Test]
        public void Can_render_attributes_withoutPrices()
        {
            var attributes = "";
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
                GiftCardType = GiftCardType.Virtual
            };
            var customer = new Customer();

            RunWithTestServiceProvider(() =>
            {
                var formattedAttributes = _productAttributeFormatter.FormatAttributes(product,
                    attributes, customer, "<br />", false, false);
                formattedAttributes.Should().Be("Color: Green<br />Some custom option: Option 1<br />Some custom option: Option 2<br />Custom text: Some custom text goes here<br />From: senderName 1 <senderEmail@gmail.com><br />For: recipientName 1 <recipientEmail@gmail.com>");
            });
        }
    }
}

