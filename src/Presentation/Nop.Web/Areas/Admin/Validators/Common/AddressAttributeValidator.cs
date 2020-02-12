using FluentValidation;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Core.Domain.Common;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Common
{
    public partial class AddressAttributeValidator : BaseNopValidator<AddressAttributeModel>
    {
        public AddressAttributeValidator(IDataProvider dataProvider, ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Address.AddressAttributes.Fields.Name.Required"));

            SetDatabaseValidationRules<AddressAttribute>(dataProvider);
        }
    }
}