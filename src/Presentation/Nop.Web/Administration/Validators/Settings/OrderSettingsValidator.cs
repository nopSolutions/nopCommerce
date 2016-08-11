using FluentValidation;
using Nop.Admin.Models.Settings;
using Nop.Core.Domain.Orders;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Settings
{
    public partial class OrderSettingsValidator : BaseNopValidator<OrderSettingsModel>
    {
        public OrderSettingsValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.GiftCards_Activated_OrderStatusId).NotEqual((int)OrderStatus.Pending)
                .WithMessage(localizationService.GetResource("Admin.Configuration.Settings.Order.GiftCards_Activated.Pending"));
            RuleFor(x => x.GiftCards_Deactivated_OrderStatusId).NotEqual((int)OrderStatus.Pending)
                .WithMessage(localizationService.GetResource("Admin.Configuration.Settings.Order.GiftCards_Deactivated.Pending"));
        }
    }
}