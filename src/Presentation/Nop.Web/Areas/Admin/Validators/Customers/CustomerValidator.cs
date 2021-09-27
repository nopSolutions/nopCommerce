using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Customers
{
    public partial class CustomerValidator : BaseNopValidator<CustomerModel>
    {
        public CustomerValidator(CustomerSettings customerSettings,
            ICustomerService customerService,
            ILocalizationService localizationService,
            IMappingEntityAccessor mappingEntityAccessor,
            IStateProvinceService stateProvinceService)
        {
            //ensure that valid email address is entered if Registered role is checked to avoid registered customers with empty email address
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                //.WithMessage("Valid Email is required for customer to be in 'Registered' role")
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Common.WrongEmail"))
                //only for registered users
                .WhenAwait(async x => await IsRegisteredCustomerRoleCheckedAsync(x, customerService));

            //form fields
            if (customerSettings.CountryEnabled && customerSettings.CountryRequired)
            {
                RuleFor(x => x.CountryId)
                    .NotEqual(0)
                    .WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.Country.Required"))
                    //only for registered users
                    .WhenAwait(async x => await IsRegisteredCustomerRoleCheckedAsync(x, customerService));
            }
            if (customerSettings.CountryEnabled &&
                customerSettings.StateProvinceEnabled &&
                customerSettings.StateProvinceRequired)
            {
                RuleFor(x => x.StateProvinceId).MustAwait(async (x, context) =>
                {
                    //does selected country have states?
                    var hasStates = (await stateProvinceService.GetStateProvincesByCountryIdAsync(x.CountryId)).Any();
                    if (hasStates)
                    {
                        //if yes, then ensure that a state is selected
                        if (x.StateProvinceId == 0)
                            return false;
                    }

                    return true;
                }).WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.StateProvince.Required"));
            }
            if (customerSettings.CompanyRequired && customerSettings.CompanyEnabled)
            {
                RuleFor(x => x.Company)
                    .NotEmpty()
                    .WithMessageAwait(localizationService.GetResourceAsync("Admin.Customers.Customers.Fields.Company.Required"))
                    //only for registered users
                    .WhenAwait(async x => await IsRegisteredCustomerRoleCheckedAsync(x, customerService));
            }
            if (customerSettings.StreetAddressRequired && customerSettings.StreetAddressEnabled)
            {
                RuleFor(x => x.StreetAddress)
                    .NotEmpty()
                    .WithMessageAwait(localizationService.GetResourceAsync("Admin.Customers.Customers.Fields.StreetAddress.Required"))
                    //only for registered users
                    .WhenAwait(async x => await IsRegisteredCustomerRoleCheckedAsync(x, customerService));
            }
            if (customerSettings.StreetAddress2Required && customerSettings.StreetAddress2Enabled)
            {
                RuleFor(x => x.StreetAddress2)
                    .NotEmpty()
                    .WithMessageAwait(localizationService.GetResourceAsync("Admin.Customers.Customers.Fields.StreetAddress2.Required"))
                    //only for registered users
                    .WhenAwait(async x => await IsRegisteredCustomerRoleCheckedAsync(x, customerService));
            }
            if (customerSettings.ZipPostalCodeRequired && customerSettings.ZipPostalCodeEnabled)
            {
                RuleFor(x => x.ZipPostalCode)
                    .NotEmpty()
                    .WithMessageAwait(localizationService.GetResourceAsync("Admin.Customers.Customers.Fields.ZipPostalCode.Required"))
                    //only for registered users
                    .WhenAwait(async x => await IsRegisteredCustomerRoleCheckedAsync(x, customerService));
            }
            if (customerSettings.CityRequired && customerSettings.CityEnabled)
            {
                RuleFor(x => x.City)
                    .NotEmpty()
                    .WithMessageAwait(localizationService.GetResourceAsync("Admin.Customers.Customers.Fields.City.Required"))
                    //only for registered users
                    .WhenAwait(async x => await IsRegisteredCustomerRoleCheckedAsync(x, customerService));
            }
            if (customerSettings.CountyRequired && customerSettings.CountyEnabled)
            {
                RuleFor(x => x.County)
                    .NotEmpty()
                    .WithMessageAwait(localizationService.GetResourceAsync("Admin.Customers.Customers.Fields.County.Required"))
                    //only for registered users
                    .WhenAwait(async x => await IsRegisteredCustomerRoleCheckedAsync(x, customerService));
            }
            if (customerSettings.PhoneRequired && customerSettings.PhoneEnabled)
            {
                RuleFor(x => x.Phone)
                    .NotEmpty()
                    .WithMessageAwait(localizationService.GetResourceAsync("Admin.Customers.Customers.Fields.Phone.Required"))
                    //only for registered users
                    .WhenAwait(async x => await IsRegisteredCustomerRoleCheckedAsync(x, customerService));
            }
            if (customerSettings.FaxRequired && customerSettings.FaxEnabled)
            {
                RuleFor(x => x.Fax)
                    .NotEmpty()
                    .WithMessageAwait(localizationService.GetResourceAsync("Admin.Customers.Customers.Fields.Fax.Required"))
                    //only for registered users
                    .WhenAwait(async x => await IsRegisteredCustomerRoleCheckedAsync(x, customerService));
            }

            SetDatabaseValidationRules<Customer>(mappingEntityAccessor);
        }

        private async Task<bool> IsRegisteredCustomerRoleCheckedAsync(CustomerModel model, ICustomerService customerService)
        {
            var allCustomerRoles = await customerService.GetAllCustomerRolesAsync(true);
            var newCustomerRoles = new List<CustomerRole>();
            foreach (var customerRole in allCustomerRoles)
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                    newCustomerRoles.Add(customerRole);

            var isInRegisteredRole = newCustomerRoles.FirstOrDefault(cr => cr.SystemName == NopCustomerDefaults.RegisteredRoleName) != null;
            return isInRegisteredRole;
        }
    }
}