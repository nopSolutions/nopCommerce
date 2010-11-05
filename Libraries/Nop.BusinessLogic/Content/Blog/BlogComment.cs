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
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.IoC;


namespace NopSolutions.NopCommerce.BusinessLogic.Content.Blog
{
    /// <summary>
    /// Represents a blog comment
    /// </summary>
    public partial class BlogComment : BaseEntity
    {
        #region Ctor
        /// <summary>
        /// Creates a new instance of the BlogComment class
        /// </summary>
        public BlogComment()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the blog comment identifier
        /// </summary>
        public int BlogCommentId { get; set; }

        /// <summary>
        /// Gets or sets the blog post identifier
        /// </summary>
        public int BlogPostId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier who commented the blog post
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the IP address
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Gets or sets the comment text
        /// </summary>
        public string CommentText { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOn { get; set; }

        #endregion

        #region Custom Properties

        /// <summary>
        /// Gets the customer who commented the blog post
        /// </summary>
        public Customer Customer
        {
            get
            {
                return IoCFactory.Resolve<ICustomerService>().GetCustomerById(this.CustomerId);
            }
        }

        /// <summary>
        /// Gets the blog comment collection
        /// </summary>
        public BlogPost BlogPost
        {
            get
            {
                return IoCFactory.Resolve<IBlogService>().GetBlogPostById(this.BlogPostId);
            }
        }
        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the blog post
        /// </summary>
        public virtual BlogPost NpBlogPost { get; set; }

        #endregion
    }

}
