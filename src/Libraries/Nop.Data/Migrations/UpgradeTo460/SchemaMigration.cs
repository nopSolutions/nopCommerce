using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping;
using Nop.Data.Extensions;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Tax;
using System.Data;

namespace Nop.Data.Migrations.UpgradeTo460
{
    [NopMigration("2022-02-02 00:00:00", "Customer attribute", MigrationProcessType.Update)]
    public class SchemaMigration : Migration
    {
        private readonly INopDataProvider _dataProvider;

        public SchemaMigration(INopDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            CustomerTable();
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }

        private void CustomerTable()
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
    }
}
