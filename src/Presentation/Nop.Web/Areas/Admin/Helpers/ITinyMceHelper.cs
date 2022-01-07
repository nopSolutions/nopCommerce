using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Helpers
{
    public interface ITinyMceHelper
    {
        Task<string> GetTinyMceLanguageAsync();
    }
}