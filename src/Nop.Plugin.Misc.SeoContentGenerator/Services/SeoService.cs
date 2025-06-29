using System.Threading.Tasks;

namespace Nop.Plugin.Misc.SeoContentGenerator.Services
{
    public interface ISeoService
    {
        Task<string> GenerateSeoContentAsync(string input);
    }

    public class SeoService : ISeoService
    {
        public async Task<string> GenerateSeoContentAsync(string input)
        {
            // Later: Call Clearscope or Frase.io API here.
            return await Task.FromResult($"[SEO Optimized] {input}");
        }
    }
}
