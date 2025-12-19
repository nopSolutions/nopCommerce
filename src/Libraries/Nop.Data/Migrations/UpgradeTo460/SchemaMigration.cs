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
        this.AddOrAlterColumnFor<Customer>(t => t.FirstName)
            .AsString(1000)
            .Nullable();

        this.AddOrAlterColumnFor<Customer>(t => t.LastName)
            .AsString(1000)
            .Nullable();

        this.AddOrAlterColumnFor<Customer>(t => t.Gender)
            .AsString(1000)
            .Nullable();

        this.AddOrAlterColumnFor<Customer>(t => t.DateOfBirth)
            .AsDateTime2()
            .Nullable();

        this.AddOrAlterColumnFor<Customer>(t => t.Company)
            .AsString(1000)
            .Nullable();

        this.AddOrAlterColumnFor<Customer>(t => t.StreetAddress)
            .AsString(1000)
            .Nullable();

        this.AddOrAlterColumnFor<Customer>(t => t.StreetAddress2)
            .AsString(1000)
            .Nullable();

        this.AddOrAlterColumnFor<Customer>(t => t.ZipPostalCode)
            .AsString(1000)
            .Nullable();

        this.AddOrAlterColumnFor<Customer>(t => t.City)
            .AsString(1000)
            .Nullable();

        this.AddOrAlterColumnFor<Customer>(t => t.County)
            .AsString(1000)
            .Nullable();

        this.AddOrAlterColumnFor<Customer>(t => t.CountryId)
            .AsInt32()
            .NotNullable()
            .SetExistingRowsTo(0);

        this.AddOrAlterColumnFor<Customer>(t => t.StateProvinceId)
            .AsInt32()
            .NotNullable()
            .SetExistingRowsTo(0);

        this.AddOrAlterColumnFor<Customer>(t => t.Phone)
            .AsString(1000)
            .Nullable();

        this.AddOrAlterColumnFor<Customer>(t => t.Fax)
            .AsString(1000)
            .Nullable();

        this.AddOrAlterColumnFor<Customer>(t => t.VatNumber)
            .AsString(1000)
            .Nullable();

        this.AddOrAlterColumnFor<Customer>(t => t.VatNumberStatusId)
            .AsInt32()
            .NotNullable()
            .SetExistingRowsTo((int)VatNumberStatus.Unknown);

        this.AddOrAlterColumnFor<Customer>(t => t.TimeZoneId)
            .AsString(1000)
            .Nullable();

        this.AddOrAlterColumnFor<Customer>(t => t.CustomCustomerAttributesXML)
            .AsString(int.MaxValue)
            .Nullable();

        this.AddOrAlterColumnFor<Customer>(t => t.CurrencyId)
            .AsInt32()
            .ForeignKey<Currency>(onDelete: Rule.SetNull)
            .Nullable();

        this.AddOrAlterColumnFor<Customer>(t => t.LanguageId)
            .AsInt32()
            .ForeignKey<Language>(onDelete: Rule.SetNull)
            .Nullable();

        this.AddOrAlterColumnFor<Customer>(t => t.TaxDisplayTypeId)
            .AsInt32()
            .Nullable();


        // 5705

        this.AddOrAlterColumnFor<Discount>(t => t.IsActive)
            .AsBoolean()
            .NotNullable()
            .SetExistingRowsTo(true);

    }
}