using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog
{
    public partial record ProductDetailsModel
    {
        public string PrimaryTechnology { get; set; }
        public string SecondaryTechnology { get; set; }
        public string CurrentAvalibility { get; set; }
        public string RelaventExperiance { get; set; }
        public string MotherTongue { get; set; }

        public string AvailabilityDays { get; set; }
        public string AvailabilityTimings { get; set; }
        public string NumberOfHourswork { get; set; }

        public string ProfileType { get; set; }
        public string MobileNumber { get; set; }
        public bool ProfileShortListed { get; set; }

        public bool InterestSent { get; set; }
        public string LastLoginDateTime { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Location { get; internal set; }
        public string EmailId { get; set; }
        public string Gender { get; set; }


        public partial record AddToCartModel : BaseNopModel
        {
            public bool ProfileShortListed { get; internal set; }
            public string ProfileShortListedOn { get; internal set; }
            public bool InterestSent { get; internal set; }
        }
    }
}
