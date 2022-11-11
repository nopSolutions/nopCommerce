namespace Nop.Core.Domain.Security
{
    /// <summary>
    /// Represents default values related to robots.txt
    /// </summary>
    public partial class RobotsTxtDefaults
    {
        /// <summary>
        /// Gets a name of custom robots file
        /// </summary>
        public static string RobotsFileName => "robots.txt";

        /// <summary>
        /// Gets a name of custom robots file
        /// </summary>
        public static string RobotsCustomFileName => "robots.custom.txt";

        /// <summary>
        /// Gets a name of robots additions file
        /// </summary>
        public static string RobotsAdditionsFileName => "robots.additions.txt";
    }
}
