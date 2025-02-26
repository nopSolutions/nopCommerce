using FluentAssertions;
using Nop.Core;
using NUnit.Framework;

namespace Nop.Tests.Nop.Core.Tests;

[TestFixture]
public class CommonHelperTests
{
    [Test]
    public void CanGetTypedValue()
    {
        CommonHelper.To<int>("1000").Should().BeOfType(typeof(int));
        CommonHelper.To<int>("1000").Should().Be(1000);
    }


    [Test]
    public void CanDeserializeEmptyString()
    {
        var deserialized = CommonHelper.DeserializeCustomValuesFromXml(string.Empty);

        deserialized.Should().NotBeNull();
        deserialized.Count.Should().Be(0);
    }

    [Test]
    public void CanDeserializeNullString()
    {
        var deserialized = CommonHelper.DeserializeCustomValuesFromXml(null);

        deserialized.Should().NotBeNull();
        deserialized.Count.Should().Be(0);
    }

    [Test]
    public void CanSerializeAndDeserializeEmptyCustomValues()
    {
        var customValues = new Dictionary<string, string>();
        var serializedXml = CommonHelper.SerializeCustomValuesToXml(customValues);
        var deserialized = CommonHelper.DeserializeCustomValuesFromXml(serializedXml);

        deserialized.Should().NotBeNull();
        deserialized.Count.Should().Be(0);
    }

    [Test]
    public void CanSerializeAndDeserializeCustomValues()
    {
        var customValues = new Dictionary<string, string>
        {
            { "key1", "value1" }, 
            { "key2", null }, 
            { "key3", "3" }, 
            { "<test key4>", "<test value 4>" }
        };

        var serializedXml = CommonHelper.SerializeCustomValuesToXml(customValues);
        var deserialized = CommonHelper.DeserializeCustomValuesFromXml(serializedXml);

        deserialized.Should().NotBeNull();
        deserialized.Count.Should().Be(4);

        deserialized.ContainsKey("key1").Should().BeTrue();
        deserialized["key1"].Should().Be("value1");

        deserialized.ContainsKey("key2").Should().BeTrue();
        //deserialized["key2"].Should().Be(null);
        deserialized["key2"].Should().Be(string.Empty);

        deserialized.ContainsKey("key3").Should().BeTrue();
        deserialized["key3"].Should().Be("3");

        deserialized.ContainsKey("<test key4>").Should().BeTrue();
        deserialized["<test key4>"].Should().Be("<test value 4>");

        serializedXml = "<?xml version=\"1.0\" encoding=\"utf-16\"?><DictionarySerializer><item><key>test_1</key><value>test 1</value></item><item><key>test_2</key><value>test 2</value></item><item><key>test_3</key><value>test 3</value></item><item><key>test_4</key><value>test 4</value></item></DictionarySerializer>";
        deserialized = CommonHelper.DeserializeCustomValuesFromXml(serializedXml);
        deserialized.Count.Should().Be(4);
        deserialized.Keys.All(k => k.StartsWith("test_")).Should().BeTrue();

        var newXml = CommonHelper.SerializeCustomValuesToXml(deserialized);
        newXml.Should().BeEquivalentTo(serializedXml);
    }
}