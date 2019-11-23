using System;
using System.ComponentModel.DataAnnotations.Schema;
using Nop.Core.Caching;
using Nop.Core.Domain.Localization;

namespace Nop.Services.Localization
{
    /// <summary>
    /// Language (for caching)
    /// </summary>
    [Serializable]
    //Entity Framework will assume that any class that inherits from a POCO class that is mapped to a table on the database requires a Discriminator column
    //That's why we have to add [NotMapped] as an attribute of the derived class.
    [NotMapped]
    public class LanguageForCaching : Language, IEntityForCaching
    {
        public LanguageForCaching()
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="l">Language to copy</param>
        public LanguageForCaching(Language l)
        {
            Id = l.Id;
            Name = l.Name;
            LanguageCulture = l.LanguageCulture;
            UniqueSeoCode = l.UniqueSeoCode;
            FlagImageFileName = l.FlagImageFileName;
            Rtl = l.Rtl;
            LimitedToStores = l.LimitedToStores;
            DefaultCurrencyId = l.DefaultCurrencyId;
            Published = l.Published;
            DisplayOrder = l.DisplayOrder;
        }
    }
}
