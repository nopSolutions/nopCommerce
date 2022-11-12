<<<<<<< HEAD
﻿using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Blogs;

namespace Nop.Web.Validators.Blogs
{
    public partial class BlogPostValidator : BaseNopValidator<BlogPostModel>
    {
        public BlogPostValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.AddNewComment.CommentText).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Blog.Comments.CommentText.Required")).When(x => x.AddNewComment != null);
        }
    }
=======
﻿using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Blogs;

namespace Nop.Web.Validators.Blogs
{
    public partial class BlogPostValidator : BaseNopValidator<BlogPostModel>
    {
        public BlogPostValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.AddNewComment.CommentText).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Blog.Comments.CommentText.Required")).When(x => x.AddNewComment != null);
        }
    }
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
}