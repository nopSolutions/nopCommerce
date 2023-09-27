using Nop.Core.Infrastructure;

namespace Nop.Web.Framework.Infrastructure
{
    /// <summary>
    /// Provides support for lazy initialization
    /// </summary>
    /// <typeparam name="T">Specifies the type of element being lazily initialized.</typeparam>
    public partial class LazyInstance<T> : Lazy<T> where T : class
    {
        public LazyInstance()
            : base(() => EngineContext.Current.Resolve<T>())
        {

        }
    }
}
