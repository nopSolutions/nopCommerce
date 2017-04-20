
namespace Nop.Web.Framework.Mvc
{
    /// <summary>
    /// Represents custom model attribute
    /// </summary>
    public interface IModelAttribute
    {
        /// <summary>
        /// Gets name of the attribute
        /// </summary>
        string Name { get; }
    }
}
