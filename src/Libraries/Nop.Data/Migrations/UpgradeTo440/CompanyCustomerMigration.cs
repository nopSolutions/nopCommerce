//using FluentMigrator;
//using Nop.Core.Domain.Companies;
//using Nop.Data.Mapping;

//namespace Nop.Data.Migrations.UpgradeTo440
//{
//    [NopMigration("2020/03/08 11:26:08:9037938", "Company Customer")]
//    [SkipMigrationOnInstall]
//    public class CompanyCustomerMigration : MigrationBase
//    {
//        #region Fields

//        private readonly IMigrationManager _migrationManager;

//        #endregion

//        #region Ctor

//        public CompanyCustomerMigration(IMigrationManager migrationManager)
//        {
//            _migrationManager = migrationManager;
//        }

//        #endregion

//        #region Methods

//        /// <summary>
//        /// Collect the UP migration expressions
//        /// </summary>
//        public override void Up()
//        {
//            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(CompanyCustomer))).Exists())
//                _migrationManager.BuildTable<CompanyCustomer>(Create);
//        }

//        public override void Down()
//        {
//            //add the downgrade logic if necessary 
//        }

//        #endregion
//    }
//}
