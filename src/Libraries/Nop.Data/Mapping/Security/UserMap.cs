using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Security;

namespace Nop.Data.Mapping.Security
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            this.ToTable("Users");
            this.Property(u => u.Username).IsRequired();
            this.Property(u => u.Email).IsRequired();
            this.Property(u => u.Password).IsRequired();
            this.Property(u => u.UserGuid).IsRequired();

            this.Ignore(u => u.PasswordFormat);
            this.Ignore(u => u.FullName);
            
        }
    }
}
