using System.Collections.Generic;
using Nop.Core.Domain.Directory;

namespace Nop.Services.Directory
{
    /// <summary>
    /// State province service interface
    /// </summary>
    public partial interface IStateProvinceService
    {
        /// <summary>
        /// Deletes a state/province
        /// </summary>
        /// <param name="stateProvince">The state/province</param>
        void DeleteStateProvince(StateProvince stateProvince);

        /// <summary>
        /// Gets a state/province
        /// </summary>
        /// <param name="stateProvinceId">The state/province identifier</param>
        /// <returns>State/province</returns>
        StateProvince GetStateProvinceById(int stateProvinceId);

        /// <summary>
        /// Gets a state/province by abbreviation
        /// </summary>
        /// <param name="abbreviation">The state/province abbreviation</param>
        /// <param name="countryId">Country identifier; pass null to load the state regardless of a country</param>
        /// <returns>State/province</returns>
        StateProvince GetStateProvinceByAbbreviation(string abbreviation, int? countryId = null);
        
        /// <summary>
        /// Gets a state/province collection by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <param name="languageId">Language identifier. It's used to sort states by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>States</returns>
        IList<StateProvince> GetStateProvincesByCountryId(int countryId, int languageId = 0, bool showHidden = false);

        /// <summary>
        /// Gets all states/provinces
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>States</returns>
        IList<StateProvince> GetStateProvinces(bool showHidden = false);

        /// <summary>
        /// Inserts a state/province
        /// </summary>
        /// <param name="stateProvince">State/province</param>
        void InsertStateProvince(StateProvince stateProvince);

        /// <summary>
        /// Updates a state/province
        /// </summary>
        /// <param name="stateProvince">State/province</param>
        void UpdateStateProvince(StateProvince stateProvince);
    }
}
