using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Profile
{
    public partial record ProfileInfoModel : BaseNopModel
    {
        public int CustomerProfileId { get; set; }

        public string AvatarUrl { get; set; }

        public bool LocationEnabled { get; set; }
        public string Location { get; set; }

        public bool PMEnabled { get; set; }

        public bool TotalPostsEnabled { get; set; }
        public string TotalPosts { get; set; }

        public bool JoinDateEnabled { get; set; }
        public string JoinDate { get; set; }

        public bool DateOfBirthEnabled { get; set; }
        public string DateOfBirth { get; set; }
    }
}