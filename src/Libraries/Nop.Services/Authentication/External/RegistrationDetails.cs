//Contributor:  Nicholas Mayne


namespace Nop.Services.Authentication.External
{
    public struct RegistrationDetails
    {
        public RegistrationDetails(OpenAuthenticationParameters parameters)
            : this()
        {
            if (parameters.UserClaims != null)
                foreach (var claim in parameters.UserClaims)
                {
                    if (string.IsNullOrEmpty(EmailAddress))
                    {
                        EmailAddress = claim.Contact.Email;
                        UserName = claim.Contact.Email;
                    }
                }
        }

        public string UserName { get; set; }
        public string EmailAddress { get; set; }
    }
}