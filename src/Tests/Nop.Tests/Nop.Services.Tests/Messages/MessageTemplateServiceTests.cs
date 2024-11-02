using FluentAssertions;
using Nop.Core.Domain.Messages;
using Nop.Services.Messages;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Messages;

[TestFixture]
public class MessageTemplateServiceTests : ServiceTest<MessageTemplate>
{
    private IMessageTemplateService _messageTemplateService;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _messageTemplateService = GetService<IMessageTemplateService>();
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
        var initMessage = new MessageTemplate { Body = "Test body", Name = "Test template" };

        var message = await _messageTemplateService.CopyMessageTemplateAsync(initMessage);
        message.Should().NotBeNull();
        message.Should().NotBeEquivalentTo(initMessage);
        message.Body.Should().BeEquivalentTo(initMessage.Body);
        message.Name.Should().BeEquivalentTo(initMessage.Name);
    }

    protected override CrudData<MessageTemplate> CrudData
    {
        get
        {
            var basTemplate = new MessageTemplate { Body = "Test body", Name = "Test template" };
            var updatedTemplate = new MessageTemplate { Body = "Test body", Name = "Test template", Subject = "Test subject" };

            return new CrudData<MessageTemplate>
            {
                BaseEntity = basTemplate,
                UpdatedEntity = updatedTemplate,
                Insert = _messageTemplateService.InsertMessageTemplateAsync,
                Update = _messageTemplateService.UpdateMessageTemplateAsync,
                Delete = _messageTemplateService.DeleteMessageTemplateAsync,
                GetById = _messageTemplateService.GetMessageTemplateByIdAsync,
                IsEqual = (first, second) => first.Body.Equals(second.Body) && first.Name.Equals(second.Name) && first.Subject.Equals(second.Subject)
            };
        }
    }
}