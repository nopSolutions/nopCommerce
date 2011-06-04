using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Security;


namespace Nop.Services.Security
{
    public class UserService : IUserService
    {
        private readonly IEncryptionService _encryptionService;
        private readonly IRepository<User> _userRepository;
        private readonly UserSettings _userSettings;

        public UserService(IEncryptionService encryptionService, 
            IRepository<User> userRepository,
            UserSettings userSettings) 
        {
            this._encryptionService = encryptionService;
            this._userRepository = userRepository;
            this._userSettings = userSettings;
        }

        public virtual User GetUserById(int id) 
        {
            return _userRepository.GetById(id);
        }

        public virtual User GetUserByUsername(string username)
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

        public virtual User GetUserByEmail(string email)
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

        public virtual IPagedList<User> GetUsers(string email, string username, int pageIndex, int pageSize)
        {
            var query = _userRepository.Table;
            if (!String.IsNullOrWhiteSpace(email))
                query = query.Where(u => u.Email.Contains(email));
            if (!String.IsNullOrWhiteSpace(username))
                query = query.Where(u => u.Username.Contains(username));

            query = query.OrderByDescending(u => u.CreatedOnUtc);

            var users = new PagedList<User>(query, pageIndex, pageSize);
            return users;
        }

        public virtual void InsertUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            _userRepository.Insert(user);
        }

        public virtual void UpdateUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            _userRepository.Update(user);
        }

        public virtual void DeleteUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            _userRepository.Delete(user);
        }

        public virtual bool ValidateUser(string usernameOrEmail, string password)
        {
            User user = null;
            if (_userSettings.UsernamesEnabled)
                user = GetUserByUsername(usernameOrEmail);
            else
                user = GetUserByEmail(usernameOrEmail);

            if (user == null || user.IsLockedOut || !user.IsApproved)
                return false;

            string pwd = string.Empty;
            switch (user.PasswordFormat)
            {
                case PasswordFormat.Encrypted:
                    pwd = _encryptionService.EncryptText(password);
                    break;
                case PasswordFormat.Hashed:
                    pwd = _encryptionService.CreatePasswordHash(password, user.PasswordSalt, _userSettings.HashedPasswordFormat);
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
        
        public virtual UserRegistrationResult RegisterUser(UserRegistrationRequest request)
        {
            var result = new UserRegistrationResult();
            if (request == null || !request.IsValid)
            {
                result.AddError("The registration request was not valid.");
                return result;
            }

            //validation
            if (String.IsNullOrEmpty(request.Email))
            {
                result.AddError("Email is not provided");
                return result;
            }
            if (!CommonHelper.IsValidEmail(request.Email))
            {
                result.AddError("Invalid email");
                return result;
            }
            if (_userSettings.UsernamesEnabled)
            {
                if (String.IsNullOrEmpty(request.Username))
                {
                    result.AddError("Username is not provided");
                    return result;
                }
            }

            //validate unique user
            if (GetUserByEmail(request.Email) != null)
            {
                result.AddError("The specified email already exists");
                return result;
            }
            if (_userSettings.UsernamesEnabled)
            {
                if (GetUserByUsername(request.Username) != null)
                {
                    result.AddError("The specified username already exists");
                    return result;
                }
            }

            //at this point request is valid
            var user = new User();
            user.Username = request.Username;
            user.Email = request.Email;
            user.PasswordFormat = request.PasswordFormat;
            user.SecurityQuestion = request.SecurityQuestion;

            switch (request.PasswordFormat)
            {
                case PasswordFormat.Clear:
                    {
                        user.Password = request.Password;
                        user.SecurityAnswer = request.SecurityAnswer;
                    }
                    break;
                case PasswordFormat.Encrypted:
                    {
                        user.Password = _encryptionService.EncryptText(request.Password);
                        user.SecurityAnswer = _encryptionService.EncryptText(request.SecurityAnswer);
                    }
                    break;
                case PasswordFormat.Hashed:
                    {
                        string saltKey = _encryptionService.CreateSaltKey(5);
                        user.PasswordSalt = saltKey;
                        user.Password = _encryptionService.CreatePasswordHash(request.Password, saltKey, _userSettings.HashedPasswordFormat);
                        user.SecurityAnswer = _encryptionService.CreatePasswordHash(request.SecurityAnswer, saltKey);
                    }
                    break;
                default:
                    break;
            }

            user.UserGuid = Guid.NewGuid();
            user.IsApproved = request.IsApproved;
            user.CreatedOnUtc = DateTime.UtcNow;

            InsertUser(user);

            result.User = user;
            return result;
        }

        public virtual PasswordChangeResult ChangePassword(ChangePasswordRequest request)
        {
            var result = new PasswordChangeResult();
            if (request == null || !request.IsValid)
            {
                result.AddError("The registration request was not valid.");
                return result;
            }

            var user = GetUserByEmail(request.Email);
            if (user == null)
            {
                result.AddError("The specified email could not be found");
                return result;
            }


            var requestIsValid = false;
            if (request.ValidateRequest)
            {
                //password
                string oldPwd = string.Empty;
                switch (user.PasswordFormat)
                {
                    case PasswordFormat.Encrypted:
                        oldPwd = _encryptionService.EncryptText(request.OldPassword);
                        break;
                    case PasswordFormat.Hashed:
                        oldPwd = _encryptionService.CreatePasswordHash(request.OldPassword, user.PasswordSalt, _userSettings.HashedPasswordFormat);
                        break;
                    default:
                        oldPwd = request.OldPassword;
                        break;
                }

                bool oldPasswordIsValid = oldPwd == user.Password;
                if (!oldPasswordIsValid)
                    result.AddError("Old password doesn't match");

                //validate security answer
                bool securityAnswerIsValid = true;
                //UNDONE validate security answer
                //result.AddError("Wrong security answer");


                if (oldPasswordIsValid && securityAnswerIsValid)
                requestIsValid = true;
            }
            else
                requestIsValid = true;


            //at this point request is valid
            if (requestIsValid)
            {
                switch (request.NewPasswordFormat)
                {
                    case PasswordFormat.Clear:
                        {
                            user.Password = request.NewPassword;
                        }
                        break;
                    case PasswordFormat.Encrypted:
                        {
                            user.Password = _encryptionService.EncryptText(request.NewPassword);
                        }
                        break;
                    case PasswordFormat.Hashed:
                        {
                            string saltKey = _encryptionService.CreateSaltKey(5);
                            user.PasswordSalt = saltKey;
                            user.Password = _encryptionService.CreatePasswordHash(request.NewPassword, saltKey, _userSettings.HashedPasswordFormat);
                        }
                        break;
                    default:
                        break;
                }
                user.PasswordFormat = request.NewPasswordFormat;
                UpdateUser(user);
            }

            return result;
        }

        /// <summary>
        /// Sets a user email
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="newEmail">New email</param>
        public virtual void SetEmail(User user, string newEmail)
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
        public virtual void SetUsername(User user, string newUsername)
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
