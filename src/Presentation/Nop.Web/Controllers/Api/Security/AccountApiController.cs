using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Authentication;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;

namespace Nop.Web.Controllers.Api.Security
{
    [Produces("application/json")]
    [Route("api/account")]
    public class AccountApiController : BaseApiController
    {
        #region Fields
        private readonly IStoreContext _storeContext;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerService _customerService;
        private readonly CustomerSettings _customerSettings;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IWorkContext _workContext;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IConfiguration _config;

        #endregion

        #region Ctor

        public AccountApiController(ICustomerRegistrationService customerRegistrationService,
            ICustomerService customerService,
            CustomerSettings customerSettings,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IWorkflowMessageService workflowMessageService,
             ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IWorkContext workContext,
            IAuthenticationService authenticationService,
            IShoppingCartService shoppingCartService,
            IConfiguration config)
        {
            _storeContext = storeContext;
            _customerRegistrationService = customerRegistrationService;
            _customerService = customerService;
            _customerSettings = customerSettings;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _workflowMessageService = workflowMessageService;
            _workContext = workContext;
            _countryService = countryService;
            _stateProvinceService = stateProvinceService;
            _authenticationService = authenticationService;
            _shoppingCartService = shoppingCartService;
            _config = config;
        }

        #endregion

        #region Methods

        public class LoginApiModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string PushToken { get; set; }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginApiModel model)
        {
            if (!ModelState.IsValid)
                return Ok(new { success = false, message = GetModelErrors(ModelState) });

            var loginResult = _customerRegistrationService.ValidateCustomer(model.Email, model.Password);
            switch (loginResult)
            {
                case CustomerLoginResults.Successful:
                    {
                        var customer = _customerService.GetCustomerByEmail(model.Email);
                        if (customer == null)
                            return Ok(new { success = false, message = "'User' could not be loaded" });

                        _genericAttributeService.SaveAttribute<string>(customer, NopCustomerDefaults.PushToken, model.PushToken, 1);

                        _workContext.CurrentCustomer.Id = customer.Id;

                        var numberGenerate = CommonHelper.GenerateRandomDigitCode(24);
                        var jwt = new JwtService(_config);
                        var token = jwt.GenerateSecurityToken(numberGenerate + "yo!Token");

                        return Ok(new { success = true, message = "Login Successfully", token });
                    }
                case CustomerLoginResults.CustomerNotExist:
                    return Ok(new { success = false, message = _localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist") });
                case CustomerLoginResults.Deleted:
                    return Ok(new { success = false, message = _localizationService.GetResource("Account.Login.WrongCredentials.Deleted") });
                case CustomerLoginResults.NotActive:
                    return Ok(new { success = false, message = _localizationService.GetResource("Account.Login.WrongCredentials.NotActive") });
                case CustomerLoginResults.NotRegistered:
                    return Ok(new { success = false, message = _localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered") });
                case CustomerLoginResults.LockedOut:
                    return Ok(new { success = false, message = _localizationService.GetResource("Account.Login.WrongCredentials.LockedOut") });
                case CustomerLoginResults.WrongPassword:
                default:
                    return Ok(new { success = false, message = _localizationService.GetResource("Account.Login.WrongCredentials") });
            }
        }
        
        #endregion
    }
}
