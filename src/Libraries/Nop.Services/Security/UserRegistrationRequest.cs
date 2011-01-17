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
    public class UserRegistrationRequest 
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public PasswordFormat PasswordFormat { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }
        public bool IsApproved { get; set; }
        public string ApplicationName { get; set; }

        public UserRegistrationRequest(string username, string password, PasswordFormat passwordFormat, string firstName, string lastName,
            string email, string securityQuestion, string securityAnswer, string applicationName, bool isApproved = true) {
            this.Username = username;
            this.Password = password;
            this.PasswordFormat = passwordFormat;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.SecurityAnswer = securityAnswer;
            this.SecurityQuestion = securityQuestion;
            this.IsApproved = isApproved;
            this.ApplicationName = applicationName;
        }

        public bool IsValid  {
            get { 
                return (!CommonHelper.AreNullOrEmpty(
                            this.Username,
                            this.Password,
                            this.Email,
                            this.SecurityAnswer,
                            this.SecurityQuestion,
                            this.ApplicationName
                        ));
            }
        }
    }
}
