using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Services.Logging;
using Nop.Services.Messages;
using NUnit.Framework;

namespace Nop.Services.Tests.Messages
{
    [TestFixture]
    class NotificationServiceTests : ServiceTest
    {
        private INotificationService _notificationService;

        private Mock<IHttpContextAccessor> _httpContextAccessor;
        private Mock<ILogger> _logger;
        private Mock<ITempDataDictionaryFactory> _tempDataDictionaryFactory;
        private Mock<IWorkContext> _workContext;

        private ITempDataDictionary _dataDictionary;


        [SetUp]
        public new void SetUp()
        {
            var httpContext = new Mock<HttpContext>();
            var tempDataProvider = new Mock<ITempDataProvider>();

            _dataDictionary = new TempDataDictionary(httpContext.Object, tempDataProvider.Object);

            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _logger = new Mock<ILogger>();
            _tempDataDictionaryFactory = new Mock<ITempDataDictionaryFactory>();
            _tempDataDictionaryFactory.Setup(f => f.GetTempData(It.IsAny<HttpContext>())).Returns(_dataDictionary);
            _workContext = new Mock<IWorkContext>();

            _notificationService = new NotificationService(
                httpContextAccessor: _httpContextAccessor.Object,
                logger: _logger.Object,
                tempDataDictionaryFactory: _tempDataDictionaryFactory.Object,
                workContext: _workContext.Object);
        }

        private IList<NotifyData> DeserializedDataDictionary => 
            JsonConvert.DeserializeObject<IList<NotifyData>>(_dataDictionary[NopMessageDefaults.NotificationListKey].ToString());

        [Test]
        public void Can_add_notification()
        {
            _notificationService.SuccessNotification("success");
            _notificationService.WarningNotification("warning");
            _notificationService.ErrorNotification("error");
            var messageList = DeserializedDataDictionary;

            messageList.Count.Should().Be(3);
            var succMsg = messageList
                .Where(m => m.Type == NotifyType.Success)
                .First();
            succMsg.Message.Should().Be("success");

            var warnMsg = messageList
                .Where(m => m.Type == NotifyType.Warning)
                .First();
            warnMsg.Message.Should().Be("warning");

            var errMsg = messageList
                .Where(m => m.Type == NotifyType.Error)
                .First();
            errMsg.Message.Should().Be("error");
        }

        [Test]
        public void Can_add_notification_from_exception()
        {
            _notificationService.ErrorNotification(new Exception("error"));
            var msg = DeserializedDataDictionary.First();
            msg.Message.Should().Be("error");
        }
    }
}
