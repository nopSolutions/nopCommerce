using FluentMigrator;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Polls;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.UpgradeTo470
{
    [NopSchemaMigration("2023-03-07 00:00:02", "SchemaMigration for 4.70.0")]
    public class SchemaMigration : ForwardOnlyMigration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            //#6167
            var messageTemplateTableName = nameof(MessageTemplate);
            var allowDirectReplyColumnName = nameof(MessageTemplate.AllowDirectReply);

            //add column
            if (!Schema.Table(messageTemplateTableName).Column(allowDirectReplyColumnName).Exists())
                Alter.Table(messageTemplateTableName)
                    .AddColumn(allowDirectReplyColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);

            //1934
            if (!Schema.Table(nameof(ProductAttributeCombinationPicture)).Exists()) 
                Create.TableFor<ProductAttributeCombinationPicture>();

            if (!Schema.Table(nameof(ProductAttributeValuePicture)).Exists()) 
                Create.TableFor<ProductAttributeValuePicture>();

            var productTableName = nameof(Product);
            var displayAttributeCombinationImagesOnlyColumnName = nameof(Product.DisplayAttributeCombinationImagesOnly);

            if (!Schema.Table(productTableName).Column(displayAttributeCombinationImagesOnlyColumnName).Exists())
                Alter.Table(productTableName)
                    .AddColumn(displayAttributeCombinationImagesOnlyColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);

            //#6710
            var productAttributeCombinationTableName = nameof(ProductAttributeCombination);
            var pac = Schema.Table(productAttributeCombinationTableName);
            var columnName = "PictureId";
            var description = "The field is not used since 4.70 and is left only for the update process use the ProductAttributeCombinationPicture instead";

            if (pac.Column(columnName).Exists())
                Alter.Table(productAttributeCombinationTableName)
                    .AlterColumn(columnName).AsInt32().Nullable().WithColumnDescription(description);
            else
                Alter.Table(productAttributeCombinationTableName)
                    .AddColumn(columnName).AsInt32().Nullable().SetExistingRowsTo(null).WithColumnDescription(description);

            var productAttributeValueTableName = nameof(ProductAttributeValue);
            var pav = Schema.Table(productAttributeValueTableName);

            if (pav.Column(columnName).Exists())
                Alter.Table(productAttributeValueTableName)
                    .AlterColumn(columnName).AsInt32().Nullable().WithColumnDescription(description);
            else
                Alter.Table(productAttributeValueTableName)
                    .AddColumn(columnName).AsInt32().Nullable().SetExistingRowsTo(null).WithColumnDescription(description);

            // 6771
            Alter.Table(nameof(Customer)).AlterColumn(nameof(Customer.LastIpAddress)).AsString(100).Nullable();
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(ForumPost))).AlterColumn(nameof(ForumPost.IPAddress)).AsString(100).Nullable();
            Alter.Table(nameof(ActivityLog)).AlterColumn(nameof(ActivityLog.IpAddress)).AsString(100).Nullable();
            Alter.Table(nameof(Log)).AlterColumn(nameof(Log.IpAddress)).AsString(100).Nullable();
            Alter.Table(nameof(Order)).AlterColumn(nameof(Order.CustomerIp)).AsString(100).Nullable();

			// #6906
			Create.Index("IX_ActivityLog_CustomerId").OnTable(nameof(ActivityLog))
				.OnColumn(nameof(ActivityLog.CustomerId)).Ascending()
				.WithOptions().NonClustered();

			Create.Index("IX_CustomerCustomerRoleMapping_CustomerId").OnTable(nameof(CustomerCustomerRoleMapping))
				.OnColumn(nameof(CustomerCustomerRoleMapping.CustomerId)).Ascending()
				.WithOptions().NonClustered();

			Create.Index("IX_ForumTopic_CustomerId").OnTable(nameof(ForumTopic))
				.OnColumn(nameof(ForumTopic.CustomerId)).Ascending()
				.WithOptions().NonClustered();

			Create.Index("IX_ForumPost_CustomerId").OnTable(nameof(ForumPost))
				.OnColumn(nameof(ForumPost.CustomerId)).Ascending()
				.WithOptions().NonClustered();

			Create.Index("IX_CustomerPassword_CustomerId").OnTable(nameof(CustomerPassword))
				.OnColumn(nameof(CustomerPassword.CustomerId)).Ascending()
				.WithOptions().NonClustered();

			Create.Index("IX_ExternalAuthenticationRecord_CustomerId").OnTable(nameof(ExternalAuthenticationRecord))
				.OnColumn(nameof(ExternalAuthenticationRecord.CustomerId)).Ascending()
				.WithOptions().NonClustered();

			Create.Index("IX_PrivateMessage_From_CustomerId").OnTable(nameof(PrivateMessage))
				.OnColumn(nameof(PrivateMessage.FromCustomerId)).Ascending()
				.WithOptions().NonClustered();

			Create.Index("IX_PrivateMessage_To_CustomerId").OnTable(nameof(PrivateMessage))
				.OnColumn(nameof(PrivateMessage.ToCustomerId)).Ascending()
				.WithOptions().NonClustered();

			Create.Index("IX_ForumSubscription_CustomerId").OnTable(nameof(ForumSubscription))
				.OnColumn(nameof(ForumSubscription.CustomerId)).Ascending()
				.WithOptions().NonClustered();

			Create.Index("IX_PollVotingRecord_CustomerId").OnTable(nameof(PollVotingRecord))
				.OnColumn(nameof(PollVotingRecord.CustomerId)).Ascending()
				.WithOptions().NonClustered();

			Create.Index("IX_Order_CustomerId").OnTable(nameof(Order))
				.OnColumn(nameof(Order.CustomerId)).Ascending()
				.WithOptions().NonClustered();

			Create.Index("IX_Log_CustomerId").OnTable(nameof(Log))
				.OnColumn(nameof(Log.CustomerId)).Ascending()
				.WithOptions().NonClustered();

			Create.Index("IX_BlogComment_CustomerId").OnTable(nameof(BlogComment))
				.OnColumn(nameof(BlogComment.CustomerId)).Ascending()
				.WithOptions().NonClustered();

			Create.Index("IX_BackInStockSubscription_CustomerId").OnTable(nameof(BackInStockSubscription))
				.OnColumn(nameof(BackInStockSubscription.CustomerId)).Ascending()
				.WithOptions().NonClustered();

			Create.Index("IX_ReturnRequest_CustomerId").OnTable(nameof(ReturnRequest))
				.OnColumn(nameof(ReturnRequest.CustomerId)).Ascending()
				.WithOptions().NonClustered();

			Create.Index("IX_RewardPointsHistory_CustomerId").OnTable(nameof(RewardPointsHistory))
				.OnColumn(nameof(RewardPointsHistory.CustomerId)).Ascending()
				.WithOptions().NonClustered();

			Create.Index("IX_NewsComment_CustomerId").OnTable(nameof(NewsComment))
				.OnColumn(nameof(NewsComment.CustomerId)).Ascending()
				.WithOptions().NonClustered();


			Create.Index("IX_CustomerAddressMapping_CustomerId").OnTable(nameof(CustomerAddressMapping))
				.OnColumn(nameof(CustomerAddressMapping.CustomerId)).Ascending()
				.WithOptions().NonClustered();

			Create.Index("IX_ShoppingCartItem_CustomerId").OnTable(nameof(ShoppingCartItem))
				.OnColumn(nameof(ShoppingCartItem.CustomerId)).Ascending()
				.WithOptions().NonClustered();

			Create.Index("IX_ProductReview_CustomerId").OnTable(nameof(ProductReview))
				.OnColumn(nameof(ProductReview.CustomerId)).Ascending()
				.WithOptions().NonClustered();
		}
	}
}
