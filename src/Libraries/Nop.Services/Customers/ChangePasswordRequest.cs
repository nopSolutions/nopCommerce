using Nop.Core;
using Nop.Core.Domain.Customers;

namespace Nop.Services.Customers
{
    public class ChangePasswordRequest
    {
        public string Email { get; set; }
        public bool ValidateRequest { get; set; }
        public PasswordFormat NewPasswordFormat { get; set; }
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }

        public ChangePasswordRequest(string email, bool validateRequest, 
            PasswordFormat newPasswordFormat, string newPassword, string oldPassword = "")
        {
            this.Email = email;
            this.ValidateRequest = validateRequest;
            this.NewPasswordFormat = newPasswordFormat;
            this.NewPassword = newPassword;
            this.OldPassword = oldPassword;
        }
    }
}
