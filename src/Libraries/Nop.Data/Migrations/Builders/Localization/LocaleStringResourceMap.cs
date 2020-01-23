using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Localization;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class LocaleStringResourceBuilder : BaseEntityBuilder<LocaleStringResource>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(LocaleStringResource.ResourceName)).AsString(200).NotNullable()
                .WithColumn(nameof(LocaleStringResource.ResourceValue)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(LocaleStringResource.LanguageId))
                    .AsInt32()
                    .ForeignKey<Language>();

        }

        #endregion
    }
}