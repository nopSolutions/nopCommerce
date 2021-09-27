using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Companies;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Services.Authentication;
using Nop.Services.Common;
using Nop.Services.Companies;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;

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
        private readonly IAddressService _addressService;
        private readonly IEncryptionService _encryptionService;
        private readonly MediaSettings _mediaSettings;
        private readonly IPictureService _pictureService;
        private readonly ICompanyService _companyService;

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
            IConfiguration config,
            IAddressService addressService,
            IEncryptionService encryptionService,
            MediaSettings mediaSettings,
            IPictureService pictureService,
            ICompanyService companyService)
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
            _addressService = addressService;
            _encryptionService = encryptionService;
            _mediaSettings = mediaSettings;
            _pictureService = pictureService;
            _companyService = companyService;
        }

        #endregion

        #region Methods

        public class LoginApiModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string PushToken { get; set; }
            public string GoogleToken { get; set; }
        }

        //to serialize json into class
        public class GoogleTokenClass
        {
            public string iss { get; set; }
            public string azp { get; set; }
            public string aud { get; set; }
            public string sub { get; set; }
            public string email { get; set; }
            public string email_verified { get; set; }
            public string at_hash { get; set; }
            public string name { get; set; }
            public string picture { get; set; }
            public string given_name { get; set; }
            public string family_name { get; set; }
            public string locale { get; set; }
            public string iat { get; set; }
            public string exp { get; set; }
            public string alg { get; set; }
            public string kid { get; set; }
            public string typ { get; set; }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginApiModel model)
        {
            if (!ModelState.IsValid)
                return Ok(new { success = false, message = GetModelErrors(ModelState) });

            var loginResult = await _customerRegistrationService.ValidateCustomerAsync(model.Email, model.Password);

            //checking if customer comes from goolge
            if (!string.IsNullOrWhiteSpace(model.GoogleToken))
            {
                //get json from the token url
                var json = new WebClient().DownloadString("https://oauth2.googleapis.com/tokeninfo?id_token=" + model.GoogleToken);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    GoogleTokenClass deserializedGoogleToken = new GoogleTokenClass();
                    try
                    {
                        //deserialized json into Google Token class
                        deserializedGoogleToken = JsonConvert.DeserializeObject<GoogleTokenClass>(json);
                    }
                    catch (Exception)
                    {
                        return Ok(new
                        {
                            success = false,
                            message = _localizationService.GetResourceAsync("Google.Token.IsNotValid")
                        });
                    }
                    //get customer
                    var customer = await _customerService.GetCustomerByEmailAsync(deserializedGoogleToken.email);
                    if (customer != null)
                    {
                        loginResult = CustomerLoginResults.Successful;
                        model.Email = deserializedGoogleToken.email;

                        if (!customer.Active)
                            loginResult = CustomerLoginResults.NotActive;
                    }
                    else
                    {
                        bool isApproved = false;

                        //Save a new record
                        await _workContext.SetCurrentCustomerAsync(await _customerService.InsertGuestCustomerAsync());

                        var newCustomer = await _workContext.GetCurrentCustomerAsync();

                        //checking if the email matches to any company email
                        var companies = await _companyService.GetAllCompaniesAsync(email: deserializedGoogleToken.email.Split('@')[1]);
                        if (companies.Any())
                        {
                            var companyId = companies.FirstOrDefault().Id;
                            isApproved = true;
                            await _companyService.InsertCompanyCustomerAsync(new CompanyCustomer { CompanyId = companyId, CustomerId = newCustomer.Id });

                            //adding 
                            var addressId = 0;
                            var existingCompanyCustomers = await _companyService.GetCompanyCustomersByCompanyIdAsync(companyId, showHidden: true);
                            if (existingCompanyCustomers.Any())
                            {
                                var addresses = await _customerService.GetAddressesByCustomerIdAsync(existingCompanyCustomers.FirstOrDefault().CustomerId);
                                foreach (var address in addresses)
                                {
                                    await _customerService.InsertCustomerAddressAsync(newCustomer, address);
                                    addressId = address.Id;
                                }

                            }
                            if (addressId > 0)
                            {
                                newCustomer.ShippingAddressId = addressId;
                                newCustomer.BillingAddressId = addressId;
                                await _customerService.UpdateCustomerAsync(newCustomer);
                            }
                        }

                        //registering new customer 
                        var registrationRequest = new CustomerRegistrationRequest(newCustomer,
                       deserializedGoogleToken.email, deserializedGoogleToken.email,
                       CommonHelper.GenerateRandomDigitCode(12),
                       _customerSettings.DefaultPasswordFormat,
                       (await _storeContext.GetCurrentStoreAsync()).Id,
                       isApproved);
                        var registrationResult = await _customerRegistrationService.RegisterCustomerAsync(registrationRequest);
                        if (registrationResult.Success)
                        {
                            if (_customerSettings.FirstNameEnabled)
                                await _genericAttributeService.SaveAttributeAsync(newCustomer, NopCustomerDefaults.FirstNameAttribute, deserializedGoogleToken.given_name);
                            if (_customerSettings.LastNameEnabled)
                                await _genericAttributeService.SaveAttributeAsync(newCustomer, NopCustomerDefaults.LastNameAttribute, deserializedGoogleToken.family_name);
                            loginResult = CustomerLoginResults.Successful;
                            model.Email = deserializedGoogleToken.email;
                        }
                    }
                }

            }
            switch (loginResult)
            {
                case CustomerLoginResults.Successful:
                    {
                        var customer = await _customerService.GetCustomerByEmailAsync(model.Email);
                        if (customer == null)
                            return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Customer.Not.Found") });

                        customer.PushToken = model.PushToken;
                        await _customerService.UpdateCustomerAsync(customer);

                        await _workContext.SetCurrentCustomerAsync(customer);

                        //migrate shopping cart
                        await _shoppingCartService.MigrateShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), customer, true);

                        //sign in new customer
                        await _authenticationService.SignInAsync(customer, false);

                        var jwt = new JwtService(_config);
                        var token = jwt.GenerateSecurityToken(customer.Email, customer.Id);

                        var shippingAddress = customer.ShippingAddressId.HasValue ? await _addressService.GetAddressByIdAsync(customer.ShippingAddressId.Value) : null;

                        var firstName = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.FirstNameAttribute);
                        var lastName = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.LastNameAttribute);

                        return Ok(new
                        {
                            success = true,
                            message = await _localizationService.GetResourceAsync("Customer.Login.Successfully"),
                            token,
                            pushToken = customer.PushToken,
                            shippingAddress,
                            firstName,
                            lastName,
                            RemindMeNotification = customer.RemindMeNotification,
                            RateReminderNotification = customer.RateReminderNotification,
                            OrderStatusNotification = customer.OrderStatusNotification,
                            avatar = await _pictureService.GetPictureUrlAsync(await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute), _mediaSettings.AvatarPictureSize, true)
                        });
                    }
                case CustomerLoginResults.CustomerNotExist:
                    return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.CustomerNotExist") });
                case CustomerLoginResults.Deleted:
                    return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.Deleted") });
                case CustomerLoginResults.NotActive:
                    return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.NotActive") });
                case CustomerLoginResults.NotRegistered:
                    return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.NotRegistered") });
                case CustomerLoginResults.LockedOut:
                    return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.LockedOut") });
                case CustomerLoginResults.WrongPassword:
                default:
                    return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Account.Login.WrongCredentials") });
            }
        }

        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (customer == null)
                return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Customer.Not.Found") });

            //customer.PushToken = null;
            //await _customerService.UpdateCustomerAsync(customer);

            //standard logout 
            await _authenticationService.SignOutAsync();

            return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Customer.Logout.Successfully") });
        }

        [AllowAnonymous]
        [HttpGet("check-customer-token")]
        public async Task<IActionResult> CheckCustomerToken()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (customer == null)
                return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Customer.Not.Found") });

            var jwt = new JwtService(_config);
            var token = jwt.GenerateSecurityToken(customer.Email, customer.Id);
            var shippingAddress = customer.ShippingAddressId.HasValue ? await _addressService.GetAddressByIdAsync(customer.ShippingAddressId.Value) : null;
            var firstName = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.FirstNameAttribute);
            var lastName = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.LastNameAttribute);

            return Ok(new
            {
                success = true,
                token,
                pushToken = customer.PushToken,
                shippingAddress,
                firstName,
                lastName,
                RemindMeNotification = customer.RemindMeNotification,
                RateReminderNotification = customer.RateReminderNotification,
                OrderStatusNotification = customer.OrderStatusNotification,
                avatar = await _pictureService.GetPictureUrlAsync(await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute), _mediaSettings.AvatarPictureSize, true)
            });
        }

        #endregion
    }
}
