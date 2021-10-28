using System;
using System.Threading.Tasks;
using Nop.Core.Domain.Common;
using Nop.Services.Common;
using Nop.Web.Areas.Admin.Models.Common;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the address model factory implementation
    /// </summary>
    public partial class AddressModelFactory : IAddressModelFactory
    {
        #region Fields

        protected IAddressAttributeFormatter AddressAttributeFormatter { get; }
        protected IAddressAttributeModelFactory AddressAttributeModelFactory { get; }
        protected IBaseAdminModelFactory BaseAdminModelFactory { get; }

        #endregion

        #region Ctor

        public AddressModelFactory(IAddressAttributeFormatter addressAttributeFormatter,
            IAddressAttributeModelFactory addressAttributeModelFactory,
            IBaseAdminModelFactory baseAdminModelFactory)
        {
            AddressAttributeFormatter = addressAttributeFormatter;
            AddressAttributeModelFactory = addressAttributeModelFactory;
            BaseAdminModelFactory = baseAdminModelFactory;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare address model
        /// </summary>
        /// <param name="model">Address model</param>
        /// <param name="address">Address</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task PrepareAddressModelAsync(AddressModel model, Address address = null)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //prepare available countries
            await BaseAdminModelFactory.PrepareCountriesAsync(model.AvailableCountries);

            //prepare available states
            await BaseAdminModelFactory.PrepareStatesAndProvincesAsync(model.AvailableStates, model.CountryId);

            //prepare custom address attributes
            await AddressAttributeModelFactory.PrepareCustomAddressAttributesAsync(model.CustomAddressAttributes, address);

            if (address == null)
                return;

            model.FormattedCustomAddressAttributes = await AddressAttributeFormatter.FormatAttributesAsync(address.CustomAttributes);
        }

        #endregion
    }
}