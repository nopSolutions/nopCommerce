using Nop.Core;

namespace Nop.Services.ExportImport.Help
{
    /// <summary>
    /// Interface for classes working with object through its properties
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGetProperties<T>
    {
        /// <summary>
        /// Get property array
        /// </summary>
        PropertyByName<T>[] GetProperties { get; }

        /// <summary>
        /// Fills the specified object
        /// </summary>
        /// <param name="objectToFill">The object to fill</param>
        /// <param name="isNew">Is new object flag</param>
        /// <param name="manager">Property manager</param>
        void FillObject(BaseEntity objectToFill, bool isNew, PropertyManager<T> manager);
    }
}
