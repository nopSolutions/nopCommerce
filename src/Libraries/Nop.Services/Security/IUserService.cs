using Nop.Core;
using Nop.Core.Domain.Security;

namespace Nop.Services.Security
{
    public interface IUserService
    {
        User GetUserById(int id);
        User GetUserByUsername(string username);
        User GetUserByEmail(string email);
        IPagedList<User> GetUsers(int pageIndex, int pageSize);
        UserRegistrationResult RegisterUser(UserRegistrationRequest request);
        void InsertUser(User user);
        void UpdateUser(User user);
        void DeleteUser(int id);
        bool ValidateUser(string username, string password);
        void SetEmail(User user, string newEmail);
        void SetUsername(User user, string newUsername);

        // TODO PasswordChangeResult ChangePassword(ChangePasswordRequest request)
    }
}
