using System;
using System.Linq;
using FluentAssertions;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Helpers
{
    [TestFixture]
    public class DateTimeHelperTests : ServiceTest
    {
        private IGenericAttributeService _genericAttributeService;
        private DateTimeSettings _dateTimeSettings;
        private IDateTimeHelper _dateTimeHelper;
        private ISettingService _settingService;
        private Customer _customer;

        /// <summary>
        /// (GMT+02:00) Minsk
        /// </summary>
        private string _gmtPlus2MinskTimeZoneId;
       
        /// <summary>
        /// (GMT+03:00) Moscow, St. Petersburg, Volgograd
        /// </summary>
        private string _gmtPlus3MoscowTimeZoneId;

        /// <summary>
        /// (GMT+07:00) Krasnoyarsk
        /// </summary>
        private string _gmtPlus7KrasnoyarskTimeZoneId;

        private string _defaultTimeZone;
        private bool _defaultAllowCustomersToSetTimeZone;
        private string _defaultDefaultStoreTimeZoneId;

        [SetUp]
        public void SetUp()
        {
            _genericAttributeService = GetService<IGenericAttributeService>();
            _dateTimeSettings = GetService<DateTimeSettings>();
            _dateTimeHelper = GetService<IDateTimeHelper>();
            _settingService = GetService<ISettingService>();

            _customer = GetService<ICustomerService>().GetCustomerByEmail(NopTestsDefaults.AdminEmail);

            _defaultTimeZone = _genericAttributeService.GetAttribute<string>(_customer, NopCustomerDefaults.TimeZoneIdAttribute);

            _defaultAllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
            _defaultDefaultStoreTimeZoneId = _dateTimeSettings.DefaultStoreTimeZoneId;

            _gmtPlus2MinskTimeZoneId = "E. Europe Standard Time";  //(GMT+02:00) Minsk
            _gmtPlus3MoscowTimeZoneId = "Russian Standard Time"; //(GMT+03:00) Moscow, St. Petersburg, Volgograd
            _gmtPlus7KrasnoyarskTimeZoneId = "North Asia Standard Time"; //(GMT+07:00) Krasnoyarsk;

            if (Environment.OSVersion.Platform != PlatformID.Unix) 
                return;

            _gmtPlus2MinskTimeZoneId = "Europe/Minsk";  //(GMT+02:00) Minsk;
            _gmtPlus3MoscowTimeZoneId = "Europe/Moscow"; //(GMT+03:00) Moscow, St. Petersburg, Volgograd
            _gmtPlus7KrasnoyarskTimeZoneId = "Asia/Krasnoyarsk"; //(GMT+07:00) Krasnoyarsk;
        }

        [TearDown]
        public void TearDown()
        {
            _genericAttributeService.SaveAttribute(_customer, NopCustomerDefaults.TimeZoneIdAttribute, _defaultTimeZone);

            _dateTimeSettings.AllowCustomersToSetTimeZone = _defaultAllowCustomersToSetTimeZone;
            _dateTimeSettings.DefaultStoreTimeZoneId = _defaultDefaultStoreTimeZoneId;

            _settingService.SaveSetting(_dateTimeSettings);
        }

        [Test]
        public void CanFindSystemTimeZoneById()
        {
            var timeZones = _dateTimeHelper.FindTimeZoneById(_gmtPlus2MinskTimeZoneId);  //(GMT+02:00) Minsk
            timeZones.Should().NotBeNull();
            timeZones.Id.Should().Be(_gmtPlus2MinskTimeZoneId);
        }

        [Test]
        public void CanGetAllSystemTimeZones()
        {
            var systemTimeZones = _dateTimeHelper.GetSystemTimeZones();
            systemTimeZones.Should().NotBeNull();
            systemTimeZones.Any().Should().BeTrue();
        }

        [Test]
        public void CanGetCustomerTimeZoneWithCustomTimeZonesEnabled()
        {
            _dateTimeSettings.AllowCustomersToSetTimeZone = true;
            _dateTimeSettings.DefaultStoreTimeZoneId = _gmtPlus2MinskTimeZoneId; //(GMT+02:00) Minsk;
            _settingService.SaveSetting(_dateTimeSettings);

            _genericAttributeService.SaveAttribute(_customer, NopCustomerDefaults.TimeZoneIdAttribute, _gmtPlus3MoscowTimeZoneId);

            var timeZone = GetService<IDateTimeHelper>().GetCustomerTimeZone(_customer);
            timeZone.Should().NotBeNull();
            timeZone.Id.Should().Be(_gmtPlus3MoscowTimeZoneId);
        }

        [Test]
        public void CanGetCustomerTimezoneWithCustomTimeZonesDisabled()
        {
            _dateTimeSettings.AllowCustomersToSetTimeZone = false;
            _dateTimeSettings.DefaultStoreTimeZoneId = _gmtPlus2MinskTimeZoneId; //(GMT+02:00) Minsk;
            _settingService.SaveSetting(_dateTimeSettings);

            _genericAttributeService.SaveAttribute(_customer, NopCustomerDefaults.TimeZoneIdAttribute, _gmtPlus3MoscowTimeZoneId);
            
            var timeZone = GetService<IDateTimeHelper>().GetCustomerTimeZone(_customer);
            timeZone.Should().NotBeNull();
            timeZone.Id.Should().Be(_gmtPlus2MinskTimeZoneId);  //(GMT+02:00) Minsk
        }

        [Test]
        public void CanConvertDatetimeToUserTime()
        {
            var sourceDateTime = TimeZoneInfo.FindSystemTimeZoneById(_gmtPlus2MinskTimeZoneId); //(GMT+02:00) Minsk;
            sourceDateTime.Should().NotBeNull();

            var destinationDateTime = TimeZoneInfo.FindSystemTimeZoneById(_gmtPlus7KrasnoyarskTimeZoneId); //(GMT+07:00) Krasnoyarsk;
            destinationDateTime.Should().NotBeNull();

            //summer time
            _dateTimeHelper.ConvertToUserTime(new DateTime(2010, 06, 01, 0, 0, 0), sourceDateTime, destinationDateTime)
                .Should().Be(new DateTime(2010, 06, 01, 5, 0, 0));

            //winter time
            _dateTimeHelper.ConvertToUserTime(new DateTime(2010, 01, 01, 0, 0, 0), sourceDateTime, destinationDateTime)
                .Should().Be(new DateTime(2010, 01, 01, 5, 0, 0));
        }

        [Test]
        public void CanConvertDatetimeToUtcDateTime()
        {
            var sourceDateTime = TimeZoneInfo.FindSystemTimeZoneById(_gmtPlus2MinskTimeZoneId); //(GMT+02:00) Minsk;
            sourceDateTime.Should().NotBeNull();

            //summer time
            var dateTime1 = new DateTime(2010, 06, 01, 0, 0, 0);
            var convertedDateTime1 = _dateTimeHelper.ConvertToUtcTime(dateTime1, sourceDateTime);
            convertedDateTime1.Should().Be(new DateTime(2010, 05, 31, 21, 0, 0));

            //winter time
            var dateTime2 = new DateTime(2010, 01, 01, 0, 0, 0);
            var convertedDateTime2 = _dateTimeHelper.ConvertToUtcTime(dateTime2, sourceDateTime);
            convertedDateTime2.Should().Be(new DateTime(2009, 12, 31, 22, 0, 0));
        }
    }
}
