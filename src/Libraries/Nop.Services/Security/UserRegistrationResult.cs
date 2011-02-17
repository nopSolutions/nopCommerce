using System.Collections.Generic;
using Nop.Core.Domain.Security;

namespace Nop.Services.Security 
{
    public class UserRegistrationResult 
    {
        public User User { get; set; }
        public IList<string> Errors { get; set; }

        public UserRegistrationResult() 
        {
            this.Errors = new List<string>();
        }

        public bool Success 
        {
            get { return (this.User != null) && (this.Errors.Count == 0); }
        }

        public void AddError(string error) 
        {
            this.Errors.Add(error);
        }
    }
}
