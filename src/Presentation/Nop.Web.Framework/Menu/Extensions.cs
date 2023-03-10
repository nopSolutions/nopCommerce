namespace Nop.Web.Framework.Menu
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Checks whether this node or child ones has a specified system name
        /// </summary>
        /// <param name="node">Node</param>
        /// <param name="systemName">System name</param>
        /// <returns>Result</returns>
        public static bool ContainsSystemName(this SiteMapNode node, string systemName)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (string.IsNullOrWhiteSpace(systemName))
                return false;

            if (systemName.Equals(node.SystemName, StringComparison.InvariantCultureIgnoreCase))
                return true;

            return node.ChildNodes.Any(cn => ContainsSystemName(cn, systemName));
        }
    }
}
