using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.CustomProductReviews;
using Nop.Services.Configuration;

namespace Nop.Plugin.Widgets.CustomProductReviews.Data
{ 
    [NopMigration("2021-09-16 00:00:00", "ProductReviews 1.0. Add setting", MigrationProcessType.Update)]
    public class ProductReviewsMigration : MigrationBase
    {
        #region Fields

        private readonly CustomProductReviewsSettings _productReviewSettings;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public ProductReviewsMigration(CustomProductReviewsSettings ecbExchangeRateSettings,
            ISettingService settingService)
        {
            _productReviewSettings = ecbExchangeRateSettings;
            _settingService = settingService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;
            
            //settings
            if (!_settingService.SettingExistsAsync(_productReviewSettings, settings => settings.data).Result)
                _productReviewSettings.data = "test";

            _settingService.SaveSettingAsync(_productReviewSettings).Wait();
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            //nothing
        }

        #endregion
    }
}