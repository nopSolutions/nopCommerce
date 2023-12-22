using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Vendors;
using Nop.Services.Attributes;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Vendors;
using Nop.Web.Models.Vendors;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the vendor model factory
/// </summary>
public partial class VendorModelFactory : IVendorModelFactory
{
    #region Fields

    protected readonly CaptchaSettings _captchaSettings;
    protected readonly CommonSettings _commonSettings;
    protected readonly IAttributeParser<VendorAttribute, VendorAttributeValue> _vendorAttributeParser;
    protected readonly IAttributeService<VendorAttribute, VendorAttributeValue> _vendorAttributeService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPictureService _pictureService;
    protected readonly IWorkContext _workContext;
    protected readonly MediaSettings _mediaSettings;
    protected readonly VendorSettings _vendorSettings;

    #endregion

    #region Ctor

    public VendorModelFactory(CaptchaSettings captchaSettings,
        CommonSettings commonSettings,
        IAttributeParser<VendorAttribute, VendorAttributeValue> vendorAttributeParser,
        IAttributeService<VendorAttribute, VendorAttributeValue> vendorAttributeService,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        IPictureService pictureService,
        IWorkContext workContext,
        MediaSettings mediaSettings,
        VendorSettings vendorSettings)
    {
        _captchaSettings = captchaSettings;
        _commonSettings = commonSettings;
        _vendorAttributeParser = vendorAttributeParser;
        _vendorAttributeService = vendorAttributeService;
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _pictureService = pictureService;
        _workContext = workContext;
        _mediaSettings = mediaSettings;
        _vendorSettings = vendorSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare vendor attribute models
    /// </summary>
    /// <param name="vendorAttributesXml">Vendor attributes in XML format</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of the vendor attribute model
    /// </returns>
    protected virtual async Task<IList<VendorAttributeModel>> PrepareVendorAttributesAsync(string vendorAttributesXml)
    {
        var result = new List<VendorAttributeModel>();

        var vendorAttributes = await _vendorAttributeService.GetAllAttributesAsync();
        foreach (var attribute in vendorAttributes)
        {
            var attributeModel = new VendorAttributeModel
            {
                Id = attribute.Id,
                Name = await _localizationService.GetLocalizedAsync(attribute, x => x.Name),
                IsRequired = attribute.IsRequired,
                AttributeControlType = attribute.AttributeControlType,
            };

            if (attribute.ShouldHaveValues)
            {
                //values
                var attributeValues = await _vendorAttributeService.GetAttributeValuesAsync(attribute.Id);
                foreach (var attributeValue in attributeValues)
                {
                    var valueModel = new VendorAttributeValueModel
                    {
                        Id = attributeValue.Id,
                        Name = await _localizationService.GetLocalizedAsync(attributeValue, x => x.Name),
                        IsPreSelected = attributeValue.IsPreSelected
                    };
                    attributeModel.Values.Add(valueModel);
                }
            }

            switch (attribute.AttributeControlType)
            {
                case AttributeControlType.DropdownList:
                case AttributeControlType.RadioList:
                case AttributeControlType.Checkboxes:
                {
                    if (!string.IsNullOrEmpty(vendorAttributesXml))
                    {
                        //clear default selection
                        foreach (var item in attributeModel.Values)
                            item.IsPreSelected = false;

                        //select new values
                        var selectedValues = await _vendorAttributeParser.ParseAttributeValuesAsync(vendorAttributesXml);
                        foreach (var attributeValue in selectedValues)
                        foreach (var item in attributeModel.Values)
                            if (attributeValue.Id == item.Id)
                                item.IsPreSelected = true;
                    }
                }
                    break;
                case AttributeControlType.ReadonlyCheckboxes:
                {
                    //do nothing
                    //values are already pre-set
                }
                    break;
                case AttributeControlType.TextBox:
                case AttributeControlType.MultilineTextbox:
                {
                    if (!string.IsNullOrEmpty(vendorAttributesXml))
                    {
                        var enteredText = _vendorAttributeParser.ParseValues(vendorAttributesXml, attribute.Id);
                        if (enteredText.Any())
                            attributeModel.DefaultValue = enteredText[0];
                    }
                }
                    break;
                case AttributeControlType.ColorSquares:
                case AttributeControlType.ImageSquares:
                case AttributeControlType.Datepicker:
                case AttributeControlType.FileUpload:
                default:
                    //not supported attribute control types
                    break;
            }

            result.Add(attributeModel);
        }

        return result;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the apply vendor model
    /// </summary>
    /// <param name="model">The apply vendor model</param>
    /// <param name="validateVendor">Whether to validate that the customer is already a vendor</param>
    /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
    /// <param name="vendorAttributesXml">Vendor attributes in XML format</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the apply vendor model
    /// </returns>
    public virtual async Task<ApplyVendorModel> PrepareApplyVendorModelAsync(ApplyVendorModel model,
        bool validateVendor, bool excludeProperties, string vendorAttributesXml)
    {
        ArgumentNullException.ThrowIfNull(model);

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (validateVendor && customer.VendorId > 0)
        {
            //already applied for vendor account
            model.DisableFormInput = true;
            model.Result = await _localizationService.GetResourceAsync("Vendors.ApplyAccount.AlreadyApplied");
        }

        model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnApplyVendorPage;
        model.TermsOfServiceEnabled = _vendorSettings.TermsOfServiceEnabled;
        model.TermsOfServicePopup = _commonSettings.PopupForTermsOfServiceLinks;

        if (!excludeProperties)
        {
            model.Email = customer.Email;
        }

        //vendor attributes
        model.VendorAttributes = await PrepareVendorAttributesAsync(vendorAttributesXml);

        return model;
    }

    /// <summary>
    /// Prepare the vendor info model
    /// </summary>
    /// <param name="model">Vendor info model</param>
    /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
    /// <param name="overriddenVendorAttributesXml">Overridden vendor attributes in XML format; pass null to use VendorAttributes of vendor</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor info model
    /// </returns>
    public virtual async Task<VendorInfoModel> PrepareVendorInfoModelAsync(VendorInfoModel model,
        bool excludeProperties, string overriddenVendorAttributesXml = "")
    {
        ArgumentNullException.ThrowIfNull(model);

        var vendor = await _workContext.GetCurrentVendorAsync();
        if (!excludeProperties)
        {
            model.Description = vendor.Description;
            model.Email = vendor.Email;
            model.Name = vendor.Name;
        }

        var picture = await _pictureService.GetPictureByIdAsync(vendor.PictureId);
        var pictureSize = _mediaSettings.AvatarPictureSize;
        (model.PictureUrl, _) = picture != null ? await _pictureService.GetPictureUrlAsync(picture, pictureSize) : (string.Empty, null);

        //vendor attributes
        if (string.IsNullOrEmpty(overriddenVendorAttributesXml))
            overriddenVendorAttributesXml = await _genericAttributeService.GetAttributeAsync<string>(vendor, NopVendorDefaults.VendorAttributes);
        model.VendorAttributes = await PrepareVendorAttributesAsync(overriddenVendorAttributesXml);

        return model;
    }

    #endregion
}