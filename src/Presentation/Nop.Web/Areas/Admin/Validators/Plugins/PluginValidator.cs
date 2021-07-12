using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Plugins;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Plugins
{
    public partial class PluginValidator : BaseNopValidator<PluginModel>
    {
        public PluginValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.FriendlyName).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Plugins.Fields.FriendlyName.Required"));
        }
    }
}