using FluentValidation;
using Nop.Core.Domain.Vendors;
using Nop.Data.Mapping;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Vendors;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Vendors
{
    public partial class VendorAttributeValidator : BaseNopValidator<VendorAttributeModel>
    {
        public VendorAttributeValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Vendors.VendorAttributes.Fields.Name.Required"));

            SetDatabaseValidationRules<VendorAttribute>(mappingEntityAccessor);
        }
    }
}