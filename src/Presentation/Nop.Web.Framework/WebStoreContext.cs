using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Stores;
using Nop.Services.Stores;

namespace Nop.Web.Framework
{
    /// <summary>
    /// Store context for web application
    /// </summary>
    public partial class WebStoreContext : IStoreContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStoreService _storeService;
        private readonly IWebHelper _webHelper;

        private Store _cachedStore;

        public WebStoreContext(IHttpContextAccessor httpContextAccessor, IStoreService storeService, IWebHelper webHelper)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._storeService = storeService;
            this._webHelper = webHelper;
        }

        /// <summary>
        /// Gets or sets the current store
        /// </summary>
        public virtual Store CurrentStore
        {
            get
            {
                if (_cachedStore != null)
                    return _cachedStore;

                //try to determine the current store by HOST header
#if NET451
                var host = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Host];
#else
                var host = "";
#endif
                var allStores = _storeService.GetAllStores();
                var store = allStores.FirstOrDefault(s => s.ContainsHostValue(host));

                if (store == null)
                {
                    //load the first found store
                    store = allStores.FirstOrDefault();
                }

                _cachedStore = store ?? throw new Exception("No store could be loaded");

                return _cachedStore;
            }
        }
    }
}
