using FluentAssertions;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Attributes;
using Nop.Services.Configuration;
using Nop.Web.Factories;
using Nop.Web.Models.Customer;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Factories;

[TestFixture]
public class CustomerModelFactoryTests : WebTest
{
    private IAttributeService<CustomerAttribute, CustomerAttributeValue> _customerAttributeService;
    private ICustomerModelFactory _customerModelFactory;
    private Customer _customer;
    private CustomerAttribute[] _customerAttributes;
    private CustomerSettings _customerSettings;
    private ISettingService _settingService;

    [OneTimeSetUp]
    public async Task SetUp()
    {
        _customerSettings = GetService<CustomerSettings>();
        _settingService = GetService<ISettingService>();

        _customerSettings.CountryEnabled = true;
        _customerSettings.StateProvinceEnabled = true;
        await _settingService.SaveSettingAsync(_customerSettings);

        _customerAttributeService = GetService<IAttributeService<CustomerAttribute, CustomerAttributeValue>>();
        _customerModelFactory = GetService<ICustomerModelFactory>();
        _customer = await GetService<IWorkContext>().GetCurrentCustomerAsync();
        
        _customerAttributes =
        [
            new CustomerAttribute
        {
            AttributeControlType = AttributeControlType.Checkboxes, Name = "Test customer attribute 1"
        },
        new CustomerAttribute
        {
            AttributeControlType = AttributeControlType.ColorSquares, Name = "Test customer attribute 2"
        },
        new CustomerAttribute
        {
            AttributeControlType = AttributeControlType.Datepicker, Name = "Test customer attribute 3"
        },
        new CustomerAttribute
        {
            AttributeControlType = AttributeControlType.DropdownList, Name = "Test customer attribute 4"
        },
        new CustomerAttribute
        {
            AttributeControlType = AttributeControlType.FileUpload, Name = "Test customer attribute 5"
        },
        new CustomerAttribute
        {
            AttributeControlType = AttributeControlType.ImageSquares, Name = "Test customer attribute 6"
        },
        new CustomerAttribute
        {
            AttributeControlType = AttributeControlType.MultilineTextbox, Name = "Test customer attribute 7"
        },
        new CustomerAttribute
        {
            AttributeControlType = AttributeControlType.RadioList, Name = "Test customer attribute 8"
        },
        new CustomerAttribute
        {
            AttributeControlType = AttributeControlType.ReadonlyCheckboxes, Name = "Test customer attribute 9"
        },
        new CustomerAttribute
            {
                AttributeControlType = AttributeControlType.TextBox, Name = "Test customer attribute 10"
            }
            ];

        foreach (var customerAttribute in _customerAttributes)
            await _customerAttributeService.InsertAttributeAsync(customerAttribute);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        _customerSettings.CountryEnabled = false;
        _customerSettings.StateProvinceEnabled = false;
        await _settingService.SaveSettingAsync(_customerSettings);

        foreach (var customerAttribute in _customerAttributes)
            await _customerAttributeService.DeleteAttributeAsync(customerAttribute);
    }

    [Test]
    public async Task CanPrepareCustomerInfoModel()
    {
        var model = await _customerModelFactory.PrepareCustomerInfoModelAsync(new CustomerInfoModel(), _customer, false);
        model.AvailableTimeZones.Any().Should().BeTrue();

        model.Email.Should().Be(NopTestsDefaults.AdminEmail);
        model.Username.Should().Be(NopTestsDefaults.AdminEmail);
        model.FirstName.Should().Be("John");
        model.LastName.Should().Be("Smith");

        model = await _customerModelFactory.PrepareCustomerInfoModelAsync(new CustomerInfoModel(), _customer, true);

        model.Email.Should().BeNullOrEmpty();
        model.Username.Should().BeNullOrEmpty();
        model.FirstName.Should().BeNullOrEmpty();
        model.LastName.Should().BeNullOrEmpty();
    }

    [Test]
    public async Task CanPrepareRegisterModel()
    {
        var model = await _customerModelFactory.PrepareRegisterModelAsync(new RegisterModel(), false);
        model.AvailableTimeZones.Any().Should().BeTrue();
        model.CustomerAttributes.Any().Should().BeTrue();
    }

    [Test]
    public async Task CanPrepareLoginModel()
    {
        var model = await _customerModelFactory.PrepareLoginModelAsync(null);
        model.CheckoutAsGuest.Should().Be(default);
        model = await _customerModelFactory.PrepareLoginModelAsync(true);
        model.CheckoutAsGuest.Should().BeTrue();
        model = await _customerModelFactory.PrepareLoginModelAsync(false);
        model.CheckoutAsGuest.Should().BeFalse();
    }

    [Test]
    public async Task CanPreparePasswordRecoveryModel()
    {
        var model = await _customerModelFactory.PreparePasswordRecoveryModelAsync(new PasswordRecoveryModel { Email = "test@email.com" });
        model.DisplayCaptcha.Should().BeFalse();
        model.Email.Should().Be("test@email.com");
    }

    [Test]
    public async Task CanPrepareRegisterResultModel()
    {
        var model = await _customerModelFactory.PrepareRegisterResultModelAsync((int)UserRegistrationType.AdminApproval, string.Empty);
        model.Result.Should().Be("Your account will be activated after approving by administrator.");
        model = await _customerModelFactory.PrepareRegisterResultModelAsync((int)UserRegistrationType.Disabled, string.Empty);
        model.Result.Should().Be("Registration not allowed. You can edit this in the admin area.");
        model = await _customerModelFactory.PrepareRegisterResultModelAsync((int)UserRegistrationType.EmailValidation, string.Empty);
        model.Result.Should().Be("Your registration has been successfully completed. You have just been sent an email containing activation instructions.");
        model = await _customerModelFactory.PrepareRegisterResultModelAsync((int)UserRegistrationType.Standard, string.Empty);
        model.Result.Should().Be("Your registration completed");
        model = await _customerModelFactory.PrepareRegisterResultModelAsync(400, string.Empty);
        model.Result.Should().BeNullOrEmpty();
    }

    [Test]
    public async Task CanPrepareCustomCustomerAttributes()
    {
        var model = await _customerModelFactory.PrepareCustomCustomerAttributesAsync(_customer);
        model.Any().Should().BeTrue();
        model.Count.Should().Be(10);
    }
}