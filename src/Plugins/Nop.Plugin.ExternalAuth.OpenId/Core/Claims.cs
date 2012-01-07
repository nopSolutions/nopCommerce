//Contributor:  Nicholas Mayne


using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.Messages;

namespace Nop.Plugin.ExternalAuth.OpenId.Core
{
    public static class Claims
    {
        public static IOpenIdMessageExtension CreateClaimsRequest()
        {
            var claimsRequest = new ClaimsRequest();

            //claimsRequest.BirthDate = DemandLevel.Require;
            //claimsRequest.Country = DemandLevel.Require;
            claimsRequest.Email = DemandLevel.Require;
            claimsRequest.FullName = DemandLevel.Require;
            //claimsRequest.Gender = DemandLevel.Require;
            //claimsRequest.Language = DemandLevel.Require;
            //claimsRequest.Nickname = DemandLevel.Require;
            //claimsRequest.PostalCode = DemandLevel.Require;
            //claimsRequest.TimeZone = DemandLevel.Require;

            return claimsRequest;
        }

        public static FetchRequest CreateFetchRequest()
        {
            var fetchRequest = new FetchRequest();

            fetchRequest.Attributes.AddRequired(WellKnownAttributes.Contact.Email);
            fetchRequest.Attributes.AddRequired(WellKnownAttributes.Name.First);
            fetchRequest.Attributes.AddRequired(WellKnownAttributes.Name.Last);
            fetchRequest.Attributes.AddRequired(WellKnownAttributes.Name.FullName);
            fetchRequest.Attributes.AddRequired(WellKnownAttributes.Name.Alias);

            return fetchRequest;
        }
    }
}