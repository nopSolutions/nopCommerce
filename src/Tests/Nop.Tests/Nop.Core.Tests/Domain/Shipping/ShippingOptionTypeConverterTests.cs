using System.ComponentModel;
using FluentAssertions;
using Nop.Core.Domain.Shipping;
using NUnit.Framework;

namespace Nop.Tests.Nop.Core.Tests.Domain.Shipping
{
    [TestFixture]
    public class ShippingOptionTypeConverterTests
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TypeDescriptor.AddAttributes(typeof(ShippingOption),
                new TypeConverterAttribute(typeof(ShippingOptionTypeConverter)));
        }

        [Test]
        public void CanGetTypeConverter()
        {
            var converter = TypeDescriptor.GetConverter(typeof(ShippingOption));
            converter.GetType().Should().Be(typeof(ShippingOptionTypeConverter));
        }

        [Test]
        public void CanConvertShippingOptionToStringAndBack()
        {
            var shippingOptionInput = new ShippingOption
            {
                Name = "1",
                Description = "2",
                Rate = 3.57M,
                ShippingRateComputationMethodSystemName = "4"
            };
            var converter = TypeDescriptor.GetConverter(shippingOptionInput.GetType());
            var result = converter.ConvertTo(shippingOptionInput, typeof(string)) as string;

            var shippingOptionOutput = converter.ConvertFrom(result) as ShippingOption;
            shippingOptionOutput.Should().NotBeNull();
            shippingOptionOutput.Name.Should().Be("1");
            shippingOptionOutput.Description.Should().Be("2");
            shippingOptionOutput.Rate.Should().Be(3.57M);
            shippingOptionOutput.ShippingRateComputationMethodSystemName.Should().Be("4");
        }
    }
}
