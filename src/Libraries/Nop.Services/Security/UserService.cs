using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Security;
using Nop.Data;

namespace Nop.Services.Security
{
    public class UserService : IUserService
    {
        private readonly IEncryptionService _encryptionService;
        private readonly IRepository<User> _userRepository;
        private readonly UserSettings _userSettings;
        string encryptionKey = "273ece6f97dd844d"; // TODO - inject

        public UserService(IEncryptionService encryptionService, 
            IRepository<User> userRepository,
            UserSettings userSettings) 
        {
            this._encryptionService = encryptionService;
            this._userRepository = userRepository;
            this._userSettings = userSettings;
        }

        public User GetUserById(int id) {
            return _userRepository.GetById(id);
        }

        public User GetUserByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            var query = from c in _userRepository.Table
                        orderby c.Id
                        where c.Username == username
                        select c;
            var user = query.FirstOrDefault();
            return user;
        }

        public User GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var query = from c in _userRepository.Table
                        orderby c.Id
                        where c.Email == email
                        select c;
            var user = query.FirstOrDefault();
            return user;
        }

        public IPagedList<User> GetUsers(int pageIndex, int pageSize) {
            return new PagedList<User>(_userRepository.Table, pageIndex, pageSize);
        }

        public void InsertUser(User user) 
        {
            _userRepository.Insert(user);
        }

        public void UpdateUser(User user) 
        {
            _userRepository.Update(user);
        }

        public void DeleteUser(int id) 
        {
            var userToDelete = GetUserById(id);
            if (userToDelete != null)
                _userRepository.Delete(userToDelete);
        }

        public bool ValidateUser(string username, string password) {
            var user = GetUserByUsername(username);

            if (user == null || user.IsLockedOut)
                return false;

            string pwd = string.Empty;
            switch (user.PasswordFormat)
            {
                case PasswordFormat.Encrypted:
                    pwd = _encryptionService.EncryptText(password, encryptionKey);
                    break;
                case PasswordFormat.Hashed:
                    pwd = _encryptionService.CreatePasswordHash(password, user.PasswordSalt, _userSettings.PasswordFormat);
                    break;
                default:
                    pwd = password;
                    break;
            }

            bool isValid = pwd == user.Password;

            if (isValid)
                user.LastLoginDateUtc = DateTime.UtcNow;
            else
                user.FailedPasswordAttemptCount++;

            UpdateUser(user);

            return isValid;
        }
        
        public UserRegistrationResult RegisterUser(UserRegistrationRequest request) {
            var result = new UserRegistrationResult();
            if (request == null || !request.IsValid)
            {
                result.AddError("The registration request was not valid.");
                return result;
            }

            // validate unique user

            var existingUser = _userRepository.Table
                .Where(x => x.Username == request.Username || x.Email == request.Email)
                .FirstOrDefault();
            
            if (existingUser != null) {
                result.AddError("The specified username or email already exists");
                return result;
            }

            // at this point request is valid

            var user = new User();
            user.Username = request.Username;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.PasswordFormat = request.PasswordFormat;
            user.SecurityQuestion = request.SecurityQuestion;

            switch (request.PasswordFormat)
            {
                case PasswordFormat.Clear:
                    user.Password = request.Password;
                    user.SecurityAnswer = request.SecurityAnswer;
                    break;
                case PasswordFormat.Encrypted:
                    user.Password = _encryptionService.EncryptText(request.Password, encryptionKey);
                    user.SecurityAnswer = _encryptionService.EncryptText(request.SecurityAnswer, encryptionKey);
                    break;
                case PasswordFormat.Hashed:
                    string saltKey = _encryptionService.CreateSaltKey(5);
                    user.PasswordSalt = saltKey;
                    user.Password = _encryptionService.CreatePasswordHash(request.Password, saltKey, _userSettings.PasswordFormat);
                    user.SecurityAnswer = _encryptionService.CreatePasswordHash(request.SecurityAnswer, saltKey);
                    break;
                default:
                    break;
            }

            user.IsApproved = request.IsApproved;
            user.CreatedOnUtc = DateTime.UtcNow;

            InsertUser(user);

            result.User = user;
            return result;
        }
        
        /// <summary>
        /// Sets a user email
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="newEmail">New email</param>
        public void SetEmail(User user, string newEmail)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            newEmail = newEmail.Trim();

            if (!CommonHelper.IsValidEmail(newEmail))
                throw new NopException("New email is not valid");

            if (newEmail.Length > 100)
                throw new NopException("E-mail address is too long.");

            var user2 = GetUserByEmail(newEmail);
            if (user2 != null && user.Id != user2.Id)
                throw new NopException("The e-mail address is already in use.");

            user.Email = newEmail;
            UpdateUser(user);
        }

        /// <summary>
        /// Sets a user username
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="newUsername">New Username</param>
        public void SetUsername(User user, string newUsername)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (!_userSettings.UsernamesEnabled)
                throw new NopException("Usernames are disabled");

            if (!_userSettings.AllowUsersToChangeUsernames)
                throw new NopException("Changing usernames is not allowed");

            newUsername = newUsername.Trim();

            if (newUsername.Length > 100)
                throw new NopException("Username is too long.");

            var user2 = GetUserByUsername(newUsername);
            if (user2 != null && user.Id != user2.Id)
                throw new NopException("This username is already in use.");

            user.Username = newUsername;
            UpdateUser(user);
        }

    }
}
