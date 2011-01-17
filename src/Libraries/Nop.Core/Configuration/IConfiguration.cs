
namespace Nop.Core.Configuration
{
    public interface IConfiguration<TSettings> where TSettings : ISettings, new() {
        TSettings Settings { get; }
    }
}
