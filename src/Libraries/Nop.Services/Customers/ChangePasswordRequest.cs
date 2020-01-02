using Nop.Core.Domain.Customers;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Change password request
    /// </summary>
    public class ChangePasswordRequest
    {
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// A value indicating whether we should validate request
        /// </summary>
        public bool ValidateRequest { get; set; }

        /// <summary>
        /// Password format
        /// </summary>
        public PasswordFormat NewPasswordFormat { get; set; }

        /// <summary>
        /// New password
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// Old password
        /// </summary>
        public string OldPassword { get; set; }

        /// <summary>
        /// Hashed password format (e.g. SHA1, SHA512)
        /// </summary>
        public string HashedPasswordFormat { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="validateRequest">A value indicating whether we should validate request</param>
        /// <param name="newPasswordFormat">Password format</param>
        /// <param name="newPassword">New password</param>
        /// <param name="oldPassword">Old password</param>
        /// <param name="hashedPasswordFormat">Hashed password format</param>
        public ChangePasswordRequest(string email, bool validateRequest,
            PasswordFormat newPasswordFormat, string newPassword, string oldPassword = "",
            string hashedPasswordFormat = null)
        {
            Email = email;
            ValidateRequest = validateRequest;
            NewPasswordFormat = newPasswordFormat;
            NewPassword = newPassword;
            OldPassword = oldPassword;
            HashedPasswordFormat = hashedPasswordFormat;
        }
    }
}