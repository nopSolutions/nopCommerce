
namespace Nop.Core.Domain.Customers 
{ 
    public enum PasswordFormat : int
    {
        Clear = 0,
        Hashed = 1,
        Encrypted = 2
    }
}
