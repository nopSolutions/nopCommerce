//Contributor:  Nicholas Mayne


using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.Messages;
using Nop.Services.Authentication.External;

namespace Nop.Plugin.ExternalAuth.OpenId.Core
{
    public static class Claims
    {
        public static IOpenIdMessageExtension CreateClaimsRequest(
            IOpenAuthenticationProviderPermissionService openAuthenticationProviderPermissionService)
        {

            var claimsRequest = new ClaimsRequest();

            if (openAuthenticationProviderPermissionService.IsPermissionEnabled("Birthdate", Provider.SystemName))
                claimsRequest.BirthDate = DemandLevel.Require;

            if (openAuthenticationProviderPermissionService.IsPermissionEnabled("Country", Provider.SystemName))
                claimsRequest.Country = DemandLevel.Require;

            //            if (openAuthenticationProviderPermissionService.IsPermissionEnabled("Email", Provider.OpenId))
            claimsRequest.Email = DemandLevel.Require;

            //            if (openAuthenticationProviderPermissionService.IsPermissionEnabled("FullName", Provider.OpenId))
            claimsRequest.FullName = DemandLevel.Require;

            if (openAuthenticationProviderPermissionService.IsPermissionEnabled("Gender", Provider.SystemName))
                claimsRequest.Gender = DemandLevel.Require;

            if (openAuthenticationProviderPermissionService.IsPermissionEnabled("Language", Provider.SystemName))
                claimsRequest.Language = DemandLevel.Require;

            if (openAuthenticationProviderPermissionService.IsPermissionEnabled("Nickname", Provider.SystemName))
                claimsRequest.Nickname = DemandLevel.Require;

            if (openAuthenticationProviderPermissionService.IsPermissionEnabled("PostalCode", Provider.SystemName))
                claimsRequest.PostalCode = DemandLevel.Require;

            if (openAuthenticationProviderPermissionService.IsPermissionEnabled("TimeZone", Provider.SystemName))
                claimsRequest.TimeZone = DemandLevel.Require;

            return claimsRequest;
        }

        public static FetchRequest CreateFetchRequest(IOpenAuthenticationProviderPermissionService openAuthenticationProviderPermissionService)
        {
            var fetchRequest = new FetchRequest();

            //if (openAuthenticationProviderPermissionService.IsPermissionEnabled("Email", Provider.OpenId))
            fetchRequest.Attributes.AddRequired(WellKnownAttributes.Contact.Email);

            //if (openAuthenticationProviderPermissionService.IsPermissionEnabled("FullName", Provider.OpenId)) {
            fetchRequest.Attributes.AddRequired(WellKnownAttributes.Name.First);
            fetchRequest.Attributes.AddRequired(WellKnownAttributes.Name.Last);
            fetchRequest.Attributes.AddRequired(WellKnownAttributes.Name.FullName);
            fetchRequest.Attributes.AddRequired(WellKnownAttributes.Name.Alias);
            //}

            return fetchRequest;
        }
    }
}