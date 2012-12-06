using Nop.Core;

namespace Nop.Services.Common
{
    /// <summary>
    ///  Maintenance service interface
    /// </summary>
    public partial interface IMaintenanceService
    {
        /// <summary>
        /// Get the current ident value
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <returns>Integer ident; null if cannot get the result</returns>
        int? GetTableIdent<T>() where T : BaseEntity;

        /// <summary>
        /// Set table ident (is supported)
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="ident">Ident value</param>
        void SetTableIdent<T>(int ident) where T : BaseEntity;
    }
}
