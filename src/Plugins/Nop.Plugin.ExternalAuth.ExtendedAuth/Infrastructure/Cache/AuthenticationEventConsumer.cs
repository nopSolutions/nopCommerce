using Microsoft.AspNetCore.StaticFiles;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Services.Authentication.External;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Media;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Nop.Plugin.ExternalAuth.ExtendedAuthentication.Infrastructure.Cache
{
    /// <summary>
    /// Facebook authentication event consumer (used for saving customer fields on registration)
    /// </summary>
    public partial class AuthenticationEventConsumer : IConsumer<CustomerAutoRegisteredByExternalMethodEvent>
    {
        #region Methods

        public void HandleEvent(CustomerAutoRegisteredByExternalMethodEvent eventMessage)
        {
            if (eventMessage?.Customer == null || eventMessage.AuthenticationParameters == null)
                return;

            //handle event only for this authentication method
            if (!eventMessage.AuthenticationParameters.ProviderSystemName.Equals(AuthenticationDefaults.PluginSystemName)) //FacebookAuthenticationDefaults.ProviderSystemName))
                return;

            var _genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
            var _customerService = EngineContext.Current.Resolve<ICustomerService>();
            var _customerSettings = EngineContext.Current.Resolve<CustomerSettings>();
            var _localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            var _pictureService = EngineContext.Current.Resolve<IPictureService>();
     
            //store some of the customer fields
            string name = string.Empty;
            string firstName = string.Empty;
            string lastName = string.Empty;

            // first Name
            List<string> firstNamesString = new List<string> {
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/first_name",
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/firstName",
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/given_name",
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname",
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/screen_name",
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name" };

            foreach (var names in firstNamesString)
            {                
                firstName = eventMessage.AuthenticationParameters.Claims?.FirstOrDefault(claim => claim.Type == names)?.Value;                
                if (!string.IsNullOrEmpty(firstName) && (firstName.Contains("_") || firstName.Contains(" ")))
                {
                    
                    var fullName =firstName.Contains("_") ?firstName.Split('_').ToArray() : firstName.Split(' ').ToArray();
                    firstName = fullName.FirstOrDefault();
                    lastName = fullName.LastOrDefault();

                    _genericAttributeService.SaveAttribute(eventMessage.Customer, NopCustomerDefaults.FirstNameAttribute, firstName);
                    _genericAttributeService.SaveAttribute(eventMessage.Customer, NopCustomerDefaults.LastNameAttribute, lastName);                    
                    break;
                }
                if (!string.IsNullOrEmpty(firstName))
                {
                    _genericAttributeService.SaveAttribute(eventMessage.Customer, NopCustomerDefaults.FirstNameAttribute, firstName);
                    break;
                }
            }

            // Last Name
            if (string.IsNullOrEmpty(lastName))
            {
                List<string> lastNamesString = new List<string> { "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/last_name",
                    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname",
                    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/lastName" };
                foreach (var lNames in lastNamesString)
                {
                    lastName = eventMessage.AuthenticationParameters.Claims?.FirstOrDefault(claim => claim.Type == lNames)?.Value;
                    if (!string.IsNullOrEmpty(lastName))
                    {
                        _genericAttributeService.SaveAttribute(eventMessage.Customer, NopCustomerDefaults.LastNameAttribute, lastName);
                        continue;
                    }
                }
            }

            //upload avatar
            var avatarUrl = eventMessage.AuthenticationParameters.Claims.FirstOrDefault(claim => claim.Type == AuthenticationDefaults.AvatarClaimType)?.Value;
            if (string.IsNullOrEmpty(avatarUrl))
                return;

            if (!_customerSettings.AllowCustomersToUploadAvatars)
                return;
            try
            {
                //try to get byte array of the user avatar image
                byte[] customerPictureBinary;
                using (var webClient = new WebClient())
                    customerPictureBinary = webClient.DownloadData(avatarUrl);

                if (customerPictureBinary.Length > _customerSettings.AvatarMaximumSizeBytes)
                {                    
                    return;
                }

                //save avatar
                new FileExtensionContentTypeProvider().TryGetContentType(avatarUrl, out string mimeType);
                var customerAvatar = _pictureService.InsertPicture(customerPictureBinary, mimeType ?? MimeTypes.ImagePng, null);
                _genericAttributeService.SaveAttribute(eventMessage.Customer, NopCustomerDefaults.AvatarPictureIdAttribute, customerAvatar.Id);
            }
            catch { }

        }
        #endregion
    }
}