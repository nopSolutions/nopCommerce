using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Tax;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.UpgradeTo460;

[NopSchemaMigration("2022-07-20 00:00:10", "SchemaMigration for 4.60.0")]
public class SchemaMigration : ForwardOnlyMigration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        // add column
        if (!Schema.ColumnExist<Customer>(t => t.FirstName))
            Alter.AddColumnFor<Customer>(t => t.FirstName).AsString(1000).Nullable();

        if (!Schema.ColumnExist<Customer>(t => t.LastName))
            Alter.AddColumnFor<Customer>(t => t.LastName).AsString(1000).Nullable();

        if (!Schema.ColumnExist<Customer>(t => t.Gender))
            Alter.AddColumnFor<Customer>(t => t.Gender).AsString(1000).Nullable();

        if (!Schema.ColumnExist<Customer>(t => t.DateOfBirth))
            Alter.AddColumnFor<Customer>(t => t.DateOfBirth).AsDateTime2().Nullable();

        if (!Schema.ColumnExist<Customer>(t => t.Company))
            Alter.AddColumnFor<Customer>(t => t.Company).AsString(1000).Nullable();

        if (!Schema.ColumnExist<Customer>(t => t.StreetAddress))
            Alter.AddColumnFor<Customer>(t => t.StreetAddress).AsString(1000).Nullable();

        if (!Schema.ColumnExist<Customer>(t => t.StreetAddress2))
            Alter.AddColumnFor<Customer>(t => t.StreetAddress2).AsString(1000).Nullable();

        if (!Schema.ColumnExist<Customer>(t => t.ZipPostalCode))
            Alter.AddColumnFor<Customer>(t => t.ZipPostalCode).AsString(1000).Nullable();

        if (!Schema.ColumnExist<Customer>(t => t.City))
            Alter.AddColumnFor<Customer>(t => t.City).AsString(1000).Nullable();

        if (!Schema.ColumnExist<Customer>(t => t.County))
            Alter.AddColumnFor<Customer>(t => t.County).AsString(1000).Nullable();

        if (!Schema.ColumnExist<Customer>(t => t.CountryId))
            Alter.AddColumnFor<Customer>(t => t.CountryId).AsInt32().NotNullable().SetExistingRowsTo(0);

        if (!Schema.ColumnExist<Customer>(t => t.StateProvinceId))
            Alter.AddColumnFor<Customer>(t => t.StateProvinceId).AsInt32().NotNullable().SetExistingRowsTo(0);

        if (!Schema.ColumnExist<Customer>(t => t.Phone))
            Alter.AddColumnFor<Customer>(t => t.Phone).AsString(1000).Nullable();

        if (!Schema.ColumnExist<Customer>(t => t.Fax))
            Alter.AddColumnFor<Customer>(t => t.Fax).AsString(1000).Nullable();

        if (!Schema.ColumnExist<Customer>(t => t.VatNumber))
            Alter.AddColumnFor<Customer>(t => t.VatNumber).AsString(1000).Nullable();

        if (!Schema.ColumnExist<Customer>(t => t.VatNumberStatusId))
            Alter.AddColumnFor<Customer>(t => t.VatNumberStatusId).AsInt32().NotNullable().SetExistingRowsTo((int)VatNumberStatus.Unknown);

        if (!Schema.ColumnExist<Customer>(t => t.TimeZoneId))
            Alter.AddColumnFor<Customer>(t => t.TimeZoneId).AsString(1000).Nullable();

        if (!Schema.ColumnExist<Customer>(t => t.CustomCustomerAttributesXML))
            Alter.AddColumnFor<Customer>(t => t.CustomCustomerAttributesXML).AsString(int.MaxValue).Nullable();

        if (!Schema.ColumnExist<Customer>(t => t.CurrencyId))
            Alter.AddColumnFor<Customer>(t => t.CurrencyId).AsInt32().ForeignKey<Currency>(onDelete: Rule.SetNull).Nullable();

        if (!Schema.ColumnExist<Customer>(t => t.LanguageId))
            Alter.AddColumnFor<Customer>(t => t.LanguageId).AsInt32().ForeignKey<Language>(onDelete: Rule.SetNull).Nullable();

        if (!Schema.ColumnExist<Customer>(t => t.TaxDisplayTypeId))
            Alter.AddColumnFor<Customer>(t => t.TaxDisplayTypeId).AsInt32().Nullable();

        // Discount table
        if (!Schema.ColumnExist<Discount>(t => t.IsActive))
            Alter.AddColumnFor<Discount>(t => t.IsActive).AsBoolean().NotNullable().SetExistingRowsTo(true);
    }
}