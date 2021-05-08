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

        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public SecurityModelFactory(ICustomerService customerService,
            ILocalizationService localizationService,
            IPermissionService permissionService)
        {
            _customerService = customerService;
            _localizationService = localizationService;
            _permissionService = permissionService;
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

            var customerRoles = await _customerService.GetAllCustomerRolesAsync(true);
            model.AvailableCustomerRoles = customerRoles.Select(role => role.ToModel<CustomerRoleModel>()).ToList();

            foreach (var permissionRecord in await _permissionService.GetAllPermissionRecordsAsync())
            {
                model.AvailablePermissions.Add(new PermissionRecordModel
                {
                    Name = await _localizationService.GetLocalizedPermissionNameAsync(permissionRecord),
                    SystemName = permissionRecord.SystemName
                });

                foreach (var role in customerRoles)
                {
                    if (!model.Allowed.ContainsKey(permissionRecord.SystemName))
                        model.Allowed[permissionRecord.SystemName] = new Dictionary<int, bool>();
                    model.Allowed[permissionRecord.SystemName][role.Id] = 
                        (await _permissionService.GetMappingByPermissionRecordIdAsync(permissionRecord.Id)).Any(mapping => mapping.CustomerRoleId == role.Id);
                }
            }

            return model;
        }

        #endregion
    }
}