//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): planetcloud (http://www.planetcloud.co.uk). 
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Data;
using Nop.Core.Domain.Security;
using Nop.Core;

namespace Nop.Services.Security
{
    public class UserService : IUserService
    {
        private readonly IEncryptionService encryptionService;
        private readonly IRepository<User> userRepository;
        string encryptionKey = "273ece6f97dd844d"; // TODO - inject

        public UserService(IEncryptionService encryptionService, IRepository<User> userRepository) {
            this.encryptionService = encryptionService;
            this.userRepository = userRepository;
        }

        public User GetUserById(int id) {
            return userRepository.GetById(id);
        }

        public User GetUserByUsername(string username) {
            return userRepository.Table.Where(u => u.Username == username).SingleOrDefault();
        }

        public User GetUserByEmail(string email) {
            return userRepository.Table.Where(u => u.Email == email).SingleOrDefault();
        }

        public IPagedList<User> GetUsers(int pageIndex, int pageSize) {
            return new PagedList<User>(userRepository.Table, pageIndex, pageSize);
        }

        public void InsertUser(User user) {
            userRepository.Insert(user);
        }

        public void UpdateUser(User user) {
            userRepository.Update(user);
        }

        public void DeleteUser(int id) {
            var userToDelete = GetUserById(id);
            if (userToDelete != null)
                userRepository.Delete(userToDelete);
        }

        public bool ValidateUser(string username, string password) {
            var user = GetUserByUsername(username);

            if (user == null || user.IsLockedOut)
                return false;

            string pwd = string.Empty;
            switch (user.PasswordFormat)
            {
                case PasswordFormat.Encrypted:
                    pwd = encryptionService.EncryptText(password, encryptionKey);
                    break;
                case PasswordFormat.Hashed:
                    //TODO pass ICustomerSettings.CustomerPasswordFormat
                    pwd = encryptionService.CreatePasswordHash(password, user.PasswordSalt);
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

            var existingUser = userRepository.Table
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
                    user.Password = encryptionService.EncryptText(request.Password, encryptionKey);
                    user.SecurityAnswer = encryptionService.EncryptText(request.SecurityAnswer, encryptionKey);
                    break;
                case PasswordFormat.Hashed:
                    string saltKey = encryptionService.CreateSaltKey(5);
                    user.PasswordSalt = saltKey;
                    //TODO pass password format (customerSettings.CustomerPasswordFormat)
                    user.Password = encryptionService.CreatePasswordHash(request.Password, saltKey);
                    user.SecurityAnswer = encryptionService.CreatePasswordHash(request.SecurityAnswer, saltKey);
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
    }
}
