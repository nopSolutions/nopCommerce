using System;
using Nop.Core.Domain.Localization;

namespace Nop.Web.Framework.Localization
{

    public static class LocalizedUrlExtenstions
    {
        private static int _seoCodeLength = 2;
        
        private static bool IsVirtualDirectory(this string applicationPath)
        {
            if (string.IsNullOrEmpty(applicationPath))
                throw new ArgumentException("Application path is not specified");

            return applicationPath != "/";
        }

        public static string RemoveApplicationPathFromRawUrl(this string rawUrl, string applicationPath)
        {
            if (string.IsNullOrEmpty(applicationPath))
                throw new ArgumentException("Application path is not specified");

            if (rawUrl.Length == applicationPath.Length)
                return "/";

            
            var result = rawUrl.Substring(applicationPath.Length);
            //raw url always starts with '/'
            if (!result.StartsWith("/"))
                result = "/" + result;
            return result;
        }

        public static string GetLanguageSeoCodeFromUrl(this string url, string applicationPath, bool isRawPath)
        {
            if (isRawPath)
            {
                if (applicationPath.IsVirtualDirectory())
                {
                    //we're in virtual directory. So remove its path
                    url = url.RemoveApplicationPathFromRawUrl(applicationPath);
                }

                return url.Substring(1, _seoCodeLength);
            }
            else
            {
                return url.Substring(2, _seoCodeLength);
            }
        }

        public static bool IsLocalizedUrl(this string url, string applicationPath, bool isRawPath)
        {
            if (string.IsNullOrEmpty(url))
                return false;
            if (isRawPath)
            {
                if (applicationPath.IsVirtualDirectory())
                {
                    //we're in virtual directory. So remove its path
                    url = url.RemoveApplicationPathFromRawUrl(applicationPath);
                }

                int length = url.Length;
                //too short url
                if (length < 1 + _seoCodeLength)
                    return false;

                //url like "/en"
                if (length == 1 + _seoCodeLength)
                    return true;

                //urls like "/en/" or "/en/somethingelse"
                return (length > 1 + _seoCodeLength) && (url[1 + _seoCodeLength] == '/');
            }
            else
            {
                int length = url.Length;
                //too short url
                if (length < 2 + _seoCodeLength)
                    return false;

                //url like "/en"
                if (length == 2 + _seoCodeLength)
                    return true;

                //urls like "/en/" or "/en/somethingelse"
                return (length > 2 + _seoCodeLength) && (url[2 + _seoCodeLength] == '/');
            }
        }

        public static string RemoveLocalizedPathFromRawUrl(this string url, string applicationPath)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            string result = null;
            if (applicationPath.IsVirtualDirectory())
            {
                //we're in virtual directory. So remove its path
                url = url.RemoveApplicationPathFromRawUrl(applicationPath);
            }

            int length = url.Length;
            if (length < _seoCodeLength + 1)    //too short url
                result = url;
            else if (length == 1 + _seoCodeLength)  //url like "/en"
                result = url.Substring(0, 1);
            else
                result = url.Substring(_seoCodeLength + 1); //urls like "/en/" or "/en/somethingelse"

            if (applicationPath.IsVirtualDirectory())
                result = applicationPath + result;  //add back applciation path
            return result;
        }

        public static string AddLocalizedPathToRawUrl(this string url, string applicationPath,
            Language language)
        {
            if (language == null)
                throw new ArgumentNullException("language");

            //null validation is not required
            //if (string.IsNullOrEmpty(url))
            //    return url;


            int startIndex = 0;
            if (applicationPath.IsVirtualDirectory())
            {
                //we're in virtual directory.
                startIndex = applicationPath.Length;
            }

            //add SEO code
            url = url.Insert(startIndex, language.UniqueSeoCode);
            url = url.Insert(startIndex, "/");

            return url;
        }
    }
}