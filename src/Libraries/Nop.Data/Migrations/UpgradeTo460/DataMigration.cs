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
            MigrateCustomerAttributes();
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }

        private void MigrateCustomerAttributes()
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
