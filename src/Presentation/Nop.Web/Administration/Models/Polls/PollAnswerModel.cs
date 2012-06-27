using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators.Polls;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Polls
{
    [Validator(typeof(PollAnswerValidator))]
    public partial class PollAnswerModel : BaseNopEntityModel
    {
        public int PollId { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Polls.Answers.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Polls.Answers.Fields.NumberOfVotes")]
        public int NumberOfVotes { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Polls.Answers.Fields.DisplayOrder")]
        //we don't name it "DisplayOrder" because Telerik has a small bug 
        //"if we have one more editor with the same name on a page, it doesn't allow editing"
        //in our case it's pollAnswer.DisplayOrder
        public int DisplayOrder1 { get; set; }

    }
}