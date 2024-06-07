using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Security;
using Nop.Services.Customers;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Security;
using Nop.Web.Framework.Menu;
using Nop.Web.Framework.Mvc.Filters;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class SecurityController : BaseAdminController
{
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly ILogger _logger;
    protected readonly IPermissionService _permissionService;
    protected readonly ISecurityModelFactory _securityModelFactory;
    protected readonly IWorkContext _workContext;
    protected readonly IXmlSiteMap _xmlSiteMap;

    private static readonly char[] _separator = [','];
    private static Dictionary<string, string> _menuSystemNames = new();

    #endregion

    #region Ctor

    public SecurityController(ICustomerService customerService,
        ILogger logger,
        IPermissionService permissionService,
        ISecurityModelFactory securityModelFactory,
        IWorkContext workContext,
        IXmlSiteMap xmlSiteMap)
    {
        _customerService = customerService;
        _logger = logger;
        _permissionService = permissionService;
        _securityModelFactory = securityModelFactory;
        _workContext = workContext;
        _xmlSiteMap = xmlSiteMap;
    }
    
    #endregion

    #region Methods

    public virtual async Task<IActionResult> AccessDenied(string pageUrl, string pageSystemNameKey)
    {
        if (!_menuSystemNames.Any())
        {
            await _xmlSiteMap.LoadFromAsync("~/Areas/Admin/sitemap.config");

            void fillSystemNames(SiteMapNode node)
            {
                if (!string.IsNullOrEmpty(node.Url))
                    return;

                if (!string.IsNullOrEmpty(node.ControllerName) && !string.IsNullOrEmpty(node.ActionName))
                {
                    var key = $"{node.ControllerName}.{node.ActionName}";
                    _menuSystemNames[key] = node.SystemName;
                }

                foreach (var childNode in node.ChildNodes) 
                    fillSystemNames(childNode);
            }

            fillSystemNames(_xmlSiteMap.RootNode);
        }

        var currentCustomer = await _workContext.GetCurrentCustomerAsync();

        var menuSystemName = "Home";

        if (_menuSystemNames.ContainsKey(pageSystemNameKey))
            menuSystemName = _menuSystemNames[pageSystemNameKey];
        else
        {
            var systemName =
                _menuSystemNames.FirstOrDefault(item => item.Key.StartsWith(pageSystemNameKey.Split('.')[0]));
                
            if (!string.IsNullOrEmpty(systemName.Value))
                    menuSystemName = systemName.Value;
        }

        if (currentCustomer == null || await _customerService.IsGuestAsync(currentCustomer))
            await _logger.InformationAsync($"Access denied to anonymous request on {pageUrl}");
        else
            await _logger.InformationAsync($"Access denied to user #{currentCustomer.Id} '{currentCustomer.Email}' on {pageUrl}");

        return View(model: menuSystemName);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_ACL)]
    public virtual async Task<IActionResult> PermissionCategory(PermissionItemSearchModel searchModel)
    {
        var model = await _securityModelFactory.PreparePermissionItemListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_ACL)]
    public virtual async Task<IActionResult> PermissionEditPopup(int id)
    {
        var permissionRecord = await _permissionService.GetPermissionRecordByIdAsync(id);
        var model = await _securityModelFactory.PreparePermissionItemModelAsync(permissionRecord);

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_ACL)]
    public virtual async Task<IActionResult> PermissionEditPopup(PermissionItemModel model)
    {
        if (ModelState.IsValid)
        {
            var mapping = await _permissionService.GetMappingByPermissionRecordIdAsync(model.Id);

            var rolesForDelete = mapping.Where(p => !model.SelectedCustomerRoleIds.Contains(p.CustomerRoleId))
                .Select(p => p.CustomerRoleId).ToList();

            var rolesToAdd = model.SelectedCustomerRoleIds.Where(p => mapping.All(m => m.CustomerRoleId != p)).ToList();

            foreach (var customerRoleId in rolesForDelete)
                await _permissionService.DeletePermissionRecordCustomerRoleMappingAsync(model.Id, customerRoleId);

            foreach (var customerRoleId in rolesToAdd)
                await _permissionService.InsertPermissionRecordCustomerRoleMappingAsync(new PermissionRecordCustomerRoleMapping
                {
                    PermissionRecordId = model.Id,
                    CustomerRoleId = customerRoleId
                });
            ViewBag.RefreshPage = true;

            var permissionRecord = await _permissionService.GetPermissionRecordByIdAsync(model.Id);

            if (rolesForDelete.Any() || rolesToAdd.Any())
                //for clear cache
                await _permissionService.UpdatePermissionRecordAsync(permissionRecord);


            model = await _securityModelFactory.PreparePermissionItemModelAsync(permissionRecord);

            return View(model);
        }

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_ACL)]
    public virtual async Task<IActionResult> PermissionCategories(PermissionCategorySearchModel searchModel)
    {
        var model = await _securityModelFactory.PreparePermissionCategoryListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_ACL)]
    public virtual async Task<IActionResult> Permissions()
    {
        //prepare model
        var model = await _securityModelFactory.PreparePermissionConfigurationModelAsync(new PermissionConfigurationModel());

        return View(model);
    }

    #endregion
}