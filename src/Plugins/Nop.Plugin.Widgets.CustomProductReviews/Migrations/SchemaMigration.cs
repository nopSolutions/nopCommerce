using System;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.CustomCustomProductReviews;
using Nop.Plugin.Widgets.CustomProductReviews.Domains;
using Nop.Plugin.Widgets.CustomProductReviews.Mapping.Builders;

namespace Nop.Plugin.Widgets.CustomProductReviews.Migrations
{
    [NopMigration("2022/12/12 15:40:55:1687541", "Nop.Plugin.Widgets.CustomProductReviews base schema", MigrationProcessType.Installation)]
    public class SchemaMigration : FluentMigrator.Migration
    {
        private readonly IMigrationManager _migrationManager;

        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Create.TableFor<Video>();
            Create.TableFor<CustomProductReviewMapping>();
            Create.TableFor<VideoBinary>();

        }     
        public override void Down()
        {
            Delete.Table("CustomProductReviewMapping");
            Delete.Table("Video");
            Delete.Table("VideoBinary");


        }
    }
}
