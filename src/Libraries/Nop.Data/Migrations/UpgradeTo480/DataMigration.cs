using FluentMigrator;

namespace Nop.Data.Migrations.UpgradeTo480;

[NopUpdateMigration("2023-05-21 12:00:00", "4.80", UpdateMigrationType.Data)]
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
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}