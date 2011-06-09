
using System;
using Nop.Core.Domain.Customers;

namespace Nop.Core.Domain.Security
{
    public class User : BaseEntity {
        
        public User() 
        {
            this.UserGuid = Guid.NewGuid();
            this.PasswordFormat = PasswordFormat.Clear;
        }

        public virtual Guid UserGuid { get; set; }
        public virtual string Username { get; set; }
        public virtual string Email { get; set; }
        public virtual string Password { get; set; }

        public virtual int PasswordFormatId { get; set; }
        public virtual PasswordFormat PasswordFormat 
        {
            get { return (PasswordFormat)PasswordFormatId; }
            set { this.PasswordFormatId = (int)value; }
        }
        
        public virtual string PasswordSalt { get; set; }
        public virtual string Comments { get; set; }
        public virtual bool IsApproved { get; set; }
        public virtual bool IsLockedOut { get; set; }
        public virtual DateTime CreatedOnUtc { get; set; }
        public virtual DateTime? LastLoginDateUtc { get; set; }
        public virtual DateTime? LastLockedOutDateUtc { get; set; }
        public virtual DateTime? LastPasswordChangeDateUtc { get; set; }
        public virtual int FailedPasswordAttemptCount { get; set; }

        public virtual int? AssociatedCustomerId { get; set; }
        public virtual Customer AssociatedCustomer { get; set; }
    }
}
