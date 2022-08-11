using FluentValidation;
using Nop.Plugin.Pickup.PickupInStore.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Pickup.PickupInStore.Validators
{
    public class StorePickupPointValidator : BaseNopValidator<StorePickupPointModel>
    {
        public StorePickupPointValidator(ILocalizationService localizationService)
        {
            // Latitude
            RuleFor(model => model.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Pickup.PickupInStore.Fields.Latitude.InvalidRange"))
                .When(model => model.Latitude.HasValue);
            RuleFor(model => model.Latitude)
                .Must(latitude => latitude.HasValue)
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Pickup.PickupInStore.Fields.Latitude.IsNullWhenLongitudeHasValue"))
                .When(model => model.Longitude.HasValue);
            RuleFor(model => model.Latitude)
                .ScalePrecision(8, 18)
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Pickup.PickupInStore.Fields.Latitude.InvalidPrecision"));

            // Longitude
            RuleFor(model => model.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Pickup.PickupInStore.Fields.Longitude.InvalidRange"))
                .When(model => model.Longitude.HasValue);
            RuleFor(model => model.Longitude)
                .Must(longitude => longitude.HasValue)
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Pickup.PickupInStore.Fields.Longitude.IsNullWhenLatitudeHasValue"))
                .When(model => model.Latitude.HasValue);
            RuleFor(model => model.Longitude)
                .ScalePrecision(8, 18)
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Pickup.PickupInStore.Fields.Longitude.InvalidPrecision"));
        }
    }
}
