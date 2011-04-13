using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Security;

namespace Nop.Data.Mapping.Security
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            this.ToTable("User");
            this.HasKey(p => p.Id);
            this.Property(u => u.Username);
            this.Property(u => u.Email).IsRequired();
            this.Property(u => u.Password).IsRequired();

            this.Ignore(u => u.PasswordFormat);
            
        }
    }
}
