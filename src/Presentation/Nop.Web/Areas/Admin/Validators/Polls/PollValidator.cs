using FluentValidation;
using Nop.Core.Domain.Polls;
using Nop.Data.Mapping;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Polls;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Polls
{
    public partial class PollValidator : BaseNopValidator<PollModel>
    {
        public PollValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Polls.Fields.Name.Required"));

            SetDatabaseValidationRules<Poll>(mappingEntityAccessor);
        }
    }
}