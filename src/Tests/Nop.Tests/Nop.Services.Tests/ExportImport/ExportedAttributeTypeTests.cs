using Nop.Services.ExportImport;
using NUnit.Framework;

namespace Nop.Tests.Services.ExportImport
{
    [TestFixture]
    public class ExportedAttributeTypeTests
    {
        [Test]
        public void Enum_Values_Should_Have_Correct_Numeric_Values()
        {
            // Assert
            Assert.AreEqual(1, (int)ExportedAttributeType.NotSpecified);
            Assert.AreEqual(10, (int)ExportedAttributeType.ProductAttribute);
            Assert.AreEqual(20, (int)ExportedAttributeType.SpecificationAttribute);
        }

        [Test]
        public void Enum_Values_Should_Not_Overlap()
        {
            // Assert
            Assert.AreNotEqual((int)ExportedAttributeType.NotSpecified, (int)ExportedAttributeType.ProductAttribute);
            Assert.AreNotEqual((int)ExportedAttributeType.NotSpecified, (int)ExportedAttributeType.SpecificationAttribute);
            Assert.AreNotEqual((int)ExportedAttributeType.ProductAttribute, (int)ExportedAttributeType.SpecificationAttribute);
        }

        [Test]
        public void Enum_Values_Should_Not_Have_Negative_Values()
        {
            // Assert
            Assert.IsFalse((int)ExportedAttributeType.NotSpecified < 0);
            Assert.IsFalse((int)ExportedAttributeType.ProductAttribute < 0);
            Assert.IsFalse((int)ExportedAttributeType.SpecificationAttribute < 0);
        }

        [Test]
        public void Enum_Values_Should_Not_Be_Duplicate()
        {
            // Arrange
            var values = (ExportedAttributeType[])Enum.GetValues(typeof(ExportedAttributeType));

            // Assert
            Assert.AreEqual(values.Length, values.Distinct().Count());
        }
    }
}
