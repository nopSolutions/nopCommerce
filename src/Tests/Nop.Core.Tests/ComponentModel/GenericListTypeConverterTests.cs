using System.Collections.Generic;
using System.ComponentModel;
using Nop.Core.ComponentModel;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Core.Tests.ComponentModel
{
    [TestFixture]
    public class GenericListTypeConverter
    {
        [SetUp]
        public void SetUp()
        {
            TypeDescriptor.AddAttributes(typeof(List<int>),
                new TypeConverterAttribute(typeof(GenericListTypeConverter<int>)));
            TypeDescriptor.AddAttributes(typeof(List<string>),
                new TypeConverterAttribute(typeof(GenericListTypeConverter<string>)));
        }

        [Test]
        public void Can_get_int_list_type_converter()
        {
            var converter = TypeDescriptor.GetConverter(typeof(List<int>));
            converter.GetType().ShouldEqual(typeof(GenericListTypeConverter<int>));
        }

        [Test]
        public void Can_get_string_list_type_converter()
        {
            var converter = TypeDescriptor.GetConverter(typeof(List<string>));
            converter.GetType().ShouldEqual(typeof(GenericListTypeConverter<string>));
        }

        [Test]
        public void Can_get_int_list_from_string()
        {
            var items = "10,20,30,40,50";
            var converter = TypeDescriptor.GetConverter(typeof(List<int>));
            var result = converter.ConvertFrom(items) as IList<int>;
            result.ShouldNotBeNull();
            result.Count.ShouldEqual(5);
        }

        [Test]
        public void Can_get_string_list_from_string()
        {
            var items = "foo, bar, day";
            var converter = TypeDescriptor.GetConverter(typeof(List<string>));
            var result = converter.ConvertFrom(items) as List<string>;
            result.ShouldNotBeNull();
            result.Count.ShouldEqual(3);
        }

        [Test]
        public void Can_convert_int_list_to_string()
        {
            var items = new List<int> { 10, 20, 30, 40, 50 };
            var converter = TypeDescriptor.GetConverter(items.GetType());
            var result = converter.ConvertTo(items, typeof(string)) as string;

            result.ShouldNotBeNull();
            result.ShouldEqual("10,20,30,40,50");
        }

        [Test]
        public void Can_convert_string_list_to_string()
        {
            var items = new List<string> { "foo", "bar", "day" };
            var converter = TypeDescriptor.GetConverter(items.GetType());
            var result = converter.ConvertTo(items, typeof(string)) as string;
            result.ShouldNotBeNull();
            result.ShouldEqual("foo,bar,day");
        }
    }
}
