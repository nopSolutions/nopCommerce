using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Suppliers;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Suppliers
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class SupplierImageBuilder : NopEntityBuilder<SupplierImage>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(SupplierImage.SupplierShopId)).AsInt32().ForeignKey<SupplierShop>()
                .WithColumn(nameof(SupplierImage.Name)).AsString(64).Nullable()
                .WithColumn(nameof(SupplierImage.Url)).AsString(1024).NotNullable()
                .WithColumn(nameof(SupplierImage.JobTitle)).AsString(64).Nullable()
                .WithColumn(nameof(SupplierImage.GradeLevel)).AsString(64).Nullable()
                .WithColumn(nameof(SupplierImage.SupplierSelfGroupId)).AsInt32().Nullable().ForeignKey<SupplierSelfGroup>().OnDelete(Rule.None)
                .WithColumn(nameof(SupplierImage.SysMessage)).AsString(1024).Nullable()
                ;
        }

        #endregion
    }
}
