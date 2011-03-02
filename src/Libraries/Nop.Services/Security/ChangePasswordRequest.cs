using Nop.Core;
using Nop.Core.Domain.Security;

namespace Nop.Services.Security
{
    public class ChangePasswordRequest 
    {
        public string Username { get; set; }
        public bool ValidateRequest { get; set; }
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }
        public string SecurityAnswer { get; set; }

        public ChangePasswordRequest(string username, bool validateRequest,
            string newPassword, string oldPassword = "", string securityAnswer = "") 
        {
            this.Username = username;
            this.ValidateRequest = validateRequest;
            this.NewPassword = newPassword;
            this.OldPassword = oldPassword;
            this.SecurityAnswer = securityAnswer;
        }

        public bool IsValid  
        {
            get 
            { 
                return true;
            }
        }
    }
}
