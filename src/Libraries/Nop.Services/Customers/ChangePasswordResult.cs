using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Customers 
{
    /// <summary>
    /// Change password result
    /// </summary>
    public class ChangePasswordResult
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ChangePasswordResult() 
        {
            this.Errors = new List<string>();
        }

        /// <summary>
        /// Gets a value indicating whether request has been completed successfully
        /// </summary>
        public bool Success
        {
            get { return (!this.Errors.Any()); }
        }

        /// <summary>
        /// Add error
        /// </summary>
        /// <param name="error">Error</param>
        public void AddError(string error) 
        {
            this.Errors.Add(error);
        }

        /// <summary>
        /// Errors
        /// </summary>
        public IList<string> Errors { get; set; }
    }
}
