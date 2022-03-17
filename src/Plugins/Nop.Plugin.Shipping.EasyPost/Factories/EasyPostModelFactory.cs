using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Shipping.EasyPost.Domain.Batch;
using Nop.Plugin.Shipping.EasyPost.Models.Pickup;
using Nop.Plugin.Shipping.EasyPost.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;

namespace Nop.Plugin.Shipping.EasyPost.Factories
{
    /// <summary>
    /// Represents plugin models factory
    /// </summary>
    public class EasyPostModelFactory
    {
        #region Fields

        private readonly EasyPostService _easyPostService;
        private readonly IAddressModelFactory _addressModelFactory;
        private readonly IAddressService _addressService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPriceFormatter _priceFormatter;

        #endregion

        #region Ctor

        public EasyPostModelFactory(EasyPostService easyPostService,
            IAddressModelFactory addressModelFactory,
            IAddressService addressService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IPriceFormatter priceFormatter)
        {
            _easyPostService = easyPostService;
            _addressModelFactory = addressModelFactory;
            _addressService = addressService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _priceFormatter = priceFormatter;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare pickup model
        /// </summary>
        /// <param name="model">Pickup model</param>
        /// <param name="shipment">Shipment; pass null if it's a pickup for the batch</param>
        /// <param name="batch">Batch; pass null if it's a pickup for the shipment</param>
        /// <param name="addressId">Address id</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the pickup model
        /// </returns>
        public async Task<PickupModel> PreparePickupModelAsync(PickupModel model, Shipment shipment, EasyPostBatch batch, int? addressId)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var pickupId = shipment is not null
                ? await _genericAttributeService.GetAttributeAsync<string>(shipment, EasyPostDefaults.PickupIdAttribute)
                : batch?.PickupId;
            model.PickupId = pickupId;
            model.ShipmentId = shipment?.Id;
            model.BatchId = batch?.Id;

            if (!string.IsNullOrEmpty(pickupId))
            {
                var (pickup, _) = await _easyPostService.GetPickupAsync(pickupId);
                model.Created = pickup is not null;
                model.Status = pickup?.status ?? "not found";
                model.Purchased = string.Equals(pickup?.status, "scheduled", StringComparison.InvariantCultureIgnoreCase);
                if (model.Created && !model.Purchased)
                {
                    var (pickupRates, _) = await _easyPostService.GetPickupRatesAsync(pickup);
                    if (pickupRates?.Any() ?? false)
                    {
                        model.AvailablePickupRates = await pickupRates.OrderBy(rate => rate.Rate).SelectAwait(async rate =>
                        {
                            var rateName = $"{rate.Carrier} {rate.Service}".TrimEnd(' ');
                            var text = $"{await _priceFormatter.FormatShippingPriceAsync(rate.Rate, true)} {rateName}";
                            return new SelectListItem(text, rate.Id);
                        }).ToListAsync();
                    }
                    else
                    {
                        var locale = await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Pickup.Rate.None");
                        model.AvailablePickupRates.Add(new SelectListItem(locale, string.Empty));
                    }
                }
            }

            if (!model.Created)
            {
                var address = await _addressService.GetAddressByIdAsync(addressId ?? 0);
                if (address is not null)
                    model.PickupAddress = address.ToModel(model.PickupAddress);
                await _addressModelFactory.PrepareAddressModelAsync(model.PickupAddress, address);
                model.PickupAddress.CustomAddressAttributes.Clear();

                model.MinDate = DateTime.UtcNow.AddDays(1).Date;
                model.MaxDate = DateTime.UtcNow.AddDays(7).Date;
            }

            return model;
        }

        #endregion
    }
}