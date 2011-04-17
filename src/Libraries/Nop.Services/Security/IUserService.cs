using Nop.Core;
using Nop.Core.Domain.Security;

namespace Nop.Services.Security
{
    public interface IUserService
    {
        User GetUserById(int id);
        User GetUserByUsername(string username);
        User GetUserByEmail(string email);
        IPagedList<User> GetUsers(string email, string username, int pageIndex, int pageSize);
        void InsertUser(User user);
        void UpdateUser(User user);
        void DeleteUser(User user);

        bool ValidateUser(string usernameOrEmail, string password);
        UserRegistrationResult RegisterUser(UserRegistrationRequest request);
        PasswordChangeResult ChangePassword(ChangePasswordRequest request);
        void SetEmail(User user, string newEmail);
        void SetUsername(User user, string newUsername);
    }
}
