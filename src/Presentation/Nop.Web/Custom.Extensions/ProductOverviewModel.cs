using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog
{
    public partial record ProductOverviewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string Company { get; set; }
        public string CountryId { get; set; }
        public string StateProvinceId { get; set; }
        public string LanguageId { get; set; }
        public string TimeZoneId { get; set; }
        public string AvatarPictureId { get; set; }
        public string CustomerProfileTypeId { get; set; }
        public string LastLoginDateTime { get; set; }
        public string PrimaryTechnology { get; set; }
        public string SecondaryTechnology { get; set; }
        public string AvailabilityDays { get; set; }
        public string AvailabilityTimings { get; set; }
        public string CurrentAvalibility { get; set; }
        public string NumberOfHourswork { get; set; }
        public string ProfileType { get; set; }
        public string MobileNumber { get; set; }
        public string Location { get; set; }
        public bool ProfileShortListed { get; set; }
        public bool InterestSent { get; set; }
        public string MotherTounge { get; set; }
        public bool PremiumCustomer { get; set; }
        public string WorkExperience { get; set; }
        public int VendorId { get; set; }

    }
}
