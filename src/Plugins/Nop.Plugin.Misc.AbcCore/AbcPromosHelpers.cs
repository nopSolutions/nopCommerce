using System.Threading.Tasks;
using System.IO;

namespace Nop.Plugin.Misc.AbcCore
{
    public class AbcPromosHelpers
    {
        public static string GetPromoBannersFolder()
        {
            var directoryPath = $"{CoreUtilities.WebRootPath()}/promo_banners";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            return directoryPath;
        }
    }
}