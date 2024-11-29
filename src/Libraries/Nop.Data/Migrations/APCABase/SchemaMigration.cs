using FluentMigrator;
using Nop.Core.Domain.Support;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.APCABase;

[NopSchemaMigration("2024/11/23 18:04:00:0000000", "AP CA Base Schema Migration", MigrationProcessType.Update)]
public class SchemaMigration: ForwardOnlyMigration
{
    public override void Up()
    {
        Create.TableFor<SupportRequest>();
        Create.TableFor<SupportMessage>();
    
        // Ensure Status value is within limits of enum
        int statusMax = Enum.GetNames<StatusEnum>().Length;
        
        Execute.Sql($@"
            ALTER TABLE {nameof(SupportRequest)} 
            ADD CONSTRAINT CK_{nameof(SupportRequest)}_{nameof(SupportRequest.Status)}
            CHECK ({nameof(SupportRequest.Status)} >= 0 AND {nameof(SupportRequest.Status)} < {statusMax})");
    }
}