using Nop.Core;
using Nop.Core.Domain.Security;

namespace Nop.Services.Security
{
    public class UserRegistrationRequest
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public PasswordFormat PasswordFormat { get; set; }
        public string SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }
        public bool IsApproved { get; set; }

        public UserRegistrationRequest(string email, string username,
            string password, 
            PasswordFormat passwordFormat,
            string securityQuestion, 
            string securityAnswer, 
            bool isApproved = true)
        {
            this.Email = email;
            this.Username = username;
            this.Password = password;
            this.PasswordFormat = passwordFormat;
            this.SecurityAnswer = securityAnswer;
            this.SecurityQuestion = securityQuestion;
            this.IsApproved = isApproved;
        }

        public bool IsValid  
        {
            get 
            {
                return (!CommonHelper.AreNullOrEmpty(
                            this.Email,
                            //TODO validate Username only when usernames are enabled
                            //this.Username,
                            this.Password
                            //TODO make configurable whether SecurityAnswer/SecurityQuestion are required
                            //this.SecurityAnswer,
                            //this.SecurityQuestion
                            ));
            }
        }
    }
}
