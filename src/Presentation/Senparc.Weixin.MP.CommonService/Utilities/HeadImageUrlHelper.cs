using System;
using System.Collections.Generic;
using System.Text;
using Nop.Services.Weixin;

namespace Senparc.Weixin.MP.CommonService.Utilities
{
    /// <summary>
    /// 微信用户头像链接地址帮助类
    /// </summary>
    public partial class HeadImageUrlHelper
    {
        /// <summary>
        /// 组合成完整URL
        /// </summary>
        /// <param name="urlKey"></param>
        /// <param name="size">有0、46、64、96、132数值可选，0代表640*640正方形头像</param>
        /// <returns></returns>
        public static string GetHeadImageUrl(string urlKey, int size = 0)
        {
            return string.Format(NopWeixinDefaults.HeadImageUrl, urlKey, size.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="headImageUrl"></param>
        /// <returns></returns>
        public static string GetHeadImageUrlKey(string headImageUrl)
        {
            if (string.IsNullOrEmpty(headImageUrl))
                return string.Empty;

            var startIndex = headImageUrl.IndexOf("mmopen/", StringComparison.InvariantCultureIgnoreCase);
            var endIndex = headImageUrl.LastIndexOf("/");

            if (startIndex > 0 && endIndex - 7 > startIndex)
            {
                return headImageUrl.Substring(startIndex + 7, endIndex - startIndex - 7);
            }

            return string.Empty;
        }
    }
}
