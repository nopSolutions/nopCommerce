using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Data;
using Nop.Services.Configuration;
using Nop.Services.Plugins;
using Nop.Services.Stores;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Misc.PolyCommerce
{
    public class PolyCommercePlugin : BasePlugin, IAdminMenuPlugin
    {
        private readonly ISettingService _settingService;
        private readonly IDbContext _dbContext;

        public PolyCommercePlugin(ISettingService settingService, IStoreService storeService, IStoreContext storeContext, IWorkContext _workContext, IDbContext dbContext)
        {
            var stores = storeService.GetAllStores(false);

            var user = _workContext.CurrentCustomer;

            var active = storeContext.CurrentStore;
            _settingService = settingService;
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
                Url = "/Admin/PolyCommerce/Configure",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", null } },
            };

            rootNode.ChildNodes.Add(menuItem);
        }
    }
}
