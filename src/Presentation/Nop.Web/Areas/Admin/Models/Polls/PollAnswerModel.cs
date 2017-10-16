using FluentValidation.Attributes;
using Nop.Web.Areas.Admin.Validators.Polls;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Polls
{
    [Validator(typeof(PollAnswerValidator))]
    public partial class PollAnswerModel : BaseNopEntityModel
    {
        public int PollId { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Polls.Answers.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Polls.Answers.Fields.NumberOfVotes")]
        public int NumberOfVotes { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Polls.Answers.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}