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
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;



namespace NopSolutions.NopCommerce.BusinessLogic.Content.Blog
{
    /// <summary>
    /// Represents a blog post
    /// </summary>
    public partial class BlogPost : BaseEntity
    {
        #region Ctor
        /// <summary>
        /// Creates a new instance of the BlogPost class
        /// </summary>
        public BlogPost()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the blog post identifier
        /// </summary>
        public int BlogPostId { get; set; }

        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        public int LanguageId { get; set; }

        /// <summary>
        /// Gets or sets the blog post title
        /// </summary>
        public string BlogPostTitle { get; set; }

        /// <summary>
        /// Gets or sets the blog post title
        /// </summary>
        public string BlogPostBody { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the blog post comments are allowed 
        /// </summary>
        public bool BlogPostAllowComments { get; set; }

        /// <summary>
        /// Gets or sets the user identifier who created the blog post
        /// </summary>
        public int CreatedById { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOn { get; set; }

        #endregion 

        #region Custom Properties
        /// <summary>
        /// Gets the language
        /// </summary>
        public Language Language
        {
            get
            {
                return LanguageManager.GetLanguageById(this.LanguageId);
            }
        }

        /// <summary>
        /// Gets the user who created the blog post
        /// </summary>
        public Customer CreatedBy
        {
            get
            {
                return CustomerManager.GetCustomerById(this.CreatedById);
            }
        }

        /// <summary>
        /// Gets the blog comment collection
        /// </summary>
        public List<BlogComment> BlogComments
        {
            get
            {
                return BlogManager.GetBlogCommentsByBlogPostId(this.BlogPostId);
            }
        }
        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the blog comments
        /// </summary>
        public virtual ICollection<BlogComment> NpBlogComments { get; set; }

        /// <summary>
        /// Gets the user who created the blog post
        /// </summary>
        public virtual Customer NpCreatedBy { get; set; }

        /// <summary>
        /// Gets the language
        /// </summary>
        public virtual Language NpLanguage { get; set; }
        
        #endregion
    }

}
