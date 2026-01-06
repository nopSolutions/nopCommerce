using Nop.Core.Domain.Messages;
using Nop.Core.Infrastructure;
using Nop.Plugin.Theme.KungFu.Infrastructure;
using Nop.Plugin.Theme.KungFu.Seeding.Category;

namespace Nop.Plugin.Theme.KungFu.Seeding.Email;

public class EmailClientSeeds(INopFileProvider fileProvider) : IEmailClientSeeds
{
    private const string MAIL_CLIENT_SEED_FILE_NAME = "EmailClient.json";

    public async Task<EmailAccount[]> GetEmailClientForSeedAsync()
    {
        var json = ThemeKungFuDefaults.MailClientSeedingVirtualPath + "/" + MAIL_CLIENT_SEED_FILE_NAME;
        var filePath = fileProvider.MapPath(json);

        if (!fileProvider.FileExists(filePath))
            return [];
        
        var fileContent = await fileProvider.ReadAllTextAsync(filePath, System.Text.Encoding.UTF8);
        var emailAccounts = System.Text.Json.JsonSerializer.Deserialize<EmailAccount[]>(fileContent, new System.Text.Json.JsonSerializerOptions
        {
            Converters = { new JsonBooleanConverter() }
        });
        return emailAccounts ?? [];
    }
}