using FluentAssertions;
using Nop.Services.Orders;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Orders;

[TestFixture]
public class OrderCustomValuesTests
{
    [Test]
    public void CanDeserializeEmptyString()
    {
        var deserialized = new CustomValues();
        deserialized.FillByXml(string.Empty);

        deserialized.Should().NotBeNull();
        deserialized.Any().Should().Be(false);
    }

    [Test]
    public void CanSerializeAndDeserializeEmptyCustomValues()
    {
        var customValues = new CustomValues();
        var serializedXml = customValues.SerializeToXml();
        var deserialized = new CustomValues();
        deserialized.FillByXml(serializedXml);

        deserialized.Should().NotBeNull();
        deserialized.Any().Should().Be(false);
    }

    [Test]
    public void CanSerializeAndDeserializeCustomValues()
    {
        var customValues = new CustomValues
        {
            new("key1", "value1", CustomValueDisplayLocation.Shipping), 
            new("key2", null, displayToCustomer:false), 
            new("key3", "3"),
            new("<test key4>", "<test value 4>")
        };

        var serializedXml = customValues.SerializeToXml();

        var deserialized = new CustomValues();
        deserialized.FillByXml(serializedXml);

        deserialized.Should().NotBeNull();
        deserialized.Count.Should().Be(4);

        var customValue = deserialized["key1"];
        customValue.Should().NotBeNullOrEmpty();
        customValue.Should().Be("value1");
        deserialized.TryGetValue("key1", out var customValueObj);
        customValueObj.DisplayToCustomer.Should().Be(true);
        customValueObj.DisplayLocation.Should().Be(CustomValueDisplayLocation.Shipping);

        customValue = deserialized["key2"];
        customValue.Should().NotBeNull();
        customValue.Should().Be(string.Empty);
        deserialized.TryGetValue("key2", out customValueObj);
        customValueObj.DisplayToCustomer.Should().Be(false);
        customValueObj.DisplayLocation.Should().Be(CustomValueDisplayLocation.Payment);

        customValue = deserialized["key3"];
        customValue.Should().NotBeNullOrEmpty();
        customValue.Should().Be("3");
        deserialized.TryGetValue("key3", out customValueObj);
        customValueObj.DisplayToCustomer.Should().Be(true);
        customValueObj.DisplayLocation.Should().Be(CustomValueDisplayLocation.Payment);


        customValue = deserialized["<test key4>"];
        customValue.Should().NotBeNullOrEmpty();
        customValue.Should().Be("<test value 4>");

        serializedXml = "<?xml version=\"1.0\" encoding=\"utf-16\"?><DictionarySerializer><item><key>test_1</key><value>test 1</value></item><item><key>test_2</key><value>test 2</value></item><item><key>test_3</key><value>test 3</value></item><item><key>test_4</key><value>test 4</value></item></DictionarySerializer>";
        deserialized.FillByXml(serializedXml);
        deserialized.Count.Should().Be(4);

        foreach (var orderCustomValue in deserialized) 
            orderCustomValue.Name.StartsWith("test_").Should().BeTrue();

        var newXml = deserialized.SerializeToXml();
        newXml.Should().NotBeEquivalentTo(serializedXml);
        newXml.Should().BeEquivalentTo("<?xml version=\"1.0\" encoding=\"utf-16\"?><DictionarySerializer><item><key>test_1</key><value>test 1</value><displayToCustomer>true</displayToCustomer><location>2</location><createdOn>0</createdOn></item><item><key>test_2</key><value>test 2</value><displayToCustomer>true</displayToCustomer><location>2</location><createdOn>0</createdOn></item><item><key>test_3</key><value>test 3</value><displayToCustomer>true</displayToCustomer><location>2</location><createdOn>0</createdOn></item><item><key>test_4</key><value>test 4</value><displayToCustomer>true</displayToCustomer><location>2</location><createdOn>0</createdOn></item></DictionarySerializer>");
    }
}
