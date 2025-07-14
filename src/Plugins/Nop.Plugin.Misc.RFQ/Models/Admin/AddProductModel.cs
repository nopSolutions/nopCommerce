﻿using System.ComponentModel.DataAnnotations;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.RFQ.Models.Admin;

/// <summary>
/// Represents a product model to add to the quote
/// </summary>
public record AddProductModel : BaseNopModel
{
    #region Ctor

    public AddProductModel()
    {
        ProductAttributes = new List<ProductAttributeModel>();
        GiftCard = new GiftCardModel();
        Warnings = new List<string>();
    }

    #endregion

    #region Properties

    public int ProductId { get; set; }

    public int? QuoteId { get; set; }

    public ProductType ProductType { get; set; }

    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Orders.Products.AddNew.UnitPriceInclTax")]
    public decimal UnitPriceInclTax { get; set; }

    [NopResourceDisplayName("Admin.Orders.Products.AddNew.Quantity")]
    public int Quantity { get; set; }

    //product attributes
    public IList<ProductAttributeModel> ProductAttributes { get; set; }
    //gift card info
    public GiftCardModel GiftCard { get; set; }
    //rental
    public bool IsRental { get; set; }

    public List<string> Warnings { get; set; }

    /// <summary>
    /// A value indicating whether this attribute depends on some other attribute
    /// </summary>
    public bool HasCondition { get; set; }

    #endregion

    #region Nested classes

    public record ProductAttributeModel : BaseNopEntityModel
    {
        public ProductAttributeModel()
        {
            Values = new List<ProductAttributeValueModel>();
        }

        public int ProductAttributeId { get; set; }

        public string Name { get; set; }

        public string TextPrompt { get; set; }

        public bool IsRequired { get; set; }

        public bool HasCondition { get; set; }

        /// <summary>
        /// Allowed file extensions for customer uploaded files
        /// </summary>
        public IList<string> AllowedFileExtensions { get; set; }

        public AttributeControlType AttributeControlType { get; set; }

        public IList<ProductAttributeValueModel> Values { get; set; }
    }

    public record ProductAttributeValueModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public bool IsPreSelected { get; set; }

        public string PriceAdjustment { get; set; }

        public bool CustomerEntersQty { get; set; }

        public int Quantity { get; set; }
    }

    public record GiftCardModel : BaseNopModel
    {
        public bool IsGiftCard { get; set; }

        [NopResourceDisplayName("Admin.GiftCards.Fields.RecipientName")]
        public string RecipientName { get; set; }
        [DataType(DataType.EmailAddress)]
        [NopResourceDisplayName("Admin.GiftCards.Fields.RecipientEmail")]
        public string RecipientEmail { get; set; }
        [NopResourceDisplayName("Admin.GiftCards.Fields.SenderName")]
        public string SenderName { get; set; }
        [DataType(DataType.EmailAddress)]
        [NopResourceDisplayName("Admin.GiftCards.Fields.SenderEmail")]
        public string SenderEmail { get; set; }
        [NopResourceDisplayName("Admin.GiftCards.Fields.Message")]
        public string Message { get; set; }

        public GiftCardType GiftCardType { get; set; }
    }

    #endregion
}