using Nop.Core.Domain.Messages;

namespace Nop.Plugin.Theme.KungFu.Seeding.Email;

public interface IEmailClientSeeds
{
    Task<EmailAccount[]> GetEmailClientForSeedAsync();
}