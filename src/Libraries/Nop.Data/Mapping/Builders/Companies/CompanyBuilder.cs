using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Companies;

namespace Nop.Data.Mapping.Builders.Companies
{
    public partial class CompanyBuilder : NopEntityBuilder<Company>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Company.Name)).AsString(400).NotNullable();
        }

        #endregion
    }
}