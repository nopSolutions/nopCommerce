using Nop.Core;
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

        // TODO PasswordChangeResult ChangePassword(ChangePasswordRequest request)
    }
}
