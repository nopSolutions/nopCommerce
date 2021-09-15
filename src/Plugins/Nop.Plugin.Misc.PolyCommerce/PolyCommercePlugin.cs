using System.Linq;
using Microsoft.AspNetCore.Routing;
using Nop.Data;
using Nop.Services.Plugins;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Misc.PolyCommerce
{
    public class PolyCommercePlugin : BasePlugin, IAdminMenuPlugin
    {
        private readonly IDbContext _dbContext;

        public PolyCommercePlugin(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public override void Install()
        {
            // Create custom PolyCommerceStore table
            _dbContext.ExecuteSqlCommand(@"if not exists(select * from information_schema.tables where TABLE_NAME = 'PolyCommerceStore')
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

            base.Install();
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            // Create custom PolyCommerce menu item

            var menuItem = new SiteMapNode()
            {
                SystemName = "PolyCommercePlugin",
                Title = "PolyCommerce",
                IconClass = "fa-cubes",
                Url = "/Admin/PolyCommerce/Dashboard",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", null } },
            };

            // Add PolyCommerce menu item if it does not already exist.
            if (!rootNode.ChildNodes.Any(x => string.Equals(x.SystemName, "PolyCommercePlugin")))
            {
                rootNode.ChildNodes.Add(menuItem);
            }
                
        }
    }
}
