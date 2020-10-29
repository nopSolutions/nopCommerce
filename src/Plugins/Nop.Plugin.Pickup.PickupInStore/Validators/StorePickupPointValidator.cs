using FluentValidation;
using Nop.Plugin.Pickup.PickupInStore.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Pickup.PickupInStore.Validators
{
    public partial class StorePickupPointValidator : BaseNopValidator<StorePickupPointModel>
    {
        public StorePickupPointValidator(ILocalizationService localizationService)
        {
            // Latitude
            RuleFor(model => model.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage(localizationService.GetResourceAsync("Plugins.Pickup.PickupInStore.Fields.Latitude.InvalidRange").Result)
                .When(model => model.Latitude.HasValue);
            RuleFor(model => model.Latitude)
                .Must(latitude => latitude.HasValue)
                .WithMessage(localizationService.GetResourceAsync("Plugins.Pickup.PickupInStore.Fields.Latitude.IsNullWhenLongitudeHasValue").Result)
                .When(model => model.Longitude.HasValue);
            RuleFor(model => model.Latitude)
                .ScalePrecision(8, 18)
                .WithMessage(localizationService.GetResourceAsync("Plugins.Pickup.PickupInStore.Fields.Latitude.InvalidPrecision").Result);

            // Longitude
            RuleFor(model => model.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessage(localizationService.GetResourceAsync("Plugins.Pickup.PickupInStore.Fields.Longitude.InvalidRange").Result)
                .When(model => model.Longitude.HasValue);
            RuleFor(model => model.Longitude)
                .Must(longitude => longitude.HasValue)
                .WithMessage(localizationService.GetResourceAsync("Plugins.Pickup.PickupInStore.Fields.Longitude.IsNullWhenLatitudeHasValue").Result)
                .When(model => model.Latitude.HasValue);
            RuleFor(model => model.Longitude)
                .ScalePrecision(8, 18)
                .WithMessage(localizationService.GetResourceAsync("Plugins.Pickup.PickupInStore.Fields.Longitude.InvalidPrecision").Result);
        }
    }
}
