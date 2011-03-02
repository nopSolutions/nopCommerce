using Nop.Core;
using Nop.Core.Domain.Security;

namespace Nop.Services.Security
{
    public class UserRegistrationRequest 
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public PasswordFormat PasswordFormat { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }
        public bool IsApproved { get; set; }

        public UserRegistrationRequest(string username, string password, PasswordFormat passwordFormat, string firstName, string lastName,
            string email, string securityQuestion, string securityAnswer, bool isApproved = true) {
            this.Username = username;
            this.Password = password;
            this.PasswordFormat = passwordFormat;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.SecurityAnswer = securityAnswer;
            this.SecurityQuestion = securityQuestion;
            this.IsApproved = isApproved;
        }

        public bool IsValid  {
            get { 
                return (!CommonHelper.AreNullOrEmpty(
                            this.Username,
                            this.Password,
                            this.Email,
                            this.SecurityAnswer,
                            this.SecurityQuestion));
            }
        }
    }
}
