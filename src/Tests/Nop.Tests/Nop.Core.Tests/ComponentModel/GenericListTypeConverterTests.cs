using System.ComponentModel;
using FluentAssertions;
using Nop.Core.ComponentModel;
using NUnit.Framework;

namespace Nop.Tests.Nop.Core.Tests.ComponentModel;

[TestFixture]
public class GenericListTypeConverter
{
    [OneTimeSetUp]
    public void SetUp()
    {
        TypeDescriptor.AddAttributes(typeof(List<int>),
            new TypeConverterAttribute(typeof(GenericListTypeConverter<int>)));
        TypeDescriptor.AddAttributes(typeof(List<string>),
            new TypeConverterAttribute(typeof(GenericListTypeConverter<string>)));
    }

    [Test]
    public void CanGetIntListTypeConverter()
    {
        var converter = TypeDescriptor.GetConverter(typeof(List<int>));
        converter.Should().BeOfType<GenericListTypeConverter<int>>();
    }

    [Test]
    public void CanGetStringListTypeConverter()
    {
        var converter = TypeDescriptor.GetConverter(typeof(List<string>));
        converter.Should().BeOfType<GenericListTypeConverter<string>>();
    }

    [Test]
    public void CanGetIntListFromString()
    {
        var items = "10,20,30,40,50";
        var converter = TypeDescriptor.GetConverter(typeof(List<int>));
        var result = converter.ConvertFrom(items) as IList<int>;
        result.Should().NotBeNull();
        result.Count.Should().Be(5);
    }

    [Test]
    public void CanGetStringListFromString()
    {
        var items = "foo, bar, day";
        var converter = TypeDescriptor.GetConverter(typeof(List<string>));
        var result = converter.ConvertFrom(items) as List<string>;
        result.Should().NotBeNull();
        result.Count.Should().Be(3);
    }

    [Test]
    public void CanConvertIntListToString()
    {
        var items = new List<int> { 10, 20, 30, 40, 50 };
        var converter = TypeDescriptor.GetConverter(items.GetType());
        var result = converter.ConvertTo(items, typeof(string)) as string;

        result.Should().NotBeNull();
        result.Should().Be("10,20,30,40,50");
    }

    [Test]
    public void CanConvertStringListToString()
    {
        var items = new List<string> { "foo", "bar", "day" };
        var converter = TypeDescriptor.GetConverter(items.GetType());
        var result = converter.ConvertTo(items, typeof(string)) as string;
        result.Should().NotBeNull();
        result.Should().Be("foo,bar,day");
    }
    
    [Test]
    public void CanConvertNullToString()
    {
        var converter = TypeDescriptor.GetConverter(typeof(List<string>));
        var result = converter.ConvertTo(null, typeof(string)) as string;
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}