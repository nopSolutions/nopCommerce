using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Nop.Core.Domain.Messages;
using Nop.Services.Messages;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Messages
{
    [TestFixture]
    public class MessageTemplateServiceTests : BaseNopTest
    {
        private IMessageTemplateService _messageTemplateService;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _messageTemplateService = GetService<IMessageTemplateService>();
        }

        [Test]
        public async Task CanCRUD()
        {
            var message = new MessageTemplate { Body = "Test body", Name = "Test template" };
            await _messageTemplateService.InsertMessageTemplateAsync(message);
            message.Id.Should().BeGreaterThan(0);
            message = await _messageTemplateService.GetMessageTemplateByIdAsync(message.Id);
            message.Should().NotBeNull();
            var namedMessage = (await _messageTemplateService.GetMessageTemplatesByNameAsync(message.Name)).FirstOrDefault(t=>t.Id == message.Id);
            namedMessage.Should().NotBeNull();
            message.Subject = "Test subject";
            await _messageTemplateService.UpdateMessageTemplateAsync(message);
            namedMessage = (await _messageTemplateService.GetMessageTemplatesByNameAsync(message.Name)).FirstOrDefault(t => t.Id == message.Id);
            namedMessage.Should().NotBeNull();
            namedMessage.Subject.Should().Be(message.Subject);
            await _messageTemplateService.DeleteMessageTemplateAsync(namedMessage);
            message = await _messageTemplateService.GetMessageTemplateByIdAsync(message.Id);
            message.Should().BeNull();
        }

        [Test]
        public async Task CanGetAllMessageTemplates()
        {
            var templates = await _messageTemplateService.GetAllMessageTemplatesAsync(1);
            templates.Count.Should().BeGreaterThan(0);
            templates = await _messageTemplateService.GetAllMessageTemplatesAsync(0, "NOT_EXISTS_ONE");
            templates.Count.Should().Be(0);
            templates = await _messageTemplateService.GetAllMessageTemplatesAsync(0, "Email validation");
            templates.Count.Should().Be(2);
        }

        [Test]
        public async Task CanCopyMessageTemplate()
        {
            var initMessage = new MessageTemplate {Body = "Test body", Name = "Test template"};

            var message = await _messageTemplateService.CopyMessageTemplateAsync(initMessage);
            message.Should().NotBeNull();
            message.Should().NotBeEquivalentTo(initMessage);
            message.Body.Should().BeEquivalentTo(initMessage.Body);
            message.Name.Should().BeEquivalentTo(initMessage.Name);
        }

    }
}
