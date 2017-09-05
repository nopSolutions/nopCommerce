using System;
using System.ComponentModel.DataAnnotations.Schema;
using Nop.Core.Caching;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Stores;

namespace Nop.Core.Domain.Directory
{
    /// <summary>
    /// Represents a currency
    /// </summary>
    public partial class Currency : BaseEntity, ILocalizedEntity, IStoreMappingSupported
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the currency code
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Gets or sets the rate
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// Gets or sets the display locale
        /// </summary>
        public string DisplayLocale { get; set; }

        /// <summary>
        /// Gets or sets the custom formatting
        /// </summary>
        public string CustomFormatting { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
        /// </summary>
        public bool LimitedToStores { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the rounding type identifier
        /// </summary>
        public int RoundingTypeId { get; set; }

        /// <summary>
        /// Gets or sets the rounding type
        /// </summary>
        public RoundingType RoundingType
        {
            get
            {
                return (RoundingType)RoundingTypeId;
            }
            set
            {
                RoundingTypeId = (int)value;
            }
        }
    }

    [Serializable]
    //Entity Framework will assume that any class that inherits from a POCO class that is mapped to a table on the database requires a Discriminator column
    //That's why we have to add [NotMapped] as an attribute of the derived class.
    [NotMapped]
    public class CurrencyForCaching : Currency, IEntityForCaching
    {
        public CurrencyForCaching()
        {

        }
        public CurrencyForCaching(Currency c)
        {
            Id = c.Id;
            Name = c.Name;
            CurrencyCode = c.CurrencyCode;
            Rate = c.Rate;
            DisplayLocale = c.DisplayLocale;
            CustomFormatting = c.CustomFormatting;
            LimitedToStores = c.LimitedToStores;
            Published = c.Published;
            DisplayOrder = c.DisplayOrder;
            CreatedOnUtc = c.CreatedOnUtc;
            UpdatedOnUtc = c.UpdatedOnUtc;
            RoundingTypeId = c.RoundingTypeId;
        }
    }
}
