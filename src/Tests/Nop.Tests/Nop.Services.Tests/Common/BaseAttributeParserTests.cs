using System.Xml;
using FluentAssertions;
using Nop.Services.Common;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Common
{
    [TestFixture]
    public class BaseAttributeParserTests : BaseAttributeParser
    {
        private string _attributesXml;
        private XmlDocument _xmlDoc;

        protected override string RootElementName { get; set; } = "Attributes";

        protected override string ChildElementName { get; set; } = "Attribute";

        [SetUp]
        public void SetUp()
        {
            _attributesXml = "<Attributes><Attribute ID=\"1\">Test 1</Attribute><Attribute ID=\"2\">Test 2</Attribute></Attributes>";
            _xmlDoc = new XmlDocument();
        }

        [Test]
        public void CanRemoveAttribute()
        {
            var xml = RemoveAttribute(_attributesXml, 1);
            _xmlDoc.LoadXml(xml);
            var childNodes = _xmlDoc.SelectNodes($@"//{RootElementName}/{ChildElementName}");

            childNodes?.Count.Should().Be(1);
        }

        [Test]
        public void RemoveAttributeShouldReturnEmptyStringWhenAllChildrenIsRemoved()
        {
            var xml = RemoveAttribute(_attributesXml, 1);
            xml = RemoveAttribute(xml, 2);

            xml.Should().BeEmpty();
        }

        [Test]
        public void RemoveAttributeShouldNotChangeXmlIfAttributeValueIdNotExists()
        {
            var xml = RemoveAttribute(_attributesXml, 3);

            xml.Equals(_attributesXml).Should().BeTrue();
        }
    }
}
