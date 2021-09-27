using FluentValidation;
using Nop.Core.Domain.Polls;
using Nop.Data.Mapping;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Polls;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Polls
{
    public partial class PollAnswerValidator : BaseNopValidator<PollAnswerModel>
    {
        public PollAnswerValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
        {
            //if validation without this set rule is applied, in this case nothing will be validated
            //it's used to prevent auto-validation of child models
            RuleSet(NopValidationDefaults.ValidationRuleSet, () =>
            {
                RuleFor(model => model.Name)
                    .NotEmpty()
                    .WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Polls.Answers.Fields.Name.Required"));

                SetDatabaseValidationRules<PollAnswer>(mappingEntityAccessor);
            });
        }
    }
}