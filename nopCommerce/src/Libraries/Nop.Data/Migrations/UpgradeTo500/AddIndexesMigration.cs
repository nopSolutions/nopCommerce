using FluentMigrator;
using Nop.Core.Domain.Common;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopSchemaMigration("2026-04-14 00:00:00", "AddIndexesMigration for 5.00.0")]
public class AddIndexesMigration : ForwardOnlyMigration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        //#7743
        var genericAttributeIndexName = "IX_GenericAttribute_EntityId_KeyGroup_and_Key";
        var genericAttributeTableName = nameof(GenericAttribute);
        if (!Schema.Table(genericAttributeTableName).Index(genericAttributeIndexName).Exists())
        {
            Create.Index(genericAttributeIndexName).OnTable(genericAttributeTableName)
            .OnColumn(nameof(GenericAttribute.EntityId)).Ascending()
            .OnColumn(nameof(GenericAttribute.KeyGroup)).Ascending()
            .OnColumn(nameof(GenericAttribute.Key)).Ascending()
            .WithOptions().NonClustered();
        }
    }
}