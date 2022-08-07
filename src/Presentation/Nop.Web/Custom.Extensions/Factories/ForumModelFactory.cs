using Nop.Web.Framework.Extensions;
using System;
using System.Threading.Tasks;

namespace Nop.Web.Factories
{

    public partial interface IForumModelFactory
    {
        Task<string> ConvertDateTimeToHumanString(DateTime dateTime);
    }

    public partial class ForumModelFactory
    {
        public async Task<string> ConvertDateTimeToHumanString(DateTime dateTime)
        {
            var languageCode = (await _workContext.GetWorkingLanguageAsync()).LanguageCulture;
            var loggedInAgo = dateTime.RelativeFormat(languageCode);
            return string.Format(await _localizationService.GetResourceAsync("Common.RelativeDateTime.Past"), loggedInAgo);
        }
    }
}
