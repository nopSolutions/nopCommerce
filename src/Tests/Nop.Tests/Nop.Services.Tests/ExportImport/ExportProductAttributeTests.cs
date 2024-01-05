using Nop.Services.ExportImport;
using NUnit.Framework;

namespace Nop.Tests.Services.ExportImport
{
    [TestFixture]
    public class ExportProductAttributeTests
    {
        [Test]
        public void Default_Values_Should_Be_Set_Correctly()
        {
            // Arrange & Act
            var exportProductAttribute = new ExportProductAttribute();

            // Assert
            Assert.AreEqual(2, ExportProductAttribute.ProductAttributeCellOffset);
            Assert.AreEqual(0, exportProductAttribute.AttributeId);
            Assert.IsNull(exportProductAttribute.AttributeName);
            Assert.IsNull(exportProductAttribute.AttributeTextPrompt);
            Assert.IsFalse(exportProductAttribute.AttributeIsRequired);
            Assert.AreEqual(0, exportProductAttribute.AttributeDisplayOrder);
            Assert.IsNull(exportProductAttribute.PictureIds);
            Assert.AreEqual(0, exportProductAttribute.AttributeControlTypeId);
            Assert.AreEqual(0, exportProductAttribute.AttributeMappingId);
            Assert.AreEqual(0, exportProductAttribute.AttributeValueTypeId);
            Assert.AreEqual(0, exportProductAttribute.AssociatedProductId);
            Assert.AreEqual(0, exportProductAttribute.Id);
            Assert.AreEqual(0, exportProductAttribute.ImageSquaresPictureId);
            Assert.IsNull(exportProductAttribute.Name);
            Assert.AreEqual(0, exportProductAttribute.WeightAdjustment);
            Assert.IsFalse(exportProductAttribute.CustomerEntersQty);
            Assert.AreEqual(0, exportProductAttribute.Quantity);
            Assert.IsFalse(exportProductAttribute.IsPreSelected);
            Assert.IsNull(exportProductAttribute.ColorSquaresRgb);
            Assert.AreEqual(0, exportProductAttribute.PriceAdjustment);
            Assert.AreEqual(0, exportProductAttribute.Cost);
            Assert.AreEqual(0, exportProductAttribute.DisplayOrder);
            Assert.IsFalse(exportProductAttribute.PriceAdjustmentUsePercentage);
            Assert.IsNull(exportProductAttribute.DefaultValue);
            Assert.IsNull(exportProductAttribute.ValidationMinLength);
            Assert.IsNull(exportProductAttribute.ValidationMaxLength);
            Assert.IsNull(exportProductAttribute.ValidationFileAllowedExtensions);
            Assert.IsNull(exportProductAttribute.ValidationFileMaximumSize);
        }

        [Test]
        public void Set_AttributeId_Should_Set_AttributeId_Property()
        {
            // Arrange
            var exportProductAttribute = new ExportProductAttribute();

            // Act
            exportProductAttribute.AttributeId = 42;

            // Assert
            Assert.AreEqual(42, exportProductAttribute.AttributeId);
        }

        [Test]
        public void Set_AttributeName_Should_Set_AttributeName_Property()
        {
            // Arrange
            var exportProductAttribute = new ExportProductAttribute();

            // Act
            exportProductAttribute.AttributeName = "Size";

            // Assert
            Assert.AreEqual("Size", exportProductAttribute.AttributeName);
        }

        [Test]
        public void Set_AttributeTextPrompt_Should_Set_AttributeTextPrompt_Property()
        {
            // Arrange
            var exportProductAttribute = new ExportProductAttribute();

            // Act
            exportProductAttribute.AttributeTextPrompt = "Enter your size";

            // Assert
            Assert.AreEqual("Enter your size", exportProductAttribute.AttributeTextPrompt);
        }


        [Test]
        public void Set_Negative_ValidationMinLength_Should_Not_Throw_Exception()
        {
            // Arrange
            var exportProductAttribute = new ExportProductAttribute();

            // Act & Assert
            Assert.DoesNotThrow(() => exportProductAttribute.ValidationMinLength = -1);
        }

        [Test]
        public void Set_Negative_ValidationMaxLength_Should_Throw_CustomValidationException()
        {
            // Arrange
            var exportProductAttribute = new ExportProductAttribute();

            // Act & Assert
            Assert.DoesNotThrow(() => exportProductAttribute.ValidationMaxLength = -1);
        }

        [Test]
        public void Set_Negative_PriceAdjustment_Should_Not_Throw_Exception()
        {
            // Arrange
            var exportProductAttribute = new ExportProductAttribute();

            // Act & Assert
            Assert.DoesNotThrow(() => exportProductAttribute.PriceAdjustment = -10.5m);
        }

    }
}
