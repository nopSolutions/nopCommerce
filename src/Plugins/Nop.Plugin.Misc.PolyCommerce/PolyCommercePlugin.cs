using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Nop.Data;
using Nop.Services.Plugins;
using Nop.Web.Framework.Menu;
using System.Data.SqlClient;
using Dapper;

namespace Nop.Plugin.Misc.PolyCommerce
{
    public class PolyCommercePlugin : BasePlugin, IAdminMenuPlugin
    {
        public override async Task InstallAsync()
        {
            var dataSettings = DataSettingsManager.LoadSettings();

            using (var conn = new SqlConnection(dataSettings.ConnectionString))
            {
                // Create custom PolyCommerceStore table

                await conn.ExecuteAsync(@"if not exists(select * from information_schema.tables where TABLE_NAME = 'PolyCommerceStore')
                                                                    create table [dbo].[PolyCommerceStore]
                                                                    (
	                                                                    Id INT NOT NULL IDENTITY(1,1),
	                                                                    StoreId INT NOT NULL,
	                                                                    Token VARCHAR(32) NOT NULL,
	                                                                    IsActive BIT NOT NULL,
	                                                                    CreatedOnUtc DATETIME NOT NULL,
	                                                                    constraint PK_PolyCommerceStore primary key(Id),
	                                                                    constraint FK_PolyCommerceStore_Store foreign key(StoreId) references Store(Id)
                                                                    )");

                await conn.ExecuteAsync(@"if not exists (SELECT * 
                                        FROM sys.indexes 
                                        WHERE name='IX_Shipment_CreatedOnUtc' AND object_id = OBJECT_ID('dbo.Shipment')
                                        )
                                        begin
	                                        create index IX_Shipment_CreatedOnUtc on Shipment(CreatedOnUtc);
                                        end");
            }

            await base.InstallAsync();
        }

        public Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            // Create custom PolyCommerce menu item

            var menuItem = new SiteMapNode()
            {
                SystemName = "PolyCommercePlugin",
                Title = "PolyCommerce",
                IconClass = "fa fa-cube",
                Url = "/Admin/PolyCommerce/Dashboard",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", null } },
            };

            // Add PolyCommerce menu item if it does not already exist.
            if (!rootNode.ChildNodes.Any(x => string.Equals(x.SystemName, "PolyCommercePlugin")))
            {
                rootNode.ChildNodes.Add(menuItem);
            }

            return Task.FromResult(0); 
        }
    }
}
