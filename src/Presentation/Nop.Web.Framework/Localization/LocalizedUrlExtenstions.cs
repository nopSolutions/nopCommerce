using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using Nop.Core.Domain.Localization;

namespace Nop.Web.Framework.Localization
{

    public static class LocalizedUrlExtenstions
    {
        private static int _seoCodeLength = 2;

        public static bool IsLocalizedUrl(this string url, bool rawPath = false)
        {
            if (string.IsNullOrEmpty(url))
                return false;
            int length = url.Length;
            if (rawPath)
            {
                //too short url
                if (length < 1 + _seoCodeLength)
                    return false;

                //url like "/en"
                if (length == 1 + _seoCodeLength)
                    return true;

                //urls like "/en/" or "/en/somethingelse"
                return (url.Length > 1 + _seoCodeLength) && (url[1 + _seoCodeLength] == '/');
            }
            else
            {
                //too short url
                if (length < 2 + _seoCodeLength)
                    return false;

                //url like "/en"
                if (length == 2 + _seoCodeLength)
                    return true;

                //urls like "/en/" or "/en/somethingelse"
                return (url.Length > 2 + _seoCodeLength) && (url[2 + _seoCodeLength] == '/');
            }



            //old code used for SEO codes like "en-US" or "ru-RU" (more validation)
            //if (rawPath)
            //{
            //    int length = url.Length;
            //    if (length < 6)
            //        return false;
            //    else if (length == 6)
            //        return url[3] == '-';
            //    else
            //        return url.Length > 6 && url[3] == '-' && url[6] == '/';
            //}
            //else
            //{
            //    int length = url.Length;
            //    if (length < 7)
            //        return false;
            //    else if (length == 7)
            //        return url[4] == '-';
            //    else
            //        return url.Length > 7 && url[4] == '-' && url[7] == '/';
            //}
        }

        public static string RemoveLocalizedPathFromUrl(this string url, bool rawPath = false)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            int length = url.Length;
            if (rawPath)
            {
                if (length < _seoCodeLength+1)
                    return url;
                //return url.Substring(_seoCodeLength + 1, length - _seoCodeLength - 1);
                return url.Substring(_seoCodeLength + 1);
            }
            else
            {
                if (length < _seoCodeLength + 2)
                    return url;
                //return url.Substring(_seoCodeLength + 2, length - _seoCodeLength - 2);
                return url.Substring(_seoCodeLength + 2);
            }
        }

        public static string AddLocalizedPathToUrl(this string url, Language language, bool rawPath = false)
        {
            if (language == null)
                throw new ArgumentNullException("language");

            //null validation is not required
            //if (string.IsNullOrEmpty(url))
            //    return url;

            if (rawPath)
            {
                url = url.Insert(0, language.UniqueSeoCode.ToLowerInvariant());
                return url.Insert(0, "/");
            }
            else
            {
                url = url.Insert(1, language.UniqueSeoCode.ToLowerInvariant());
                return url.Insert(1, "/");
            }
        }


        public static string GetLanguageSeoCodeFromUrl(this string rawPath)
        {
            return rawPath.Substring(1, _seoCodeLength);
        }

        public static string GetLanguageSeoCodeFromUrl(this HttpContextBase httpContext)
        {
            string virtualPath = httpContext.Request.AppRelativeCurrentExecutionFilePath;
            //string old = virtualPath;
            if (virtualPath.IsLocalizedUrl())
            {
                string languageSeoCode = virtualPath.Substring(2, _seoCodeLength);
                return languageSeoCode;
            }
            return null;
        }
    }
}