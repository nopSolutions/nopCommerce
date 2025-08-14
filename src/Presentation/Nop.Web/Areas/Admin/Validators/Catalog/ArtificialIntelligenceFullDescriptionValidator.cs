using FluentValidation;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Catalog;

public partial class ArtificialIntelligenceFullDescriptionValidator : BaseNopValidator<ArtificialIntelligenceFullDescriptionModel>
{
    public ArtificialIntelligenceFullDescriptionValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.ProductName)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Products.AiFullDescription.ProductName.Required"));

        RuleFor(x => x.Keywords)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Products.AiFullDescription.Keywords.Required"));

        RuleFor(x => x.CustomToneOfVoice)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Products.AiFullDescription.CustomToneOfVoice.Required"))
            .When(x => x.ToneOfVoiceId == (int)ToneOfVoiceType.Custom);
    }
}