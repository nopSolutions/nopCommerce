using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Caching;
using Nop.Core.Domain;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Security;
using Nop.Tests;
using NUnit.Framework;
using Nop.Core;
using Nop.Services.Localization;
using Nop.Core.Domain.Localization;
using Rhino.Mocks;

namespace Nop.Services.Tests.Helpers
{
    [TestFixture]
    public class DateTimeHelperTests
    {
        IWorkContext _workContext;
        ICustomerService _customerService;
        DateTimeSettings _dateTimeSettings;
        IDateTimeHelper _dateTimeHelper;

        [SetUp]
        public void SetUp()
        {
            _customerService = MockRepository.GenerateMock<ICustomerService>();

            //var customer = new Customer()
            //{
            //    //
            //};
            //_workContext = MockRepository.GenerateMock<IWorkContext>();
            //_workContext.Expect(x => x.CurrentCustomer).Return(customer);

            _dateTimeSettings = new DateTimeSettings()
            {
                AllowCustomersToSetTimeZone = false,
                DefaultStoreTimeZoneId = ""
            };

            _dateTimeHelper = new DateTimeHelper(_workContext, _customerService, _dateTimeSettings);
        }

        [Test]
        public void Can_find_systemTimeZone_by_id()
        {
            var timeZones = _dateTimeHelper.FindTimeZoneById("E. Europe Standard Time");
            timeZones.ShouldNotBeNull();
            timeZones.Id.ShouldEqual("E. Europe Standard Time");
        }

        [Test]
        public void Can_get_all_systemTimeZones()
        {
            var systemTimeZones = _dateTimeHelper.GetSystemTimeZones();
            systemTimeZones.ShouldNotBeNull();
            (systemTimeZones.Count > 0).ShouldBeTrue();
        }

        [Test]
        public void Can_get_customer_timeZone_with_customTimeZones_enabled()
        {
            _dateTimeSettings.AllowCustomersToSetTimeZone = true;
            _dateTimeSettings.DefaultStoreTimeZoneId = "E. Europe Standard Time"; //(GMT+02:00) Minsk;

            var customer = new Customer()
            {
                TimeZoneId = "Russian Standard Time" //(GMT+03:00) Moscow, St. Petersburg, Volgograd
            };
            var timeZone = _dateTimeHelper.GetCustomerTimeZone(customer);
            timeZone.ShouldNotBeNull();
            timeZone.Id.ShouldEqual("Russian Standard Time");
        }

        [Test]
        public void Can_get_customer_timeZone_with_customTimeZones_disabled()
        {
            _dateTimeSettings.AllowCustomersToSetTimeZone = false;
            _dateTimeSettings.DefaultStoreTimeZoneId = "E. Europe Standard Time"; //(GMT+02:00) Minsk;

            var customer = new Customer()
            {
                TimeZoneId = "Russian Standard Time" //(GMT+03:00) Moscow, St. Petersburg, Volgograd
            };
            var timeZone = _dateTimeHelper.GetCustomerTimeZone(customer);
            timeZone.ShouldNotBeNull();
            timeZone.Id.ShouldEqual("E. Europe Standard Time");
        }

        [Test]
        public void Can_convert_dateTime_to_userTime()
        {
            var sourceDateTime = TimeZoneInfo.FindSystemTimeZoneById("E. Europe Standard Time"); //(GMT+02:00) Minsk;
            sourceDateTime.ShouldNotBeNull();
            var destinationDateTime = TimeZoneInfo.FindSystemTimeZoneById("North Asia Standard Time"); //(GMT+07:00) Krasnoyarsk;
            destinationDateTime.ShouldNotBeNull();

            //summer time
            var dateTime1 = new DateTime(2010, 06, 01, 0, 0, 0);
            var convertedDateTime1 = _dateTimeHelper.ConvertToUserTime(dateTime1, sourceDateTime, destinationDateTime);
            convertedDateTime1.ShouldEqual(new DateTime(2010, 06, 01, 5, 0, 0));

            //winter time
            var dateTime2 = new DateTime(2010, 01, 01, 0, 0, 0);
            var convertedDateTime2 = _dateTimeHelper.ConvertToUserTime(dateTime2, sourceDateTime, destinationDateTime);
            convertedDateTime2.ShouldEqual(new DateTime(2010, 01, 01, 5, 0, 0));
        }

        [Test]
        public void Can_convert_dateTime_to_utc_dateTime()
        {
            var sourceDateTime = TimeZoneInfo.FindSystemTimeZoneById("E. Europe Standard Time"); //(GMT+02:00) Minsk;
            sourceDateTime.ShouldNotBeNull();

            //summer time
            var dateTime1 = new DateTime(2010, 06, 01, 0, 0, 0);
            var convertedDateTime1 = _dateTimeHelper.ConvertToUtcTime(dateTime1, sourceDateTime);
            convertedDateTime1.ShouldEqual(new DateTime(2010, 05, 31, 21, 0, 0));

            //winter time
            var dateTime2 = new DateTime(2010, 01, 01, 0, 0, 0);
            var convertedDateTime2 = _dateTimeHelper.ConvertToUtcTime(dateTime2, sourceDateTime);
            convertedDateTime2.ShouldEqual(new DateTime(2009, 12, 31, 22, 0, 0));
        }
    }
}
