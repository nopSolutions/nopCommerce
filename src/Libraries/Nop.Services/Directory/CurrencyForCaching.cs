using System;
using System.ComponentModel.DataAnnotations.Schema;
using Nop.Core.Caching;
using Nop.Core.Domain.Directory;

namespace Nop.Services.Directory
{
    /// <summary>
    /// Current (for caching)
    /// </summary>
    [Serializable]
    //Entity Framework will assume that any class that inherits from a POCO class that is mapped to a table on the database requires a Discriminator column
    //That's why we have to add [NotMapped] as an attribute of the derived class.
    [NotMapped]
    public class CurrencyForCaching : Currency, IEntityForCaching
    {
        public CurrencyForCaching()
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="c">Currency to copy</param>
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