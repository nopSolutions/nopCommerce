using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Models.Profile;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the profile model factory
/// </summary>
public partial class ProfileModelFactory : IProfileModelFactory
{
    #region Fields

    protected readonly CustomerSettings _customerSettings;
    protected readonly ICountryService _countryService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPictureService _pictureService;
    protected readonly IWorkContext _workContext;
    protected readonly MediaSettings _mediaSettings;
    protected readonly PrivateMessageSettings _privateMessageSettings;

    #endregion

    #region Ctor

    public ProfileModelFactory(CustomerSettings customerSettings,
        ICountryService countryService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        IPictureService pictureService,
        IWorkContext workContext,
        MediaSettings mediaSettings,
        PrivateMessageSettings privateMessageSettings)
    {
        _customerSettings = customerSettings;
        _countryService = countryService;
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _pictureService = pictureService;
        _workContext = workContext;
        _mediaSettings = mediaSettings;
        _privateMessageSettings = privateMessageSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the profile index model
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="page">Number of posts page; pass null to disable paging</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the profile index model
    /// </returns>
    public virtual async Task<ProfileIndexModel> PrepareProfileIndexModelAsync(Customer customer)
    {
        ArgumentNullException.ThrowIfNull(customer);

        var name = await _customerService.FormatUsernameAsync(customer);
        var title = string.Format(await _localizationService.GetResourceAsync("Profile.ProfileOf"), name);

        return new ProfileIndexModel
        {
            ProfileTitle = title,
            CustomerProfileId = customer.Id
        };
    }

    /// <summary>
    /// Prepare the profile info model
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the profile info model
    /// </returns>
    public virtual async Task<ProfileInfoModel> PrepareProfileInfoModelAsync(Customer customer)
    {
        ArgumentNullException.ThrowIfNull(customer);

        //avatar
        var avatarUrl = "";
        if (_customerSettings.AllowCustomersToUploadAvatars)
        {
            avatarUrl = await _pictureService.GetPictureUrlAsync(
                await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute),
                _mediaSettings.AvatarPictureSize,
                _customerSettings.DefaultAvatarEnabled,
                defaultPictureType: PictureType.Avatar);
        }

        //location
        var locationEnabled = false;
        var location = string.Empty;
        if (_customerSettings.ShowCustomersLocation)
        {
            locationEnabled = true;

            var countryId = customer.CountryId;
            var country = await _countryService.GetCountryByIdAsync(countryId);
            if (country != null)
            {
                location = await _localizationService.GetLocalizedAsync(country, x => x.Name);
            }
            else
            {
                locationEnabled = false;
            }
        }

        //private message
        var pmEnabled = _privateMessageSettings.AllowPrivateMessages && !await _customerService.IsGuestAsync(customer);

        //registration date
        var joinDateEnabled = false;
        var joinDate = string.Empty;

        if (_customerSettings.ShowCustomersJoinDate)
        {
            joinDateEnabled = true;
            joinDate = (await _dateTimeHelper.ConvertToUserTimeAsync(customer.CreatedOnUtc, DateTimeKind.Utc)).ToString("f");
        }

        //birth date
        var dateOfBirthEnabled = false;
        var dateOfBirth = string.Empty;
        if (_customerSettings.DateOfBirthEnabled)
        {
            if (customer.DateOfBirth.HasValue)
            {
                dateOfBirthEnabled = true;
                dateOfBirth = customer.DateOfBirth.Value.ToString("D");
            }
        }

        var model = new ProfileInfoModel
        {
            CustomerProfileId = customer.Id,
            AvatarUrl = avatarUrl,
            LocationEnabled = locationEnabled,
            Location = location,
            PMEnabled = pmEnabled,
            JoinDateEnabled = joinDateEnabled,
            JoinDate = joinDate,
            DateOfBirthEnabled = dateOfBirthEnabled,
            DateOfBirth = dateOfBirth,
        };

        return model;
    }

    #endregion
}