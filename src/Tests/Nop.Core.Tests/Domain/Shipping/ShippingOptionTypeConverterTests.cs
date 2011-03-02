using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Nop.Core.Domain.Shipping;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.ComponentModel;

namespace Nop.Core.Tests.Domain.Shipping
{
    [TestFixture]
    public class ShippingOptionTypeConverterTests
    {
        [SetUp]
        public void SetUp()
        {
            TypeDescriptor.AddAttributes(typeof(ShippingOption),
                new TypeConverterAttribute(typeof(ShippingOptionTypeConverter)));
        }

        [Test]
        public void Can_get_int_list_type_converter()
        {
            var converter = TypeDescriptor.GetConverter(typeof(ShippingOption));
            converter.GetType().ShouldEqual(typeof(ShippingOptionTypeConverter));
        }

        [Test]
        public void Can_convert_shippingOption_to_string_and_back()
        {
            var shippingOptionInput = new ShippingOption()
            {
                Name = "1",
                Description = "2",
                Rate = 3.57M,
                ShippingRateComputationMethodSystemName = "4"
            };
            var converter = TypeDescriptor.GetConverter(shippingOptionInput.GetType());
            var result = converter.ConvertTo(shippingOptionInput, typeof(string)) as string;

            var shippingOptionOutput = converter.ConvertFrom(result) as ShippingOption;
            shippingOptionOutput.ShouldNotBeNull();
            shippingOptionOutput.Name.ShouldEqual("1");
            shippingOptionOutput.Description.ShouldEqual("2");
            shippingOptionOutput.Rate.ShouldEqual(3.57M);
            shippingOptionOutput.ShippingRateComputationMethodSystemName.ShouldEqual("4");
        }
    }
}
