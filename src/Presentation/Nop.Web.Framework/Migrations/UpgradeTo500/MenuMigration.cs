using FluentMigrator;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Menus;
using Nop.Core.Http;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Helpers;
using Nop.Web.Framework.Extensions;
using M = Nop.Core.Domain.Menus;

namespace Nop.Web.Framework.Migrations.UpgradeTo500;

[NopMigration("2026-08-28 12:00:02", "Menus. Adding menus", MigrationProcessType.Update)]
public class MenuMigration : Migration
{
    #region Fields

    private readonly IRepository<M.Menu> _menuRepository;
    private readonly IRepository<MenuItem> _menuItemRepository;

    #endregion

    #region Ctor

    public MenuMigration(IRepository<M.Menu> menuRepository, IRepository<MenuItem> menuItemRepository)
    {
        _menuRepository = menuRepository;
        _menuItemRepository = menuItemRepository;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        var syncCodeHelper = EngineContext.Current.Resolve<ISyncCodeHelper>();

        var footerMyAccount = _menuRepository.Table.FirstOrDefault(m => m.MenuTypeId == (int)MenuType.Footer && m.Name.Equals("My Account"));

        if (footerMyAccount == null)
            return;

        var menuItemExists = _menuItemRepository.Table.Any(m => m.MenuId == footerMyAccount.Id && m.RouteName.Equals(NopRouteNames.General.APPLY_AFFILIATE_ACCOUNT));

        if (menuItemExists)
            return;

        syncCodeHelper.InsertEntity(new MenuItem
        {
            MenuId = footerMyAccount.Id,
            MenuItemType = MenuItemType.StandardPage,
            RouteName = NopRouteNames.General.APPLY_AFFILIATE_ACCOUNT,
            Title = "Apply for affiliate account",
            Published = this.GetSettingByKey($"{nameof(AffiliateSettings)}.{nameof(AffiliateSettings.AllowCustomersToApplyForAffiliateAccount)}", defaultValue: false)
        });
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {

    }

    #endregion
}



