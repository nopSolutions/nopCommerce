using Nop.Core.Domain.Messages;
using Nop.Services.Messages;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Messages;

[TestFixture]
public class EmailAccountServiceTests : ServiceTest<EmailAccount>
{
    private IEmailAccountService _emailAccountService;

    [OneTimeSetUp]
    public void SetUp()
    {
        _emailAccountService = GetService<IEmailAccountService>();
    }

    protected override CrudData<EmailAccount> CrudData
    {
        get
        {
            var insertItem = new EmailAccount
            {
                Email = "test@test.com",
                DisplayName = "Test name",
                Host = "smtp.test.com",
                Port = 25,
                Username = "test_user",
                Password = "test_password",
                EnableSsl = false
            };

            var updateItem = new EmailAccount
            {
                Email = "test@test.com",
                DisplayName = "Test name",
                Host = "smtp.test.com",
                Port = 430,
                Username = "test_user",
                Password = "test_password",
                EnableSsl = true
            };

            return new CrudData<EmailAccount>
            {
                BaseEntity = insertItem,
                UpdatedEntity = updateItem,
                Insert = _emailAccountService.InsertEmailAccountAsync,
                Update = _emailAccountService.UpdateEmailAccountAsync,
                GetById = _emailAccountService.GetEmailAccountByIdAsync,
                IsEqual = (item, other) => item.Port.Equals(other.Port) && item.EnableSsl.Equals(other.EnableSsl),
                Delete = _emailAccountService.DeleteEmailAccountAsync
            };
        }
    }

}