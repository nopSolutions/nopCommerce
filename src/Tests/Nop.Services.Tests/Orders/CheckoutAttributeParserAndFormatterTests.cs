using System.Linq;
using FluentAssertions;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Orders;
using NUnit.Framework;

namespace Nop.Services.Tests.Orders
{
    [TestFixture]
    public class CheckoutAttributeParserAndFormatterTests : ServiceTest
    {
        private ICheckoutAttributeParser _checkoutAttributeParser;
        private ICheckoutAttributeFormatter _checkoutAttributeFormatter;
        private ICheckoutAttributeService _checkoutAttributeService;

        private CheckoutAttribute _ca1, _ca2, _ca3;
        private CheckoutAttributeValue _cav11, _cav12, _cav21, _cav22;

        [SetUp]
        public void SetUp()
        {
            _checkoutAttributeParser = GetService<ICheckoutAttributeParser>();
            _checkoutAttributeFormatter = GetService<ICheckoutAttributeFormatter>();
            _checkoutAttributeService = GetService<ICheckoutAttributeService>();

            //color (dropdownlist)
            _ca1 = new CheckoutAttribute
            {
                Name = "Color",
                TextPrompt = "Select color:",
                IsRequired = true,
                AttributeControlType = AttributeControlType.DropdownList,
                DisplayOrder = 1
            };

            _checkoutAttributeService.InsertCheckoutAttribute(_ca1);

            _cav11 = new CheckoutAttributeValue
            {
                Name = "Green",
                DisplayOrder = 1,
                CheckoutAttributeId = _ca1.Id
            };
            _cav12 = new CheckoutAttributeValue
            {
                Name = "Red",
                DisplayOrder = 2,
                CheckoutAttributeId = _ca1.Id
            };

            _checkoutAttributeService.InsertCheckoutAttributeValue(_cav11);
            _checkoutAttributeService.InsertCheckoutAttributeValue(_cav12);

            //custom option (checkboxes)
            _ca2 = new CheckoutAttribute
            {
                Name = "Custom option",
                TextPrompt = "Select custom option:",
                IsRequired = true,
                AttributeControlType = AttributeControlType.Checkboxes,
                DisplayOrder = 2
            };

            _checkoutAttributeService.InsertCheckoutAttribute(_ca2);

            _cav21 = new CheckoutAttributeValue
            {
                Name = "Option 1",
                DisplayOrder = 1,
                CheckoutAttributeId = _ca2.Id
            };
            _cav22 = new CheckoutAttributeValue
            {
                Name = "Option 2",
                DisplayOrder = 2,
                CheckoutAttributeId = _ca2.Id
            };

            _checkoutAttributeService.InsertCheckoutAttributeValue(_cav21);
            _checkoutAttributeService.InsertCheckoutAttributeValue(_cav22);

            //custom text
            _ca3 = new CheckoutAttribute
            {
                Name = "Custom text",
                TextPrompt = "Enter custom text:",
                IsRequired = true,
                AttributeControlType = AttributeControlType.MultilineTextbox,
                DisplayOrder = 3
            };

            _checkoutAttributeService.InsertCheckoutAttribute(_ca3);
        }

        [TearDown]
        public void TearDown()
        {
            _checkoutAttributeService.DeleteCheckoutAttributeValue(_cav11);
            _checkoutAttributeService.DeleteCheckoutAttributeValue(_cav12);
            _checkoutAttributeService.DeleteCheckoutAttribute(_ca1);

            _checkoutAttributeService.DeleteCheckoutAttributeValue(_cav21);
            _checkoutAttributeService.DeleteCheckoutAttributeValue(_cav22);
            _checkoutAttributeService.DeleteCheckoutAttribute(_ca2);

            _checkoutAttributeService.DeleteCheckoutAttribute(_ca3);
        }

        [Test]
        public void CanAddAndParseCheckoutAttributes()
        {
            var attributes = string.Empty;
            //color: green
            attributes = _checkoutAttributeParser.AddCheckoutAttribute(attributes, _ca1, _cav11.Id.ToString());
            //custom option: option 1, option 2
            attributes = _checkoutAttributeParser.AddCheckoutAttribute(attributes, _ca2, _cav21.Id.ToString());
            attributes = _checkoutAttributeParser.AddCheckoutAttribute(attributes, _ca2, _cav22.Id.ToString());
            //custom text
            attributes = _checkoutAttributeParser.AddCheckoutAttribute(attributes, _ca3, "Some custom text goes here");

            var parsedAttributeValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(attributes).ToList();
            parsedAttributeValues.SelectMany(x => x.values.Select(p => p.Id)).Contains(_cav11.Id).Should().BeTrue();
            parsedAttributeValues.SelectMany(x => x.values.Select(p => p.Id)).Contains(_cav12.Id).Should().BeFalse();
            parsedAttributeValues.SelectMany(x => x.values.Select(p => p.Id)).Contains(_cav21.Id).Should().BeTrue();
            parsedAttributeValues.SelectMany(x => x.values.Select(p => p.Id)).Contains(_cav22.Id).Should().BeTrue();
            parsedAttributeValues.SelectMany(x => x.values.Select(p => p.Id)).Contains(_cav22.Id).Should().BeTrue();

            var parsedValues = _checkoutAttributeParser.ParseValues(attributes, _ca3.Id);
            parsedValues.Count.Should().Be(1);
            parsedValues.Contains("Some custom text goes here").Should().BeTrue();
            parsedValues.Contains("Some other custom text").Should().BeFalse();
        }

        [Test]
        public void CanAddRenderAttributesWithoutPrices()
        {
            var attributes = string.Empty;
            //color: green
            attributes = _checkoutAttributeParser.AddCheckoutAttribute(attributes, _ca1, _cav11.Id.ToString());
            //custom option: option 1, option 2
            attributes = _checkoutAttributeParser.AddCheckoutAttribute(attributes, _ca2, _cav21.Id.ToString());
            attributes = _checkoutAttributeParser.AddCheckoutAttribute(attributes, _ca2, _cav22.Id.ToString());
            //custom text
            attributes = _checkoutAttributeParser.AddCheckoutAttribute(attributes, _ca3, "Some custom text goes here");

            var customer = new Customer();

            var formattedAttributes =
                _checkoutAttributeFormatter.FormatAttributes(attributes, customer, "<br />", false, false);
            formattedAttributes.Should()
                .Be(
                    "Color: Green<br />Custom option: Option 1<br />Custom option: Option 2<br />Custom text: Some custom text goes here");
        }

        [Test]
        public void CanAddAndRemoveCheckoutAttributes()
        {
            var attributes = string.Empty;
            //color: green
            attributes = _checkoutAttributeParser.AddCheckoutAttribute(attributes, _ca1, _cav11.Id.ToString());
            //custom option: option 1, option 2
            attributes = _checkoutAttributeParser.AddCheckoutAttribute(attributes, _ca2, _cav21.Id.ToString());
            attributes = _checkoutAttributeParser.AddCheckoutAttribute(attributes, _ca2, _cav22.Id.ToString());
            //custom text
            attributes = _checkoutAttributeParser.AddCheckoutAttribute(attributes, _ca3, "Some custom text goes here");
            //delete some of them
            attributes = _checkoutAttributeParser.RemoveCheckoutAttribute(attributes, _ca2);
            attributes = _checkoutAttributeParser.RemoveCheckoutAttribute(attributes, _ca3);

            var parsedAttributeValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(attributes).ToList();
            parsedAttributeValues.SelectMany(x => x.values.Select(p => p.Id)).Contains(_cav11.Id).Should().BeTrue();
            parsedAttributeValues.SelectMany(x => x.values.Select(p => p.Id)).Contains(_cav12.Id).Should().BeFalse();
            parsedAttributeValues.SelectMany(x => x.values.Select(p => p.Id)).Contains(_cav21.Id).Should().BeFalse();
            parsedAttributeValues.SelectMany(x => x.values.Select(p => p.Id)).Contains(_cav22.Id).Should().BeFalse();
            parsedAttributeValues.SelectMany(x => x.values.Select(p => p.Id)).Contains(_cav22.Id).Should().BeFalse();

            var parsedValues = _checkoutAttributeParser.ParseValues(attributes, _ca3.Id);
            parsedValues.Count.Should().Be(0);
        }
    }
}
