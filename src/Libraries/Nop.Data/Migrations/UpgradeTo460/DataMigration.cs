using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;

namespace Nop.Data.Migrations.UpgradeTo460
{
    [NopMigration("2022-02-03 00:00:00", "4.60.0", UpdateMigrationType.Data, MigrationProcessType.Update)]
    public class DataMigration : Migration
    {
        private readonly INopDataProvider _dataProvider;

        public DataMigration(INopDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            //#4601 customer attibute values to customer table column values
            var attributeKyes = new string[] { nameof(Customer.FirstName), nameof(Customer.LastName), nameof(Customer.Gender),
                nameof(Customer.Company), nameof(Customer.StreetAddress), nameof(Customer.StreetAddress2), nameof(Customer.ZipPostalCode),
                nameof(Customer.City), nameof(Customer.County), nameof(Customer.Phone), nameof(Customer.Fax), nameof(Customer.VatNumber),
                nameof(Customer.TimeZoneId), nameof(Customer.CustomCustomerAttributesXML), nameof(Customer.CountryId),
                nameof(Customer.StateProvinceId), nameof(Customer.VatNumberStatusId), nameof(Customer.CurrencyId), nameof(Customer.LanguageId),
                nameof(Customer.TaxDisplayTypeId), nameof(Customer.DateOfBirth)};

            var languages = _dataProvider.GetTable<Language>().ToList();
            var currencies = _dataProvider.GetTable<Currency>().ToList();
            var customerRole = _dataProvider.GetTable<CustomerRole>().FirstOrDefault(cr => cr.SystemName == NopCustomerDefaults.RegisteredRoleName);
            var customerRoleId = customerRole?.Id ?? 0;

            var query = from c in _dataProvider.GetTable<Customer>()
                        join crm in _dataProvider.GetTable<CustomerCustomerRoleMapping>() on c.Id equals crm.CustomerId
                        where !c.Deleted && (customerRoleId == 0 || crm.CustomerRoleId == customerRoleId)
                        select c;

            var pageIndex = 0;
            var pageSize = 500;

            while (true)
            {
                var customers = query.ToPagedListAsync(pageIndex++, pageSize).Result;
                if (!customers.Any())
                    break;

                var customerIds = customers.Select(c => c.Id).ToList();
                var genericAttributes = _dataProvider.GetTable<GenericAttribute>()
                    .Where(ga => ga.KeyGroup == nameof(Customer) && customerIds.Contains(ga.EntityId) && attributeKyes.Contains(ga.Key)).ToList();

                if (!genericAttributes.Any())
                    continue;

                foreach (var customer in customers)
                {
                    var customerAttributes = genericAttributes.Where(ga => ga.EntityId == customer.Id).ToList();
                    if (!customerAttributes.Any())
                        continue;

                    if (TryGetAttributeValue(customerAttributes, nameof(Customer.FirstName), 1000, out var attributeValue))
                        customer.FirstName = attributeValue;
                    if (TryGetAttributeValue(customerAttributes, nameof(Customer.LastName), 1000, out attributeValue))
                        customer.LastName = attributeValue;
                    if (TryGetAttributeValue(customerAttributes, nameof(Customer.Gender), 1000, out attributeValue))
                        customer.Gender = attributeValue;
                    if (TryGetAttributeValue(customerAttributes, nameof(Customer.Company), 1000, out attributeValue))
                        customer.Company = attributeValue;
                    if (TryGetAttributeValue(customerAttributes, nameof(Customer.StreetAddress), 1000, out attributeValue))
                        customer.StreetAddress = attributeValue;
                    if (TryGetAttributeValue(customerAttributes, nameof(Customer.StreetAddress2), 1000, out attributeValue))
                        customer.StreetAddress2 = attributeValue;
                    if (TryGetAttributeValue(customerAttributes, nameof(Customer.ZipPostalCode), 1000, out attributeValue))
                        customer.ZipPostalCode = attributeValue;
                    if (TryGetAttributeValue(customerAttributes, nameof(Customer.City), 1000, out attributeValue))
                        customer.City = attributeValue;
                    if (TryGetAttributeValue(customerAttributes, nameof(Customer.County), 1000, out attributeValue))
                        customer.County = attributeValue;
                    if (TryGetAttributeValue(customerAttributes, nameof(Customer.Phone), 1000, out attributeValue))
                        customer.Phone = attributeValue;
                    if (TryGetAttributeValue(customerAttributes, nameof(Customer.Fax), 1000, out attributeValue))
                        customer.Fax = attributeValue;
                    if (TryGetAttributeValue(customerAttributes, nameof(Customer.VatNumber), 1000, out attributeValue))
                        customer.VatNumber = attributeValue;
                    if (TryGetAttributeValue(customerAttributes, nameof(Customer.TimeZoneId), 1000, out attributeValue))
                        customer.TimeZoneId = attributeValue;
                    if (TryGetAttributeValue(customerAttributes, nameof(Customer.CustomCustomerAttributesXML), int.MaxValue, out attributeValue))
                        customer.CustomCustomerAttributesXML = attributeValue;

                    if (int.TryParse(customerAttributes.FirstOrDefault(ga => ga.Key == nameof(Customer.CountryId))?.Value, out var countryId))
                        customer.CountryId = countryId;
                    if (int.TryParse(customerAttributes.FirstOrDefault(ga => ga.Key == nameof(Customer.StateProvinceId))?.Value, out var stateProvinceId))
                        customer.StateProvinceId = stateProvinceId;
                    if (int.TryParse(customerAttributes.FirstOrDefault(ga => ga.Key == nameof(Customer.VatNumberStatusId))?.Value, out var vatNumberStatusId))
                        customer.VatNumberStatusId = vatNumberStatusId;
                    if (int.TryParse(customerAttributes.FirstOrDefault(ga => ga.Key == nameof(Customer.CurrencyId))?.Value, out var currencyId))
                        customer.CurrencyId = currencies.FirstOrDefault(c => c.Id == currencyId)?.Id;
                    if (int.TryParse(customerAttributes.FirstOrDefault(ga => ga.Key == nameof(Customer.LanguageId))?.Value, out var languageId))
                        customer.LanguageId = languages.FirstOrDefault(l => l.Id == languageId)?.Id;
                    if (int.TryParse(customerAttributes.FirstOrDefault(ga => ga.Key == nameof(Customer.TaxDisplayTypeId))?.Value, out var taxDisplayTypeId))
                        customer.TaxDisplayTypeId = taxDisplayTypeId;
                    if (DateTime.TryParseExact(customerAttributes.FirstOrDefault(ga => ga.Key == nameof(Customer.DateOfBirth))?.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOfBirth))
                        customer.DateOfBirth = dateOfBirth;
                }

                _dataProvider.UpdateEntitiesAsync(customers);
                _dataProvider.BulkDeleteEntitiesAsync(genericAttributes);
            }

            //#3777 new activity log types
            var activityLogTypeTable = _dataProvider.GetTable<ActivityLogType>();

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ImportNewsLetterSubscriptions", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "ImportNewsLetterSubscriptions",
                        Enabled = true,
                        Name = "Newsletter subscriptions were imported"
                    }
                );

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ExportCustomers", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "ExportCustomers",
                        Enabled = true,
                        Name = "Customers were exported"
                    }
                );

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ExportCategories", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "ExportCategories",
                        Enabled = true,
                        Name = "Categories were exported"
                    }
                );

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ExportManufacturers", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "ExportManufacturers",
                        Enabled = true,
                        Name = "Manufacturers were exported"
                    }
                );

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ExportProducts", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "ExportProducts",
                        Enabled = true,
                        Name = "Products were exported"
                    }
                );

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ExportOrders", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "ExportOrders",
                        Enabled = true,
                        Name = "Orders were exported"
                    }
                );

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ExportStates", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "ExportStates",
                        Enabled = true,
                        Name = "States were exported"
                    }
                );

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ExportNewsLetterSubscriptions", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "ExportNewsLetterSubscriptions",
                        Enabled = true,
                        Name = "Newsletter subscriptions were exported"
                    }
                );
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }

        private bool TryGetAttributeValue(IList<GenericAttribute> attributes, string key, int maxLength, out string value)
        {
            value = null;
            if (attributes.FirstOrDefault(ga => ga.Key == key) is GenericAttribute genericAttribute)
            {
                value = genericAttribute.Value;
                value = value.Length > maxLength ? value[..maxLength] : value;
                return true;
            }

            return false;
        }
    }
}