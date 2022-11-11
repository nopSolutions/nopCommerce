using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Security;
using Nop.Services.Customers;
using Nop.Services.Security;
using Nop.Web.Framework.Models;

namespace Nop.Web.Framework.Factories
{
    /// <summary>
    /// Represents the base implementation of the factory of model which supports access control list (ACL)
    /// </summary>
    public partial class AclSupportedModelFactory : IAclSupportedModelFactory
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly ICustomerService _customerService;

        #endregion

        #region Ctor

        public AclSupportedModelFactory(IAclService aclService,
            ICustomerService customerService)
        {
            _aclService = aclService;
            _customerService = customerService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare selected and all available customer roles for the passed model
        /// </summary>
        /// <typeparam name="TModel">ACL supported model type</typeparam>
        /// <param name="model">Model</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task PrepareModelCustomerRolesAsync<TModel>(TModel model) where TModel : IAclSupportedModel
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //prepare available customer roles
            var availableRoles = await _customerService.GetAllCustomerRolesAsync(showHidden: true);
            model.AvailableCustomerRoles = availableRoles.Select(role => new SelectListItem
            {
                Text = role.Name,
                Value = role.Id.ToString(),
                Selected = model.SelectedCustomerRoleIds.Contains(role.Id)
            }).ToList();
        }

        /// <summary>
        /// Prepare selected and all available customer roles for the passed model by ACL mappings
        /// </summary>
        /// <typeparam name="TModel">ACL supported model type</typeparam>
        /// <typeparam name="TEntity">ACL supported entity type</typeparam>
        /// <param name="model">Model</param>
        /// <param name="entity">Entity</param>
        /// <param name="ignoreAclMappings">Whether to ignore existing ACL mappings</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task PrepareModelCustomerRolesAsync<TModel, TEntity>(TModel model, TEntity entity, bool ignoreAclMappings)
            where TModel : IAclSupportedModel where TEntity : BaseEntity, IAclSupported
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //prepare customer roles with granted access
            if (!ignoreAclMappings && entity != null)
                model.SelectedCustomerRoleIds = (await _aclService.GetCustomerRoleIdsWithAccessAsync(entity)).ToList();

            await PrepareModelCustomerRolesAsync(model);
        }

        #endregion
    }
}