using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping;
using Nop.Data.Extensions;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Tax;
using System.Data;
using System;
using System.Linq;
using Nop.Core.Domain.Common;

namespace Nop.Data.Migrations.UpgradeTo460
{
    [NopMigration("2022-02-02 00:00:00", "Customer attribute", MigrationProcessType.Update)]
    public class CustomerAttributeMigration : Migration
    {
        private readonly INopDataProvider _dataProvider;

        public CustomerAttributeMigration(INopDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            CustomerTableAlter();
            MigrateAttributeValues();
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }

        private void CustomerTableAlter()
        {
            // add column
            var customerTableName = NameCompatibilityManager.GetTableName(typeof(Customer));

            var firstNameCustomerColumnName = nameof(Customer.FirstName);
            var lastNameCustomerColumnName = nameof(Customer.LastName);
            var genderCustomerColumnName = nameof(Customer.Gender);
            var dobCustomerColumnName = nameof(Customer.DateOfBirth);
            var companyCustomerColumnName = nameof(Customer.Company);
            var address1CustomerColumnName = nameof(Customer.StreetAddress);
            var address2CustomerColumnName = nameof(Customer.StreetAddress2);
            var zipCustomerColumnName = nameof(Customer.ZipPostalCode);
            var cityCustomerColumnName = nameof(Customer.City);
            var countyCustomerColumnName = nameof(Customer.County);
            var countryIdCustomerColumnName = nameof(Customer.CountryId);
            var stateIdCustomerColumnName = nameof(Customer.StateProvinceId);
            var phoneCustomerColumnName = nameof(Customer.Phone);
            var faxCustomerColumnName = nameof(Customer.Fax);
            var vatNumberCustomerColumnName = nameof(Customer.VatNumber);
            var vatNumberStatusIdCustomerColumnName = nameof(Customer.VatNumberStatusId);
            var timeZoneIdCustomerColumnName = nameof(Customer.TimeZoneId);
            var attributeXmlCustomerColumnName = nameof(Customer.CustomCustomerAttributesXML);
            var currencyIdCustomerColumnName = nameof(Customer.CurrencyId);
            var languageIdCustomerColumnName = nameof(Customer.LanguageId);
            var taxDisplayTypeIdCustomerColumnName = nameof(Customer.TaxDisplayTypeId);

            if (!Schema.Table(customerTableName).Column(firstNameCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(firstNameCustomerColumnName).AsString().Nullable();
            }
            if (!Schema.Table(customerTableName).Column(lastNameCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(lastNameCustomerColumnName).AsString().Nullable();
            }
            if (!Schema.Table(customerTableName).Column(genderCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(genderCustomerColumnName).AsString().Nullable();
            }
            if (!Schema.Table(customerTableName).Column(dobCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(dobCustomerColumnName).AsDateTime2().Nullable();
            }
            if (!Schema.Table(customerTableName).Column(companyCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(companyCustomerColumnName).AsString().Nullable();
            }
            if (!Schema.Table(customerTableName).Column(address1CustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(address1CustomerColumnName).AsString().Nullable();
            }
            if (!Schema.Table(customerTableName).Column(address2CustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(address2CustomerColumnName).AsString().Nullable();
            }
            if (!Schema.Table(customerTableName).Column(zipCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(zipCustomerColumnName).AsString().Nullable();
            }
            if (!Schema.Table(customerTableName).Column(cityCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(cityCustomerColumnName).AsString().Nullable();
            }
            if (!Schema.Table(customerTableName).Column(countyCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(countyCustomerColumnName).AsString().Nullable();
            }
            if (!Schema.Table(customerTableName).Column(countryIdCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(countryIdCustomerColumnName).AsInt32().NotNullable().SetExistingRowsTo(0);
            }
            if (!Schema.Table(customerTableName).Column(stateIdCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(stateIdCustomerColumnName).AsInt32().WithDefaultValue(0);
            }
            if (!Schema.Table(customerTableName).Column(phoneCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(phoneCustomerColumnName).AsString().Nullable();
            }
            if (!Schema.Table(customerTableName).Column(faxCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(faxCustomerColumnName).AsString().Nullable();
            }
            if (!Schema.Table(customerTableName).Column(vatNumberCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(vatNumberCustomerColumnName).AsString().Nullable();
            }
            if (!Schema.Table(customerTableName).Column(vatNumberStatusIdCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(vatNumberStatusIdCustomerColumnName).AsInt32().WithDefaultValue((int)VatNumberStatus.Unknown);
            }
            if (!Schema.Table(customerTableName).Column(timeZoneIdCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(timeZoneIdCustomerColumnName).AsString().Nullable();
            }
            if (!Schema.Table(customerTableName).Column(attributeXmlCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(attributeXmlCustomerColumnName).AsString().Nullable();
            }
            if (!Schema.Table(customerTableName).Column(currencyIdCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(currencyIdCustomerColumnName).AsInt32().ForeignKey<Currency>(onDelete: Rule.SetNull).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(languageIdCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(languageIdCustomerColumnName).AsInt32().ForeignKey<Language>(onDelete: Rule.SetNull).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(taxDisplayTypeIdCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(taxDisplayTypeIdCustomerColumnName).AsInt32().Nullable();
            }
        }

        private void MigrateAttributeValues()
        {
            // customer attibute values to customer table column values
            var customerRole = _dataProvider.GetTable<CustomerRole>().FirstOrDefault(cr => cr.SystemName == NopCustomerDefaults.RegisteredRoleName);
            var customerRoleId = customerRole?.Id ?? 0;

            var query = from c in _dataProvider.GetTable<Customer>()
                        join crm in _dataProvider.GetTable<CustomerCustomerRoleMapping>() on c.Id equals crm.CustomerId
                        where !c.Deleted && (customerRoleId == 0 || crm.CustomerRoleId == customerRoleId)
                        select new { Customer = c, CustomerRoleMapping = crm };

            var pageIndex = 0;
            var pageSize = 500;

            while (true)
            {
                var customers = query.Select(x => x.Customer).Skip(pageIndex * pageSize).Take(pageSize).ToList();
                if (!customers.Any())
                    break;

                for (var i = 0; i < customers.Count; i++)
                {
                    var customer = customers[i];
                    var customerAttributes = _dataProvider.GetTable<GenericAttribute>()
                        .Where(ga => ga.KeyGroup == nameof(Customer) && ga.EntityId == customer.Id).ToList();

                    if (!customerAttributes.Any())
                        continue;

                    customer.FirstName = customerAttributes.FirstOrDefault(ga => ga.Key == "FirstName")?.Value;
                    customer.LastName = customerAttributes.FirstOrDefault(ga => ga.Key == "LastName")?.Value;
                    customer.Gender = customerAttributes.FirstOrDefault(ga => ga.Key == "Gender")?.Value;
                    customer.Company = customerAttributes.FirstOrDefault(ga => ga.Key == "Company")?.Value;
                    customer.StreetAddress = customerAttributes.FirstOrDefault(ga => ga.Key == "StreetAddress")?.Value;
                    customer.StreetAddress2 = customerAttributes.FirstOrDefault(ga => ga.Key == "StreetAddress2")?.Value;
                    customer.ZipPostalCode = customerAttributes.FirstOrDefault(ga => ga.Key == "ZipPostalCode")?.Value;
                    customer.City = customerAttributes.FirstOrDefault(ga => ga.Key == "City")?.Value;
                    customer.County = customerAttributes.FirstOrDefault(ga => ga.Key == "County")?.Value;
                    customer.Phone = customerAttributes.FirstOrDefault(ga => ga.Key == "Phone")?.Value;
                    customer.Fax = customerAttributes.FirstOrDefault(ga => ga.Key == "Fax")?.Value;
                    customer.VatNumber = customerAttributes.FirstOrDefault(ga => ga.Key == "VatNumber")?.Value;
                    customer.TimeZoneId = customerAttributes.FirstOrDefault(ga => ga.Key == "TimeZoneId")?.Value;
                    customer.CustomCustomerAttributesXML = customerAttributes.FirstOrDefault(ga => ga.Key == "CustomCustomerAttributesXML")?.Value;

                    if (int.TryParse(customerAttributes.FirstOrDefault(ga => ga.Key == "CountryId")?.Value, out var countryId))
                        customer.CountryId = countryId;
                    if (int.TryParse(customerAttributes.FirstOrDefault(ga => ga.Key == "StateProvinceId")?.Value, out var stateProvinceId))
                        customer.StateProvinceId = stateProvinceId;
                    if (int.TryParse(customerAttributes.FirstOrDefault(ga => ga.Key == "VatNumberStatusId")?.Value, out var vatNumberStatusId))
                        customer.VatNumberStatusId = vatNumberStatusId;
                    if (int.TryParse(customerAttributes.FirstOrDefault(ga => ga.Key == "CurrencyId")?.Value, out var currencyId))
                        customer.CurrencyId = currencyId;
                    if (int.TryParse(customerAttributes.FirstOrDefault(ga => ga.Key == "LanguageId")?.Value, out var languageId))
                        customer.LanguageId = languageId;
                    if (int.TryParse(customerAttributes.FirstOrDefault(ga => ga.Key == "TaxDisplayTypeId")?.Value, out var taxDisplayTypeId))
                        customer.TaxDisplayTypeId = taxDisplayTypeId;
                    if (DateTime.TryParse(customerAttributes.FirstOrDefault(ga => ga.Key == "DateOfBirth")?.Value, out var dateOfBirth))
                        customer.DateOfBirth = dateOfBirth;

                    _dataProvider.UpdateEntityAsync(customer);
                }

                pageIndex++;
            }
        }
    }
}
