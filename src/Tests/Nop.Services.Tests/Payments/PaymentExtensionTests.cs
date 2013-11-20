using Nop.Services.Payments;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Payments
{
    [TestFixture]
    public class PaymentExtensionTests : ServiceTest
    {
        [Test]
        public void Can_deserialize_empty_string()
        {
            var processPaymentRequest = new ProcessPaymentRequest();
            var deserialized = processPaymentRequest.DeserializeCustomValues("");

            deserialized.ShouldNotBeNull();
            deserialized.Count.ShouldEqual(0);
        }

        [Test]
        public void Can_deserialize_null_string()
        {
            var processPaymentRequest = new ProcessPaymentRequest();
            var deserialized = processPaymentRequest.DeserializeCustomValues(null);

            deserialized.ShouldNotBeNull();
            deserialized.Count.ShouldEqual(0);
        }

        [Test]
        public void Can_serialize_and_deserialize_empty_CustomValues()
        {
            var processPaymentRequest = new ProcessPaymentRequest();
            var serializedXml = processPaymentRequest.SerializeCustomValues();
            var deserialized = processPaymentRequest.DeserializeCustomValues(serializedXml);

            deserialized.ShouldNotBeNull();
            deserialized.Count.ShouldEqual(0);
        }

        [Test]
        public void Can_serialize_and_deserialize_CustomValues()
        {
            var processPaymentRequest = new ProcessPaymentRequest();
            processPaymentRequest.CustomValues.Add("key1", "value1");
            processPaymentRequest.CustomValues.Add("key2", null);
            processPaymentRequest.CustomValues.Add("key3", 3);
            processPaymentRequest.CustomValues.Add("<test key4>", "<test value 4>");
            var serializedXml = processPaymentRequest.SerializeCustomValues();
            var deserialized = processPaymentRequest.DeserializeCustomValues(serializedXml);

            deserialized.ShouldNotBeNull();
            deserialized.Count.ShouldEqual(4);

            deserialized.ContainsKey("key1").ShouldEqual(true);
            deserialized["key1"].ShouldEqual("value1");

            deserialized.ContainsKey("key2").ShouldEqual(true);
            //deserialized["key2"].ShouldEqual(null);
            deserialized["key2"].ShouldEqual("");

            deserialized.ContainsKey("key3").ShouldEqual(true);
            deserialized["key3"].ShouldEqual("3");

            deserialized.ContainsKey("<test key4>").ShouldEqual(true);
            deserialized["<test key4>"].ShouldEqual("<test value 4>");
        }
    }
}
