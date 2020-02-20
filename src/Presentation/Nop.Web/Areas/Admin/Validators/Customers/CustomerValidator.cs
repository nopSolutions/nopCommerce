using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using System;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.IO;

namespace Nop.Web.Areas.Admin.Validators.Customers
{
    public partial class CustomerValidator : BaseNopValidator<CustomerModel>
    {
        public CustomerValidator(IDataProvider dataProvider, 
            ILocalizationService localizationService,
            IStateProvinceService stateProvinceService,
            ICustomerService customerService,
            CustomerSettings customerSettings)
        {
            //ensure that valid email address is entered if Registered role is checked to avoid registered customers with empty email address
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                //.WithMessage("Valid Email is required for customer to be in 'Registered' role")
                .WithMessage(localizationService.GetResource("Admin.Common.WrongEmail"))
                //only for registered users
                .When(x => IsRegisteredCustomerRoleChecked(x, customerService));

            // For each string input in the CreateOrUpdate.Info Partial view for a customer, validate that
            // the input is safe
            RuleFor(x => x.Username).Must(username => IsSafeInput(username)).When(x => x.UsernamesEnabled);
            RuleFor(x => x.Password).Must(password => IsSafeInput(password));
            RuleFor(x => x.Gender).Must(gender => IsSafeInput(gender)).When(x => x.GenderEnabled);
            RuleFor(x => x.FirstName).Must(firstName => IsSafeInput(firstName));
            RuleFor(x => x.LastName).Must(lastName => IsSafeInput(lastName));
            RuleFor(x => x.Company).Must(company => IsSafeInput(company)).When(x => x.CompanyEnabled);
            RuleFor(x => x.StreetAddress).Must(streetAddress => IsSafeInput(streetAddress))
                .When(x => x.StreetAddressEnabled);
            RuleFor(x => x.StreetAddress2).Must(streetAddress2 => IsSafeInput(streetAddress2)).
                When(x => x.StreetAddress2Enabled);
            RuleFor(x => x.ZipPostalCode).Must(zipcode => IsSafeInput(zipcode)).When(x => x.ZipPostalCodeEnabled);
            RuleFor(x => x.City).Must(city => IsSafeInput(city)).When(x => x.CityEnabled);
            RuleFor(x => x.County).Must(county => IsSafeInput(county)).When(x => x.CountyEnabled);
            RuleFor(x => x.Phone).Must(phone => IsSafeInput(phone)).When(x => x.PhoneEnabled);
            RuleFor(x => x.Fax).Must(fax => IsSafeInput(fax)).When(x => x.FaxEnabled);
            RuleFor(x => x.AdminComment).Must(adminComment => IsSafeInput(adminComment));
            RuleFor(x => x.TimeZoneId).Must(timezone => IsSafeInput(timezone));
            RuleFor(x => x.VatNumber).Must(vatNumber => IsSafeInput(vatNumber)).When(x => x.DisplayVatNumber);

            //form fields
            if (customerSettings.CountryEnabled && customerSettings.CountryRequired)
            {
                RuleFor(x => x.CountryId)
                    .NotEqual(0)
                    .WithMessage(localizationService.GetResource("Account.Fields.Country.Required"))
                    //only for registered users
                    .When(x => IsRegisteredCustomerRoleChecked(x, customerService));
            }
            if (customerSettings.CountryEnabled &&
                customerSettings.StateProvinceEnabled &&
                customerSettings.StateProvinceRequired)
            {
                RuleFor(x => x.StateProvinceId).Must((x, context) =>
                {
                    //does selected country have states?
                    var hasStates = stateProvinceService.GetStateProvincesByCountryId(x.CountryId).Any();
                    if (hasStates)
                    {
                        //if yes, then ensure that a state is selected
                        if (x.StateProvinceId == 0)
                            return false;
                    }

                    return true;
                }).WithMessage(localizationService.GetResource("Account.Fields.StateProvince.Required"));
            }
            if (customerSettings.CompanyRequired && customerSettings.CompanyEnabled)
            {
                RuleFor(x => x.Company)
                    .NotEmpty()
                    .WithMessage(localizationService.GetResource("Admin.Customers.Customers.Fields.Company.Required"))
                    //only for registered users
                    .When(x => IsRegisteredCustomerRoleChecked(x, customerService));
            }
            if (customerSettings.StreetAddressRequired && customerSettings.StreetAddressEnabled)
            {
                RuleFor(x => x.StreetAddress)
                    .NotEmpty()
                    .WithMessage(localizationService.GetResource("Admin.Customers.Customers.Fields.StreetAddress.Required"))
                    //only for registered users
                    .When(x => IsRegisteredCustomerRoleChecked(x, customerService));
            }
            if (customerSettings.StreetAddress2Required && customerSettings.StreetAddress2Enabled)
            {
                RuleFor(x => x.StreetAddress2)
                    .NotEmpty()
                    .WithMessage(localizationService.GetResource("Admin.Customers.Customers.Fields.StreetAddress2.Required"))
                    //only for registered users
                    .When(x => IsRegisteredCustomerRoleChecked(x, customerService));
            }
            if (customerSettings.ZipPostalCodeRequired && customerSettings.ZipPostalCodeEnabled)
            {
                RuleFor(x => x.ZipPostalCode)
                    .NotEmpty()
                    .WithMessage(localizationService.GetResource("Admin.Customers.Customers.Fields.ZipPostalCode.Required"))
                    //only for registered users
                    .When(x => IsRegisteredCustomerRoleChecked(x, customerService));
            }
            if (customerSettings.CityRequired && customerSettings.CityEnabled)
            {
                RuleFor(x => x.City)
                    .NotEmpty()
                    .WithMessage(localizationService.GetResource("Admin.Customers.Customers.Fields.City.Required"))
                    //only for registered users
                    .When(x => IsRegisteredCustomerRoleChecked(x, customerService));
            }
            if (customerSettings.CountyRequired && customerSettings.CountyEnabled)
            {
                RuleFor(x => x.County)
                    .NotEmpty()
                    .WithMessage(localizationService.GetResource("Admin.Customers.Customers.Fields.County.Required"))
                    //only for registered users
                    .When(x => IsRegisteredCustomerRoleChecked(x, customerService));
            }
            if (customerSettings.PhoneRequired && customerSettings.PhoneEnabled)
            {
                RuleFor(x => x.Phone)
                    .NotEmpty()
                    .WithMessage(localizationService.GetResource("Admin.Customers.Customers.Fields.Phone.Required"))
                    //only for registered users
                    .When(x => IsRegisteredCustomerRoleChecked(x, customerService));
            }
            if (customerSettings.FaxRequired && customerSettings.FaxEnabled)
            {
                RuleFor(x => x.Fax)
                    .NotEmpty()
                    .WithMessage(localizationService.GetResource("Admin.Customers.Customers.Fields.Fax.Required"))
                    //only for registered users
                    .When(x => IsRegisteredCustomerRoleChecked(x, customerService));
            }

            SetDatabaseValidationRules<Customer>(dataProvider);
        }

        private bool IsRegisteredCustomerRoleChecked(CustomerModel model, ICustomerService customerService)
        {
            var allCustomerRoles = customerService.GetAllCustomerRoles(true);
            var newCustomerRoles = new List<CustomerRole>();
            foreach (var customerRole in allCustomerRoles)
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                    newCustomerRoles.Add(customerRole);

            var isInRegisteredRole = newCustomerRoles.FirstOrDefault(cr => cr.SystemName == NopCustomerDefaults.RegisteredRoleName) != null;
            return isInRegisteredRole;
        }
        /// <summary>
        /// Given input text, determine if it is safe based on SQL, HTML, and URL checks
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool IsSafeInput(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return true;
            }
            //Check if there are tags to verify against HTML
            var tagRegex = new Regex(@"<[^>]+>");
            if (tagRegex.IsMatch(text))
            {
                return false;
            }

            //Check if the string contains a URL
            bool isUri = Uri.IsWellFormedUriString(text, UriKind.RelativeOrAbsolute);
            if (isUri)
            {
                return false;
            }

            //Check if string is query. If errors is null, that means that conversion to query is successful
            var parser = new TSql100Parser(false);
            using (var reader = new StringReader(text))
            {
                parser.Parse(reader, out var errors);
                if (errors == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}