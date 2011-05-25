using FluentValidation;
using Nop.Admin.Models;
using Nop.Services.Localization;

namespace Nop.Admin.Validators
{
    public class MessageTemplateValidator : AbstractValidator<MessageTemplateModel>
    {
        public MessageTemplateValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Subject).NotNull().WithMessage(localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Fields.Subject.Required"));
            RuleFor(x => x.Body).NotNull().WithMessage(localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Fields.Body.Required"));
        }
    }
}