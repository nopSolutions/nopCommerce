using System.Collections.Generic;
using System.ComponentModel;
using Nop.Core.Domain.Shipping;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Core.Tests.Domain.Shipping
{
    [TestFixture]
    public class ShippingOptionListTypeConverterTests
    {
        [SetUp]
        public void SetUp()
        {
            TypeDescriptor.AddAttributes(typeof(List<ShippingOption>),
                new TypeConverterAttribute(typeof(ShippingOptionListTypeConverter)));
        }

        [Test]
        public void Can_get_type_converter()
        {
            var converter = TypeDescriptor.GetConverter(typeof(List<ShippingOption>));
            converter.GetType().ShouldEqual(typeof(ShippingOptionListTypeConverter));
        }

        [Test]
        public void Can_convert_shippingOptionList_to_string_and_back()
        {
            var shippingOptionsInput = new List<ShippingOption>();
            shippingOptionsInput.Add(new ShippingOption
            {
                Name = "a1",
                Description = "a2",
                Rate = 3.57M,
                ShippingRateComputationMethodSystemName = "a4"
            });
            shippingOptionsInput.Add(new ShippingOption
            {
                Name = "b1",
                Description = "b2",
                Rate = 7.00M,
                ShippingRateComputationMethodSystemName = "b4"
            });

            var converter = TypeDescriptor.GetConverter(shippingOptionsInput.GetType());
            var result = converter.ConvertTo(shippingOptionsInput, typeof(string)) as string;

            var shippingOptionsOutput = converter.ConvertFrom(result) as List<ShippingOption>;
            shippingOptionsOutput.ShouldNotBeNull();
            shippingOptionsOutput.Count.ShouldEqual(2);
            shippingOptionsOutput[0].Name.ShouldEqual("a1");
            shippingOptionsOutput[0].Description.ShouldEqual("a2");
            shippingOptionsOutput[0].Rate.ShouldEqual(3.57M);
            shippingOptionsOutput[0].ShippingRateComputationMethodSystemName.ShouldEqual("a4");

            shippingOptionsOutput[1].Name.ShouldEqual("b1");
            shippingOptionsOutput[1].Description.ShouldEqual("b2");
            shippingOptionsOutput[1].Rate.ShouldEqual(7.00M);
            shippingOptionsOutput[1].ShippingRateComputationMethodSystemName.ShouldEqual("b4");
        }
    }
}
