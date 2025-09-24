using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Reminders;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Reminders;

public partial class ReminderValidator : BaseNopValidator<RemindersModel>
{
    public ReminderValidator(ILocalizationService localizationService)
    {
        RuleForEach(x => x.AbandonedCartFollowUps).ChildRules(rulesForFollowUp);
        RuleForEach(x => x.PendingOrdersFollowUps).ChildRules(rulesForFollowUp);
        RuleForEach(x => x.IncompleteRegistrationFollowUps).ChildRules(rulesForFollowUp);
        
        void rulesForFollowUp(InlineValidator<FollowUpModel> followUpValidator)
        {
            followUpValidator.RuleFor(x => x.DelayBeforeSend)
                .NotEmpty()
                .When(x => x.Enabled)
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Promotions.Reminder.FollowUp.DelayBeforeSend.Required"));

            followUpValidator.RuleFor(x => x.DelayBeforeSend)
                .GreaterThan(0)
                .When(x => x.Enabled)
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Promotions.Reminder.FollowUp.DelayBeforeSend.MustBeGreaterThanZero"));
        }
    }
}