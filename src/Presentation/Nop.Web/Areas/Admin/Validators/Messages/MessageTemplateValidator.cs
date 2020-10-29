using FluentValidation;
using Nop.Core.Domain.Messages;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Messages
{
    public partial class MessageTemplateValidator : BaseNopValidator<MessageTemplateModel>
    {
        public MessageTemplateValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Subject).NotEmpty().WithMessage(localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Fields.Subject.Required").Result);
            RuleFor(x => x.Body).NotEmpty().WithMessage(localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Fields.Body.Required").Result);

            SetDatabaseValidationRules<MessageTemplate>(dataProvider);
        }
    }
}