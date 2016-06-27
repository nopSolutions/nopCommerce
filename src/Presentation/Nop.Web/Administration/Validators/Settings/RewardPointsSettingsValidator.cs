using FluentValidation;
using Nop.Admin.Models.Settings;
using Nop.Core.Domain.Orders;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Settings
{
    public partial class RewardPointsSettingsValidator : BaseNopValidator<RewardPointsSettingsModel>
    {
        public RewardPointsSettingsValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.PointsForPurchases_Awarded).NotEqual((int)OrderStatus.Pending).WithMessage(localizationService.GetResource("Admin.Configuration.Settings.RewardPoints.PointsForPurchases_Awarded.Pending"));
            RuleFor(x => x.PointsForPurchases_Canceled).NotEqual((int)OrderStatus.Pending).WithMessage(localizationService.GetResource("Admin.Configuration.Settings.RewardPoints.PointsForPurchases_Canceled.Pending"));
        }
    }
}