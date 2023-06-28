using System.Text.Json.Serialization;
using Nop.Core.Configuration;
using WebOptimizer;

namespace Nop.Web.Framework.WebOptimizer
{
    public partial class WebOptimizerConfig : WebOptimizerOptions, IConfig
    {
        #region Ctor

        public WebOptimizerConfig()
        {
            EnableDiskCache = true;
            EnableTagHelperBundling = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// A value indicating whether JS file bundling and minification is enabled
        /// </summary>
        public bool EnableJavaScriptBundling { get; protected set; } = true;

        /// <summary>
        /// A value indicating whether CSS file bundling and minification is enabled
        /// </summary>
        public bool EnableCssBundling { get; protected set; } = true;

        /// <summary>
        /// Gets or sets a suffix for the js-file name of generated bundles
        /// </summary>
        public string JavaScriptBundleSuffix { get; protected set; } = ".scripts";

        /// <summary>
        /// Gets or sets a suffix for the css-file name of generated bundles
        /// </summary>
        public string CssBundleSuffix { get; protected set; } = ".styles";

        /// <summary>
        /// Gets a section name to load configuration
        /// </summary>
        [JsonIgnore]
        public string Name => "WebOptimizer";

        /// <summary>
        /// Gets an order of configuration
        /// </summary>
        /// <returns>Order</returns>
        public int GetOrder() => 2;

        #endregion
    }
}