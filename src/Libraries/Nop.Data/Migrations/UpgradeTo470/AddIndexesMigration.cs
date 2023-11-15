using FluentMigrator;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Polls;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.UpgradeTo470
{
    [NopSchemaMigration("2023-11-09 00:00:02", "AddIndexesMigration for 4.70.0")]
    public class AddIndexesMigration : ForwardOnlyMigration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            const string databaseType = "sqlserver";

            IfDatabase(databaseType).Create.Index("IX_ActivityLog_CustomerId").OnTable(nameof(ActivityLog))
                .OnColumn(nameof(ActivityLog.CustomerId)).Ascending()
                .WithOptions().NonClustered();

            IfDatabase(databaseType).Create.Index("IX_CustomerCustomerRoleMapping_CustomerId").OnTable(NameCompatibilityManager.GetTableName(typeof(CustomerCustomerRoleMapping)))
                .OnColumn(NameCompatibilityManager.GetColumnName(typeof(CustomerCustomerRoleMapping), nameof(CustomerCustomerRoleMapping.CustomerId))).Ascending()
                .WithOptions().NonClustered();

            IfDatabase(databaseType).Create.Index("IX_ForumTopic_CustomerId").OnTable(NameCompatibilityManager.GetTableName(typeof(ForumTopic)))
                .OnColumn(nameof(ForumTopic.CustomerId)).Ascending()
                .WithOptions().NonClustered();

            IfDatabase(databaseType).Create.Index("IX_ForumPost_CustomerId").OnTable(NameCompatibilityManager.GetTableName(typeof(ForumPost)))
                .OnColumn(nameof(ForumPost.CustomerId)).Ascending()
                .WithOptions().NonClustered();

            IfDatabase(databaseType).Create.Index("IX_CustomerPassword_CustomerId").OnTable(nameof(CustomerPassword))
                .OnColumn(nameof(CustomerPassword.CustomerId)).Ascending()
                .WithOptions().NonClustered();

            IfDatabase(databaseType).Create.Index("IX_ExternalAuthenticationRecord_CustomerId").OnTable(nameof(ExternalAuthenticationRecord))
                .OnColumn(nameof(ExternalAuthenticationRecord.CustomerId)).Ascending()
                .WithOptions().NonClustered();

            IfDatabase(databaseType).Create.Index("IX_PrivateMessage_CustomerId").OnTable(NameCompatibilityManager.GetTableName(typeof(PrivateMessage)))
                .OnColumn(nameof(PrivateMessage.FromCustomerId)).Ascending()
                .WithOptions().NonClustered();

            IfDatabase(databaseType).Create.Index("IX_PrivateMessage_CustomerId").OnTable(NameCompatibilityManager.GetTableName(typeof(PrivateMessage)))
                .OnColumn(nameof(PrivateMessage.ToCustomerId)).Ascending()
                .WithOptions().NonClustered();

            IfDatabase(databaseType).Create.Index("IX_ForumSubscription_CustomerId").OnTable(NameCompatibilityManager.GetTableName(typeof(ForumSubscription)))
                .OnColumn(nameof(ForumSubscription.CustomerId)).Ascending()
                .WithOptions().NonClustered();

            IfDatabase(databaseType).Create.Index("IX_PollVotingRecord_CustomerId").OnTable(nameof(PollVotingRecord))
                .OnColumn(nameof(PollVotingRecord.CustomerId)).Ascending()
                .WithOptions().NonClustered();

            IfDatabase(databaseType).Create.Index("IX_Order_CustomerId").OnTable(nameof(Order))
                .OnColumn(nameof(Order.CustomerId)).Ascending()
                .WithOptions().NonClustered();

            IfDatabase(databaseType).Create.Index("IX_Log_CustomerId").OnTable(nameof(Log))
                .OnColumn(nameof(Log.CustomerId)).Ascending()
                .WithOptions().NonClustered();

            IfDatabase(databaseType).Create.Index("IX_BlogComment_CustomerId").OnTable(nameof(BlogComment))
                .OnColumn(nameof(BlogComment.CustomerId)).Ascending()
                .WithOptions().NonClustered();

            IfDatabase(databaseType).Create.Index("IX_BackInStockSubscription_CustomerId").OnTable(nameof(BackInStockSubscription))
                .OnColumn(nameof(BackInStockSubscription.CustomerId)).Ascending()
                .WithOptions().NonClustered();

            IfDatabase(databaseType).Create.Index("IX_ReturnRequest_CustomerId").OnTable(nameof(ReturnRequest))
                .OnColumn(nameof(ReturnRequest.CustomerId)).Ascending()
                .WithOptions().NonClustered();

            IfDatabase(databaseType).Create.Index("IX_RewardPointsHistory_CustomerId").OnTable(nameof(RewardPointsHistory))
                .OnColumn(nameof(RewardPointsHistory.CustomerId)).Ascending()
                .WithOptions().NonClustered();

            IfDatabase(databaseType).Create.Index("IX_NewsComment_CustomerId").OnTable(nameof(NewsComment))
                .OnColumn(nameof(NewsComment.CustomerId)).Ascending()
                .WithOptions().NonClustered();

            IfDatabase(databaseType).Create.Index("IX_CustomerAddressMapping_CustomerId").OnTable(NameCompatibilityManager.GetTableName(typeof(CustomerAddressMapping)))
                .OnColumn(NameCompatibilityManager.GetColumnName(typeof(CustomerAddressMapping), nameof(CustomerAddressMapping.CustomerId))).Ascending()
                .WithOptions().NonClustered();

            IfDatabase(databaseType).Create.Index("IX_ShoppingCartItem_CustomerId").OnTable(nameof(ShoppingCartItem))
                .OnColumn(nameof(ShoppingCartItem.CustomerId)).Ascending()
                .WithOptions().NonClustered();

            IfDatabase(databaseType).Create.Index("IX_ProductReview_CustomerId").OnTable(nameof(ProductReview))
                .OnColumn(nameof(ProductReview.CustomerId)).Ascending()
                .WithOptions().NonClustered();
        }
    }
}
