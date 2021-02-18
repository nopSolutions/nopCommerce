using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Versioning;
using System.Security.Principal;

namespace Nop.Web.Framework.Security
{
    /// <summary>
    /// Represent information about current OS user 
    /// </summary>
    public static class CurrentOSUser
    {
        #region Ctor

        static CurrentOSUser()
        {
            Name = Environment.UserName;

            DomainName = Environment.UserDomainName;

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    if (OperatingSystem.IsWindows())
                        PopulateWindowsUser();
                    break;
                case PlatformID.Unix:
                    PopulateLinuxUser();
                    break;
                default:
                    UserId = Name;
                    Groups = new List<string>();
                    break;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Populate information about windows user
        /// </summary>
        [SupportedOSPlatform("windows")]
        public static void PopulateWindowsUser()
        {
            Groups = WindowsIdentity.GetCurrent().Groups?.Select(p => p.Value).ToList();
            UserId = Name;
        }

        /// <summary>
        /// Populate information about linux user
        /// </summary>
        public static void PopulateLinuxUser()
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    FileName = "sh",
                    Arguments = "-c \" id -u ; id -G \""
                }
            };

            process.Start();
            process.WaitForExit();

            var res = process.StandardOutput.ReadToEnd();

            var respars = res.Split("\n");

            UserId = respars[0];
            Groups = respars[1].Split(" ").ToList();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns user name
        /// </summary>
        public static string Name { get; }

        /// <summary>
        /// Returns user domain name for Windows or group for Linux
        /// </summary>
        public static string DomainName { get; }

        /// <summary>
        /// Returns user groups
        /// </summary>
        public static List<string> Groups { get; private set; }

        /// <summary>
        /// Returns user name for Windows or user Id  for Linux like 1001
        /// </summary>
        public static string UserId { get; private set; }

        /// <summary>
        /// Returns full user name
        /// </summary>
        public static string FullName => $@"{DomainName}\{Name}";

        #endregion
    }
}