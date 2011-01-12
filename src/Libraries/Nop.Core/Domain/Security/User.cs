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
        public virtual string Username { get; set; }
        public virtual string Email { get; set; }
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
        public virtual DateTime CreatedOnUtc { get; set; }
        public virtual DateTime? LastLoginDateUtc { get; set; }
        public virtual DateTime? LastLockedOutDateUtc { get; set; }
        public virtual DateTime? LastPasswordChangeDateUtc { get; set; }
        public virtual int FailedPasswordAttemptCount { get; set; }

        public virtual string FullName {
            get { return FirstName + " " + LastName; }
        }
    }
}
