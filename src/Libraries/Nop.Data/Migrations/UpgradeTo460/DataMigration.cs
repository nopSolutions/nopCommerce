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
    [NopMigration("2022-01-21 00:00:00", "4.50.0", UpdateMigrationType.Data, MigrationProcessType.Update)]
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
            // add column
            var customerTableName = NameCompatibilityManager.GetTableName(typeof(Customer));
            var firstNameCustomerColumnName = "FirstName";
            var lastNameCustomerColumnName = "LastName";
            var genderCustomerColumnName = "Gender";
            var dobCustomerColumnName = "DateOfBirth";
            var companyCustomerColumnName = "Company";
            var address1CustomerColumnName = "StreetAddress1";
            var address2CustomerColumnName = "StreetAddress2";
            var zioCustomerColumnName = "ZipPostalCode";
            var cityCustomerColumnName = "City";
            var countyCustomerColumnName = "County";
            var countryIdCustomerColumnName = "CountryId";
            var stateIdCustomerColumnName = "StateProvinceId";
            var phoneNumberCustomerColumnName = "PhoneNumber";
            var faxCustomerColumnName = "Fax";
            var vatNumberCustomerColumnName = "VatNumber";
            var vatNumberStatusIdCustomerColumnName = "VatNumberStatusId";
            var timeZoneIdCustomerColumnName = "TimeZoneId";
            var attributeXmlCustomerColumnName = "CustomCustomerAttributesXML";
            var currencyIdCustomerColumnName = "CurrencyId";
            var languageIdCustomerColumnName = "LanguageId";
            var taxDisplayTypeIdCustomerColumnName = "TaxDisplayTypeId";
            var euCookieLawAcceptedCustomerColumnName = "EuCookieLawAccepted";

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
                    .AddColumn(dobCustomerColumnName).AsDateTime().Nullable();
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
            if (!Schema.Table(customerTableName).Column(zioCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(zioCustomerColumnName).AsString().Nullable();
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
                    .AddColumn(countryIdCustomerColumnName).AsInt32().WithDefaultValue(0);
            }
            if (!Schema.Table(customerTableName).Column(stateIdCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(stateIdCustomerColumnName).AsInt32().WithDefaultValue(0);
            }
            if (!Schema.Table(customerTableName).Column(phoneNumberCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(phoneNumberCustomerColumnName).AsString().Nullable();
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
            if (!Schema.Table(customerTableName).Column(euCookieLawAcceptedCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(euCookieLawAcceptedCustomerColumnName).AsBoolean().WithDefaultValue(false);
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
