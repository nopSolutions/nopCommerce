using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Tax;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.UpgradeTo460
{
    [NopSchemaMigration("2022-07-20 00:00:10", "SchemaMigration for 4.60.0")]
    public class SchemaMigration : ForwardOnlyMigration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            // add column
            var customerTableName = nameof(Customer);

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
                    .AddColumn(firstNameCustomerColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(lastNameCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(lastNameCustomerColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(genderCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(genderCustomerColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(dobCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(dobCustomerColumnName).AsDateTime2().Nullable();
            }
            if (!Schema.Table(customerTableName).Column(companyCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(companyCustomerColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(address1CustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(address1CustomerColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(address2CustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(address2CustomerColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(zipCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(zipCustomerColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(cityCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(cityCustomerColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(countyCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(countyCustomerColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(countryIdCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(countryIdCustomerColumnName).AsInt32().NotNullable().SetExistingRowsTo(0);
            }
            if (!Schema.Table(customerTableName).Column(stateIdCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(stateIdCustomerColumnName).AsInt32().NotNullable().SetExistingRowsTo(0);
            }
            if (!Schema.Table(customerTableName).Column(phoneCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(phoneCustomerColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(faxCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(faxCustomerColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(vatNumberCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(vatNumberCustomerColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(vatNumberStatusIdCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(vatNumberStatusIdCustomerColumnName).AsInt32().NotNullable().SetExistingRowsTo((int)VatNumberStatus.Unknown);
            }
            if (!Schema.Table(customerTableName).Column(timeZoneIdCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(timeZoneIdCustomerColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(attributeXmlCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(attributeXmlCustomerColumnName).AsString(int.MaxValue).Nullable();
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

            //5705
            var discountTableName = nameof(Discount);
            var isActiveDiscountColumnName = nameof(Discount.IsActive);

            if (!Schema.Table(discountTableName).Column(isActiveDiscountColumnName).Exists())
            {
                Alter.Table(discountTableName)
                    .AddColumn(isActiveDiscountColumnName).AsBoolean().NotNullable().SetExistingRowsTo(true);
            }
        }
    }
}
