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

namespace Nop.Core.Domain.Security
{
    public class User : BaseEntity {
        
        public User() {
            this.UserGuid = Guid.NewGuid();
            this.PasswordFormat = Security.PasswordFormat.Clear;
        }

        public virtual Guid UserGuid { get; set; }
        public virtual string ApplicationName { get; set; }
        
        private string username;
        public virtual string Username {
            get { return this.username; }
            set {
                this.username = value;
                this.LoweredUsername = this.username.ToLower();
            }
        }

        private string email;
        public virtual string Email
        {
            get { return this.email;}
            set {
                this.email = value;
                this.LoweredEmail = this.email.ToLower();
            }
        }

        public virtual string LoweredUsername { get; set; }
        public virtual string LoweredEmail { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Password { get; set; }

        public virtual int PasswordFormatId { get; set; }
        public virtual PasswordFormat PasswordFormat {
            get { return (Security.PasswordFormat)PasswordFormatId; }
            set { this.PasswordFormatId = (int)value; }
        }
        
        public virtual string PasswordSalt { get; set; }
        public virtual string SecurityQuestion { get; set; }
        public virtual string SecurityAnswer { get; set; }
        public virtual string Comments { get; set; }
        public virtual bool IsApproved { get; set; }
        public virtual bool IsLockedOut { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        public virtual DateTime? LastActivityDate { get; set; }
        public virtual DateTime? LastLoginDate { get; set; }
        public virtual DateTime? LastLockedOutDate { get; set; }
        public virtual DateTime? LastPasswordChangeDate { get; set; }
        public virtual int FailedPasswordAttemptCount { get; set; }

        public virtual string FullName {
            get { return FirstName + " " + LastName; }
        }
    }
}
