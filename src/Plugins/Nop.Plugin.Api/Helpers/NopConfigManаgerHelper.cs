using System.Xml.Linq;
using Nop.Core.Data;

namespace Nop.Plugin.Api.Helpers
{
    using System;
    using System.Reflection;
    using System.Xml;
    using System.Xml.XPath;

    public class NopConfigManagerHelper : IConfigManagerHelper
    {
        public NopConfigManagerHelper(DataSettings dataSettings)
        {
            DataSettings = dataSettings;
        }

        public NopConfigManagerHelper()
        {
            DataSettings = DataSettingsManager.LoadSettings();
        }

        public void AddBindingRedirects()
        {
            var hasChanged = false;

            // load Nop.Web.exe.config
            XDocument appConfig = null;

            var nopWebAssemblyConfigLocation = $"{Assembly.GetEntryAssembly().Location}.config";

            using (var fs = System.IO.File.OpenRead(nopWebAssemblyConfigLocation))
            {
                appConfig = XDocument.Load(fs);
            }

            if (appConfig != null)
            {
                appConfig.Changed += (o, e) => { hasChanged = true; };

                var runtime = appConfig.XPathSelectElement("configuration//runtime");

                if (runtime == null)
                {
                    runtime = new XElement("runtime");
                    appConfig.XPathSelectElement("configuration")?.Add(runtime);
                }

                // Required by Swagger
                //AddAssemblyBinding(runtime, "Microsoft.AspNetCore.StaticFiles", "adb9793829ddae60", "0.0.0.0-2.0.0.0", "2.0.0.0");
                //AddAssemblyBinding(runtime, "Microsoft.Extensions.FileProviders.Embedded", "adb9793829ddae60", "0.0.0.0-2.0.0.0", "2.0.0.0");
                //AddAssemblyBinding(runtime, "Microsoft.AspNetCore.Mvc.Formatters.Json", "adb9793829ddae60", "0.0.0.0-2.0.0.0", "2.0.0.0");

                // Required by WebHooks
                AddAssemblyBinding(runtime, "Microsoft.AspNetCore.DataProtection.Abstractions", "adb9793829ddae60", "0.0.0.0-2.0.0.0", "2.0.0.0");

                if (hasChanged)
                {
                    // only save when changes have been made
                    try
                    {
                        appConfig.Save(nopWebAssemblyConfigLocation);

                        //TODO: Upgrade 4.10 Check this!
                        //System.Configuration.ConfigurationManager.RefreshSection("runtime");
                    }
                    catch (Exception)
                    {
                        // we should do nothing here as throwing an exception breaks nopCommerce.
                        // The right thing to do is to write a message in the Log that the user needs to provide Write access to Web.config
                        // but doing this will lead to many warnings in the Log added after each restart. 
                        // So it is better to do nothing here.
                        //throw new NopException(
                        //    "nopCommerce needs to be restarted due to a configuration change, but was unable to do so." +
                        //    Environment.NewLine +
                        //    "To prevent this issue in the future, a change to the web server configuration is required:" +
                        //    Environment.NewLine +
                        //    "- give the application write access to the 'web.config' file.");
                    }
                }
            }
        }

        public DataSettings DataSettings { get; }

        public void AddConnectionString()
        {
            var hasChanged = false;

            // load web.config
            XDocument appConfig = null;

            var nopWebAssemblyConfigLocation = $"{Assembly.GetEntryAssembly().Location}.config";

            using (var fs = System.IO.File.OpenRead(nopWebAssemblyConfigLocation))
            {
                appConfig = XDocument.Load(fs);
            }

            if (appConfig != null)
            {
                appConfig.Changed += (o, e) => { hasChanged = true; };

                var connectionStrings = appConfig.XPathSelectElement("configuration//connectionStrings");

                if (connectionStrings == null)
                {
                    var configuration = appConfig.XPathSelectElement("configuration");
                    connectionStrings = new XElement("connectionStrings");
                    configuration.Add(connectionStrings);
                }

                var connectionStringFromNop = DataSettings.DataConnectionString;

                var element = appConfig.XPathSelectElement("configuration//connectionStrings//add[@name='MS_SqlStoreConnectionString']");

                // create the connection string if not exists
                if (element == null)
                {
                    element = new XElement("add");
                    element.SetAttributeValue("name", "MS_SqlStoreConnectionString");
                    element.SetAttributeValue("connectionString", connectionStringFromNop);
                    element.SetAttributeValue("providerName", "System.Data.SqlClient");
                    connectionStrings.Add(element);
                }
                else
                {
                    // Check if the connection string is changed.
                    // If so update the connection string in the config.
                    var connectionStringInConfig = element.Attribute("connectionString").Value;

                    if (!String.Equals(connectionStringFromNop, connectionStringInConfig, StringComparison.InvariantCultureIgnoreCase))
                    {
                        element.SetAttributeValue("connectionString", connectionStringFromNop);
                    }
                }

                if (hasChanged)
                {
                    // only save when changes have been made
                    try
                    {
                        appConfig.Save(nopWebAssemblyConfigLocation);

                        //TODO: Upgrade 4.1. Check this!
                        //System.Configuration.ConfigurationManager.RefreshSection("connectionStrings");
                    }
                    catch
                    {
                        // we should do nothing here as throwing an exception breaks nopCommerce.
                        // The right thing to do is to write a message in the Log that the user needs to provide Write access to Web.config
                        // but doing this will lead to many warnings in the Log added after each restart. 
                        // So it is better to do nothing here.
                        //throw new NopException(
                        //    "nopCommerce needs to be restarted due to a configuration change, but was unable to do so." +
                        //    Environment.NewLine +
                        //    "To prevent this issue in the future, a change to the web server configuration is required:" +
                        //    Environment.NewLine +
                        //    "- give the application write access to the 'web.config' file.");
                    }
                }
            }
        }

        private void AddAssemblyBinding(XElement runtime, string name, string publicToken, string oldVersion, string newVersion)
        {
            var xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
            xmlNamespaceManager.AddNamespace("bind", "urn:schemas-microsoft-com:asm.v1");

            var assemblyBindingElement = runtime.XPathSelectElement(
                    $"bind:assemblyBinding//bind:dependentAssembly//bind:assemblyIdentity[@name='{name}']", xmlNamespaceManager);

            // create the binding redirect if it does not exist
            if (assemblyBindingElement == null)
            {
                assemblyBindingElement = XElement.Parse($@"
                    <assemblyBinding xmlns=""urn:schemas-microsoft-com:asm.v1"">
                        <dependentAssembly>
                            <assemblyIdentity name=""{name}"" publicKeyToken=""{publicToken}"" culture=""neutral"" />
                            <bindingRedirect oldVersion=""{oldVersion}"" newVersion=""{newVersion}"" />
                        </dependentAssembly>
                    </assemblyBinding>
                ");

                runtime.Add(assemblyBindingElement);
            }
        }
    }
}