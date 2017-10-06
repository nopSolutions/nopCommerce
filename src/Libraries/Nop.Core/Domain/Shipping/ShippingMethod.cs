using System.Collections.Generic;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Shipping
{
    /// <summary>
    /// Represents a shipping method (used by offline shipping rate computation methods)
    /// </summary>
    public partial class ShippingMethod : BaseEntity, ILocalizedEntity
    {
        private ICollection<ShippingMethodCountry> _restrictedCountries;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the restricted countries
        /// </summary>
        public virtual ICollection<ShippingMethodCountry> RestrictedCountries
        {
            get { return _restrictedCountries ?? (_restrictedCountries = new List<ShippingMethodCountry>()); }
            protected set { _restrictedCountries = value; }
        }

        public void RestrictedCountriesAdd(Country country)
        {
            ShippingMethodCountry smc = new ShippingMethodCountry()
            {
                Country = country,
                CountryId = country.Id,
                ShippingMethod = this,
                ShippingMethodId = this.Id
            };
            RestrictedCountries.Add(smc);
        }

        public void RestrictedCountriesRemove(Country country)
        {
            var item = ((List<ShippingMethodCountry>) RestrictedCountries).Find(c =>
                c.CountryId == country.Id && c.ShippingMethodId == this.Id);
            RestrictedCountries.Remove(item);
        }
    }
}