using System;
using System.ComponentModel.DataAnnotations.Schema;
using Nop.Core.Caching;
using Nop.Core.Domain.Stores;

namespace Nop.Services.Stores
{
    /// <summary>
    /// Store (for caching)
    /// </summary>
    [Serializable]
    //Entity Framework will assume that any class that inherits from a POCO class that is mapped to a table on the database requires a Discriminator column
    //That's why we have to add [NotMapped] as an attribute of the derived class.
    [NotMapped]
    public class StoreForCaching : Store, IEntityForCaching
    {
        public StoreForCaching()
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="s">Store to copy</param>
        public StoreForCaching(Store s)
        {
            Id = s.Id;
            Name = s.Name;
            Url = s.Url;
            SslEnabled = s.SslEnabled;
            Hosts = s.Hosts;
            DefaultLanguageId = s.DefaultLanguageId;
            DisplayOrder = s.DisplayOrder;
            CompanyName = s.CompanyName;
            CompanyAddress = s.CompanyAddress;
            CompanyPhoneNumber = s.CompanyPhoneNumber;
            CompanyVat = s.CompanyVat;
        }
    }
}