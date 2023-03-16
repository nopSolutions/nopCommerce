using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;
using NUglify;
using NUglify.JavaScript;
using WebOptimizer;

namespace Nop.Web.Framework.WebOptimizer.Processors
{
    /// <summary>
    /// Represents a class of processor that handle javascript assets
    /// </summary>
    /// <remarks>Implementation has taken from WebOptimizer to add logging</remarks>
    public partial class NopJsMinifier : Processor
    {
        #region Methods

        /// <summary>
        /// Executes the processor on the specified configuration.
        /// </summary>
        /// <param name="context">The context used to perform processing to WebOptimizer.IAsset instances</param>
        public override async Task ExecuteAsync(IAssetContext context)
        {
            var content = new Dictionary<string, byte[]>();

            foreach (var key in context.Content.Keys)
            {
                if (key.EndsWith(".min.js", StringComparison.InvariantCultureIgnoreCase))
                {
                    content[key] = context.Content[key];
                    continue;
                }

                var input = context.Content[key].AsString();
                var result = Uglify.Js(input, new CodeSettings { TermSemicolons = true });

                var minified = result.Code;

                if (result.HasErrors)
                {
                    await EngineContext.Current.Resolve<ILogger>()
                        .WarningAsync($"JavaScript minification: {key}", new(string.Join(Environment.NewLine, result.Errors)));
                }

                content[key] = minified.AsByteArray();
            }

            context.Content = content;

            return;
        }

        #endregion

    }
}