using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Areas.Admin.Models.Security;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the security model factory implementation
    /// </summary>
    public partial class SecurityModelFactory : ISecurityModelFactory
    {
        #region Fields

        protected ICustomerService CustomerService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IPermissionService PermissionService { get; }

        #endregion

        #region Ctor

        public SecurityModelFactory(ICustomerService customerService,
            ILocalizationService localizationService,
            IPermissionService permissionService)
        {
            CustomerService = customerService;
            LocalizationService = localizationService;
            PermissionService = permissionService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare permission mapping model
        /// </summary>
        /// <param name="model">Permission mapping model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the permission mapping model
        /// </returns>
        public virtual async Task<PermissionMappingModel> PreparePermissionMappingModelAsync(PermissionMappingModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var customerRoles = await CustomerService.GetAllCustomerRolesAsync(true);
            model.AvailableCustomerRoles = customerRoles.Select(role => role.ToModel<CustomerRoleModel>()).ToList();

            foreach (var permissionRecord in await PermissionService.GetAllPermissionRecordsAsync())
            {
                model.AvailablePermissions.Add(new PermissionRecordModel
                {
                    Name = await LocalizationService.GetLocalizedPermissionNameAsync(permissionRecord),
                    SystemName = permissionRecord.SystemName
                });

                foreach (var role in customerRoles)
                {
                    if (!model.Allowed.ContainsKey(permissionRecord.SystemName))
                        model.Allowed[permissionRecord.SystemName] = new Dictionary<int, bool>();
                    model.Allowed[permissionRecord.SystemName][role.Id] = 
                        (await PermissionService.GetMappingByPermissionRecordIdAsync(permissionRecord.Id)).Any(mapping => mapping.CustomerRoleId == role.Id);
                }
            }

            return model;
        }

        #endregion
    }
}