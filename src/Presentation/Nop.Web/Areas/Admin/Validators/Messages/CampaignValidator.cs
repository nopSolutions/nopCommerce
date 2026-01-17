using FluentValidation;
using Nop.Core.Domain.Messages;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Messages;

public partial class CampaignValidator : BaseNopValidator<CampaignModel>
{
    public CampaignValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Admin.Promotions.Campaigns.Fields.Name.Required");

        RuleFor(x => x.Subject).NotEmpty().WithMessage("Admin.Promotions.Campaigns.Fields.Subject.Required");

        RuleFor(x => x.Body).NotEmpty().WithMessage("Admin.Promotions.Campaigns.Fields.Body.Required");

        SetDatabaseValidationRules<Campaign>();
    }
}