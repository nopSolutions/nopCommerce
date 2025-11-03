using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Domain.Security;
using Nop.Data;
using Nop.Plugin.Api.Services;
using Nop.Services.Authentication;
using Nop.Services.Security;

namespace Nop.Plugin.Api.Authorization.Attributes
{
	/// <summary>
	/// Represents a filter attribute that confirms access to the admin panel
	/// </summary>
	public sealed class AuthorizePermissionAttribute : TypeFilterAttribute
	{
		#region Ctor

		public AuthorizePermissionAttribute(string permissionSystemName, bool ignore = false)
			: base(typeof(AuthorizePermissionFilter))
		{
			PermissionSystemName = permissionSystemName;
			IgnoreFilter = ignore;
			Arguments = new object[] { permissionSystemName, ignore };
		}


		#endregion

		#region Properties

		public string PermissionSystemName { get; }

		public bool IgnoreFilter { get; }

		#endregion

		#region Nested filter

		/// <summary>
		/// Represents a filter that confirms access to the admin panel
		/// </summary>
		private class AuthorizePermissionFilter : IAsyncAuthorizationFilter
		{
			#region Fields

			private readonly string permission;
			private readonly bool ignoreFilter;
			private readonly IPermissionService permissionService;
			private readonly IAuthenticationService authenticationService;

			#endregion

			#region Ctor

			public AuthorizePermissionFilter(string permission, bool ignoreFilter, IPermissionService permissionService, IAuthenticationService authenticationService)
			{
				this.permission = permission;
				this.ignoreFilter = ignoreFilter;
				this.permissionService = permissionService;
				this.authenticationService = authenticationService;
			}

			#endregion

			#region Private methods

			/// <summary>
			/// Called early in the filter pipeline to confirm request is authorized
			/// </summary>
			/// <param name="context">Authorization filter context</param>
			/// <returns>A task that represents the asynchronous operation</returns>
			private async Task AuthorizePermissionAsync(AuthorizationFilterContext context)
			{
				if (context == null)
					throw new ArgumentNullException(nameof(context));

				if (!DataSettingsManager.IsDatabaseInstalled())
					return;

				//check whether this filter has been overridden for the action
				var actionFilter = context.ActionDescriptor.FilterDescriptors
					.Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
					.Select(filterDescriptor => filterDescriptor.Filter)
					.OfType<AuthorizePermissionAttribute>()
					.Where(a => a.PermissionSystemName == this.permission)
					.FirstOrDefault();

				//ignore filter (the action is available even if navigation is not allowed)
				if (actionFilter is not null && actionFilter.IgnoreFilter)
					return; // ignore attribute on controller, allow access for anyone

				var customer = await authenticationService.GetAuthenticatedCustomerAsync();

				// check whether current customer has permission to access resource
				if (customer is not null && await permissionService.AuthorizeAsync(permission, customer))
					return; // authorized, allow access

				// current user hasn't access
				context.Result = new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status403Forbidden);
			}

			#endregion

			#region Methods

			/// <summary>
			/// Called early in the filter pipeline to confirm request is authorized
			/// </summary>
			/// <param name="context">Authorization filter context</param>
			/// <returns>A task that represents the asynchronous operation</returns>
			public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
			{
				await AuthorizePermissionAsync(context);
			}

			#endregion
		}

		#endregion
	}
}
