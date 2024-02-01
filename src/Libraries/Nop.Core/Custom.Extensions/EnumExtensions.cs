using System.ComponentModel;

namespace Nop.Core.Domain.Catalog
{
    public enum ProductAndCustomerAttributeEnum
    {
        ProfileType = 1,

        [Description("Current Avalibility")]
        CurrentAvalibility = 2,

        [Description("Relavent Experiance")]
        RelaventExperiance = 3,

        [Description("Mother Tongue")]
        MotherTongue = 4,

        ShortDescription = 5,

        FullDescription = 6,

        [Description("Primary Technology")]
        PrimaryTechnology = 7,

        [Description("Secondary Technology")]
        SecondaryTechnology = 8,

        Gender = 9,

    }

    public enum CustomerProfileTypeEnum
    {
        GiveSupport = 1,

        TakeSupport = 2
    }
}