namespace Nop.Services.ExportImport
{
    public partial class ExportProductAttribute
    {
        /// <summary>
        ///  Gets or sets the cell offset
        /// </summary>
        public static int ProductAttributeCellOffset { get; } = 2;

        /// <summary>
        /// Gets or sets the attribute identifier
        /// </summary>
        public int AttributeId { get; set; }

        /// <summary>
        /// Gets or sets the attribute name
        /// </summary>
        public string AttributeName { get; set; }

        /// <summary>
        /// Gets or sets a value a text prompt
        /// </summary>
        public string AttributeTextPrompt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is required
        /// </summary>
        public bool AttributeIsRequired { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int AttributeDisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the comma separated picture identifiers
        /// </summary>
        public string PictureIds { get; set; }

        /// <summary>
        /// Gets or sets the attribute control type identifier
        /// </summary>
        public int AttributeControlTypeId { get; set; }

        /// <summary>
        /// Gets or sets the product attribute mapping identifier
        /// </summary>
        public int AttributeMappingId { get; set; }

        /// <summary>
        /// Gets or sets the attribute value type identifier
        /// </summary>
        public int AttributeValueTypeId { get; set; }

        /// <summary>
        /// Gets or sets the associated product identifier (used only with AttributeValueType.AssociatedToProduct)
        /// </summary>
        public int AssociatedProductId { get; set; }

        /// <summary>
        /// Gets or sets the identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the picture ID for image square (used with "Image squares" attribute type)
        /// </summary>
        public int ImageSquaresPictureId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the weight adjustment
        /// </summary>
        public decimal WeightAdjustment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer can enter the quantity of associated product (used only with AttributeValueType.AssociatedToProduct)
        /// </summary>
        public bool CustomerEntersQty { get; set; }

        /// <summary>
        /// Gets or sets the quantity of associated product (used only with AttributeValueType.AssociatedToProduct)
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the value is pre-selected
        /// </summary>
        public bool IsPreSelected { get; set; }

        /// <summary>
        /// Gets or sets the color RGB value (used with "Color squares" attribute type)
        /// </summary>
        public string ColorSquaresRgb { get; set; }

        /// <summary>
        /// Gets or sets the price adjustment
        /// </summary>
        public decimal PriceAdjustment { get; set; }

        /// <summary>
        /// Gets or sets the attribute value cost
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether "price adjustment" is specified as percentage
        /// </summary>
        public bool PriceAdjustmentUsePercentage { get; set; }

        /// <summary>
        /// Gets or sets the default value (for textbox and multiline textbox)
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the validation rule for minimum length (for textbox and multiline textbox)
        /// </summary>
        public int? ValidationMinLength { get; set; }

        /// <summary>
        /// Gets or sets the validation rule for maximum length (for textbox and multiline textbox)
        /// </summary>
        public int? ValidationMaxLength { get; set; }

        /// <summary>
        /// Gets or sets the validation rule for file allowed extensions (for file upload)
        /// </summary>
        public string ValidationFileAllowedExtensions { get; set; }

        /// <summary>
        /// Gets or sets the validation rule for file maximum size in kilobytes (for file upload)
        /// </summary>
        public int? ValidationFileMaximumSize { get; set; }
    }
}