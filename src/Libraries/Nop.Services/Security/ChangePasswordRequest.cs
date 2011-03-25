using Nop.Core;
using Nop.Core.Domain.Security;

namespace Nop.Services.Security
{
    public class ChangePasswordRequest
    {
        public string Email { get; set; }
        public bool ValidateRequest { get; set; }
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }
        public string SecurityAnswer { get; set; }

        public ChangePasswordRequest(string email, bool validateRequest,
            string newPassword, string oldPassword = "", string securityAnswer = "")
        {
            this.Email = email;
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
