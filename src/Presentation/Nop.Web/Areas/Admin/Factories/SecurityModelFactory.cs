using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
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
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public SecurityModelFactory(ICustomerService customerService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IWorkContext workContext)
        {
            this._customerService = customerService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare permission mapping model
        /// </summary>
        /// <param name="model">Permission mapping model</param>
        /// <returns>Permission mapping model</returns>
        public virtual PermissionMappingModel PreparePermissionMappingModel(PermissionMappingModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var customerRoles = _customerService.GetAllCustomerRoles(true);
            model.AvailableCustomerRoles = customerRoles.Select(role => role.ToModel(new CustomerRoleModel())).ToList();

            foreach (var permissionRecord in _permissionService.GetAllPermissionRecords())
            {
                model.AvailablePermissions.Add(new PermissionRecordModel
                {
                    Name = permissionRecord.GetLocalizedPermissionName(_localizationService, _workContext),
                    SystemName = permissionRecord.SystemName
                });

                foreach (var role in customerRoles)
                {
                    if (!model.Allowed.ContainsKey(permissionRecord.SystemName))
                        model.Allowed[permissionRecord.SystemName] = new Dictionary<int, bool>();
                    model.Allowed[permissionRecord.SystemName][role.Id] = permissionRecord.PermissionRecordCustomerRoleMappings
                        .Any(mapping => mapping.CustomerRoleId == role.Id);
                }
            }

            return model;
        }

        #endregion
    }
}