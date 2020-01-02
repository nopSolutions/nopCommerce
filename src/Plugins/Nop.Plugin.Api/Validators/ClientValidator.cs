using FluentValidation;
using Nop.Services.Localization;

namespace Nop.Plugin.Api.Validators
{
    using IdentityServer4.Models;

    public class ClientValidator : AbstractValidator<Client>
    {
        public ClientValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.ClientName).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.Name"));
            RuleFor(x => x.ClientId).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.ClientId"));
            RuleFor(x => x.ClientSecrets).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.ClientSecret"));
            RuleFor(x => x.RedirectUris).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.CallbackUrl"));
        }
    }
}