using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Orders;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Orders
{
    [TestFixture]
    public class CheckoutAttributeParserAndFormatterTests : ServiceTest
    {
        private ICheckoutAttributeParser _checkoutAttributeParser;
        private ICheckoutAttributeFormatter _checkoutAttributeFormatter;
        private ICheckoutAttributeService _checkoutAttributeService;

        private CheckoutAttribute _ca1, _ca2, _ca3;
        private CheckoutAttributeValue _cav11, _cav12, _cav21, _cav22;

        [OneTimeSetUp]
        public async Task SetUp()
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

            await _checkoutAttributeService.InsertCheckoutAttributeAsync(_ca1);

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

            await _checkoutAttributeService.InsertCheckoutAttributeValueAsync(_cav11);
            await _checkoutAttributeService.InsertCheckoutAttributeValueAsync(_cav12);

            //custom option (checkboxes)
            _ca2 = new CheckoutAttribute
            {
                Name = "Custom option",
                TextPrompt = "Select custom option:",
                IsRequired = true,
                AttributeControlType = AttributeControlType.Checkboxes,
                DisplayOrder = 2
            };

            await _checkoutAttributeService.InsertCheckoutAttributeAsync(_ca2);

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

            await _checkoutAttributeService.InsertCheckoutAttributeValueAsync(_cav21);
            await _checkoutAttributeService.InsertCheckoutAttributeValueAsync(_cav22);

            //custom text
            _ca3 = new CheckoutAttribute
            {
                Name = "Custom text",
                TextPrompt = "Enter custom text:",
                IsRequired = true,
                AttributeControlType = AttributeControlType.MultilineTextbox,
                DisplayOrder = 3
            };

            await _checkoutAttributeService.InsertCheckoutAttributeAsync(_ca3);
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await _checkoutAttributeService.DeleteCheckoutAttributeValueAsync(_cav11);
            await _checkoutAttributeService.DeleteCheckoutAttributeValueAsync(_cav12);
            await _checkoutAttributeService.DeleteCheckoutAttributeAsync(_ca1);

            await _checkoutAttributeService.DeleteCheckoutAttributeValueAsync(_cav21);
            await _checkoutAttributeService.DeleteCheckoutAttributeValueAsync(_cav22);
            await _checkoutAttributeService.DeleteCheckoutAttributeAsync(_ca2);

            await _checkoutAttributeService.DeleteCheckoutAttributeAsync(_ca3);
        }

        [Test]
        public async Task CanAddAndParseCheckoutAttributes()
        {
            var attributes = string.Empty;
            //color: green
            attributes = _checkoutAttributeParser.AddCheckoutAttribute(attributes, _ca1, _cav11.Id.ToString());
            //custom option: option 1, option 2
            attributes = _checkoutAttributeParser.AddCheckoutAttribute(attributes, _ca2, _cav21.Id.ToString());
            attributes = _checkoutAttributeParser.AddCheckoutAttribute(attributes, _ca2, _cav22.Id.ToString());
            //custom text
            attributes = _checkoutAttributeParser.AddCheckoutAttribute(attributes, _ca3, "Some custom text goes here");

            var parsedAttributeValues = await _checkoutAttributeParser.ParseCheckoutAttributeValues(attributes).ToListAsync();
            var attributeValues = await parsedAttributeValues.SelectAwait(async x => await x.values.Select(p => p.Id).ToListAsync()).SelectMany(p=>p.ToAsyncEnumerable()).ToListAsync();
            
            attributeValues.Contains(_cav11.Id).Should().BeTrue();
            attributeValues.Contains(_cav12.Id).Should().BeFalse();
            attributeValues.Contains(_cav21.Id).Should().BeTrue();
            attributeValues.Contains(_cav22.Id).Should().BeTrue();
            attributeValues.Contains(_cav22.Id).Should().BeTrue();

            var parsedValues = _checkoutAttributeParser.ParseValues(attributes, _ca3.Id);
            parsedValues.Count.Should().Be(1);
            parsedValues.Contains("Some custom text goes here").Should().BeTrue();
            parsedValues.Contains("Some other custom text").Should().BeFalse();
        }

        [Test]
        public async Task CanAddRenderAttributesWithoutPrices()
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
                await _checkoutAttributeFormatter.FormatAttributesAsync(attributes, customer, "<br />", false, false);
            formattedAttributes.Should()
                .Be(
                    "Color: Green<br />Custom option: Option 1<br />Custom option: Option 2<br />Custom text: Some custom text goes here");
        }

        [Test]
        public async Task CanAddAndRemoveCheckoutAttributes()
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

            var parsedAttributeValues = await _checkoutAttributeParser.ParseCheckoutAttributeValues(attributes).ToListAsync();
            var attributeValues = await parsedAttributeValues.SelectAwait(async x => await x.values.Select(p => p.Id).ToListAsync()).SelectMany(p => p.ToAsyncEnumerable()).ToListAsync();

            attributeValues.Contains(_cav11.Id).Should().BeTrue();
            attributeValues.Contains(_cav12.Id).Should().BeFalse();
            attributeValues.Contains(_cav21.Id).Should().BeFalse();
            attributeValues.Contains(_cav22.Id).Should().BeFalse();
            attributeValues.Contains(_cav22.Id).Should().BeFalse();

            var parsedValues = _checkoutAttributeParser.ParseValues(attributes, _ca3.Id);
            parsedValues.Count.Should().Be(0);
        }
    }
}
