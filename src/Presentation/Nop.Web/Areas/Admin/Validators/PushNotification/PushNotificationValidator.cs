using FluentValidation;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.PushNotification
{
    public partial class PushNotificationValidator : BaseNopValidator<PushNotificationModel>
    {
        public PushNotificationValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.MessageTitle).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.PushNotification.PushNotifications.Fields.MessageTitle.Required"));
            RuleFor(x => x.MessageBody).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.PushNotification.PushNotifications.Fields.MessageBody.Required"));

            SetDatabaseValidationRules<CustomerRole>(dataProvider);
        }
    }
}