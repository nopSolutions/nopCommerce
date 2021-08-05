using FluentValidation;
using Nop.Core.Domain.Catalog;
using Nop.Data.Mapping;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Catalog
{
    /// <summary>
    /// Represents a validator for <see cref="SpecificationAttributeGroupModel"/>
    /// </summary>
    public partial class SpecificationAttributeGroupValidator : BaseNopValidator<SpecificationAttributeGroupModel>
    {
        public SpecificationAttributeGroupValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Fields.Name.Required"));

            SetDatabaseValidationRules<SpecificationAttributeGroup>(mappingEntityAccessor);
        }
    }
}
