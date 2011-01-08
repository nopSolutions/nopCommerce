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
            this.Ignore(u => u.PasswordFormat);
            this.HasRequired(u => u.Username);
            this.HasRequired(u => u.Email);
            this.HasRequired(u => u.Password);
            this.Property(u => u.UserGuid).IsRequired();
        }
    }
}
