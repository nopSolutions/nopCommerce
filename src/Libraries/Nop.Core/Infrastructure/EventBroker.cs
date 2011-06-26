using System;
using System.Diagnostics;
using System.Web;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// A broker for events from the http application. The purpose of this 
    /// class is to reduce the risk of temporary errors during initialization
    /// causing the site to be crippled.
    /// </summary>
    public class EventBroker
    {
        static EventBroker()
        {
            Instance = new EventBroker();
        }

        /// <summary>Accesses the event broker singleton instance.</summary>
        public static EventBroker Instance
        {
            get { return Singleton<EventBroker>.Instance; }
            protected set { Singleton<EventBroker>.Instance = value; }
        }

        /// <summary>Attaches to events from the application instance.</summary>
        public virtual void Attach(HttpApplication application)
        {
            Trace.WriteLine("EventBroker: Attaching to " + application);

            application.BeginRequest += Application_BeginRequest;
            application.AuthorizeRequest += Application_AuthorizeRequest;

            application.PostResolveRequestCache += Application_PostResolveRequestCache;
            application.PostMapRequestHandler += Application_PostMapRequestHandler;

            application.AcquireRequestState += Application_AcquireRequestState;
            application.Error += Application_Error;
            application.EndRequest += Application_EndRequest;

            application.Disposed += Application_Disposed;
        }

        public EventHandler<EventArgs> BeginRequest;
        public EventHandler<EventArgs> AuthorizeRequest;
        public EventHandler<EventArgs> PostResolveRequestCache;
        public EventHandler<EventArgs> AcquireRequestState;
        public EventHandler<EventArgs> PostMapRequestHandler;
        public EventHandler<EventArgs> Error;
        public EventHandler<EventArgs> EndRequest;
        
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (BeginRequest != null && !IsStaticResource(sender))
            {
                Debug.WriteLine("Application_BeginRequest");
                BeginRequest(sender, e);
            }
        }

        protected void Application_AuthorizeRequest(object sender, EventArgs e)
        {
            if (AuthorizeRequest != null && !IsStaticResource(sender))
            {
                Debug.WriteLine("Application_AuthorizeRequest");
                AuthorizeRequest(sender, e);
            }
        }

        private void Application_PostResolveRequestCache(object sender, EventArgs e)
        {
            if (PostResolveRequestCache != null && !IsStaticResource(sender))
            {
                Debug.WriteLine("Application_PostResolveRequestCache");
                PostResolveRequestCache(sender, e);
            }
        }

        private void Application_PostMapRequestHandler(object sender, EventArgs e)
        {
            if (PostMapRequestHandler != null && !IsStaticResource(sender))
            {
                Debug.WriteLine("Application_PostMapRequestHandler");
                PostMapRequestHandler(sender, e);
            }
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            if (AcquireRequestState != null && !IsStaticResource(sender))
            {
                Debug.WriteLine("Application_AcquireRequestState");
                AcquireRequestState(sender, e);
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            if (Error != null && !IsStaticResource(sender))
                Error(sender, e);
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            if (EndRequest != null && !IsStaticResource(sender))
                EndRequest(sender, e);
        }

        /// <summary>Detaches events from the application instance.</summary>
        void Application_Disposed(object sender, EventArgs e)
        {
            Trace.WriteLine("EventBroker: Disposing " + sender);
        }

        protected static bool IsStaticResource(object sender)
        {
            HttpApplication application = sender as HttpApplication;
            if (application != null)
            {
                IWebHelper webHelper = new WebHelper();
                return webHelper.IsStaticResource(application.Request);
            }
            return false;
        }
    }
}
