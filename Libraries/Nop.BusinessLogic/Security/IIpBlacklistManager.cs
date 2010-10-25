//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Security
{
    /// <summary>
    /// IP Blacklist manager interface
    /// </summary>
    public partial interface IIpBlacklistManager
    {
        /// <summary>
        /// Gets an IP address by its identifier
        /// </summary>
        /// <param name="ipAddressId">IP Address unique identifier</param>
        /// <returns>An IP address</returns>
        BannedIpAddress GetBannedIpAddressById(int ipAddressId);

        /// <summary>
        /// Gets all IP addresses
        /// </summary>
        /// <returns>An IP address collection</returns>
        List<BannedIpAddress> GetBannedIpAddressAll();

        /// <summary>
        /// Inserts an IP address
        /// </summary>
        /// <param name="ipAddress">IP address</param>
        /// <returns>IP Address</returns>
        void InsertBannedIpAddress(BannedIpAddress ipAddress);

        /// <summary>
        /// Updates an IP address
        /// </summary>
        /// <param name="ipAddress">IP address</param>
        /// <returns>IP address</returns>
        void UpdateBannedIpAddress(BannedIpAddress ipAddress);

        /// <summary>
        /// Deletes an IP address by its identifier
        /// </summary>
        /// <param name="ipAddressId">IP address unique identifier</param>
        void DeleteBannedIpAddress(int ipAddressId);

        /// <summary>
        /// Gets an IP network by its Id
        /// </summary>
        /// <param name="bannedIpNetworkId">IP network unique identifier</param>
        /// <returns>IP network</returns>
        BannedIpNetwork GetBannedIpNetworkById(int bannedIpNetworkId);

        /// <summary>
        /// Gets all IP networks
        /// </summary>
        /// <returns>IP network collection</returns>
        List<BannedIpNetwork> GetBannedIpNetworkAll();

        /// <summary>
        /// Inserts an IP network
        /// </summary>
        /// <param name="ipNetwork">IP network</param>
        void InsertBannedIpNetwork(BannedIpNetwork ipNetwork);

        /// <summary>
        /// Updates an IP network
        /// </summary>
        /// <param name="ipNetwork">IP network</param>
        void UpdateBannedIpNetwork(BannedIpNetwork ipNetwork);

        /// <summary>
        /// Deletes an IP network
        /// </summary>
        /// <param name="bannedIpNetwork">IP network unique identifier</param>
        void DeleteBannedIpNetwork(int bannedIpNetwork);

        /// <summary>
        /// Checks if an IP from the IpAddressCollection or the IpNetworkCollection is banned
        /// </summary>
        /// <param name="ipAddress">IP address</param>
        /// <returns>False or true</returns>
        bool IsIpAddressBanned(BannedIpAddress ipAddress);

        /// <summary>
        /// Check if the ip is valid.
        /// </summary>
        /// <param name="ipAddress">The string representation of an IP address</param>
        /// <returns>True if the IP is valid.</returns>
        bool IsValidIp(string ipAddress);

        /// <summary>
        /// Compares two IP addresses for equality. 
        /// </summary>
        /// <param name="ipAddress1">The first IP to compare</param>
        /// <param name="ipAddress2">The second IP to compare</param>
        /// <returns>True if equal, false if not.</returns>
        bool AreEqual(string ipAddress1, string ipAddress2);

        /// <summary>
        /// Compares two string representations of an Ip address to see if one
        /// is greater than the other
        /// </summary>
        /// <param name="toCompare">The IP address on the left hand side of the greater 
        /// than operator</param>
        /// <param name="compareAgainst">The Ip address on the right hand side of the 
        /// greater than operator</param>
        /// <returns>True if ToCompare is greater than CompareAgainst, else false</returns>       
        bool IsGreater(string toCompare, string compareAgainst);

        /// <summary>
        /// Compares two string representations of an Ip address to see if one
        /// is less than the other
        /// </summary>
        /// <param name="toCompare">The IP address on the left hand side of the less 
        /// than operator</param>
        /// <param name="compareAgainst">The Ip address on the right hand side of the 
        /// less than operator</param>
        /// <returns>True if ToCompare is greater than CompareAgainst, else false</returns>
        bool IsLess(string toCompare, string compareAgainst);

        /// <summary>
        /// Determines whether a specified object is equal to another object
        /// </summary>
        /// <param name="toCompare">The IP address on the left hand side of the less 
        /// than operator</param>
        /// <param name="compareAgainst">The Ip address on the right hand side of the 
        /// less than operator</param>
        /// <returns>Result</returns>
        bool IsEqual(string toCompare, string compareAgainst);

        /// <summary>
        /// Compares two string representations of an Ip address to see if one
        /// is greater than or equal to the other.
        /// </summary>
        /// <param name="toCompare">The IP address on the left hand side of the greater 
        /// than or equal operator</param>
        /// <param name="compareAgainst">The Ip address on the right hand side of the 
        /// greater than or equal operator</param>
        /// <returns>True if ToCompare is greater than or equal to CompareAgainst, else false</returns>
        bool IsGreaterOrEqual(string toCompare, string compareAgainst);

        /// <summary>
        /// Compares two string representations of an Ip address to see if one
        /// is less than or equal to the other.
        /// </summary>
        /// <param name="toCompare">The IP address on the left hand side of the less 
        /// than or equal operator</param>
        /// <param name="compareAgainst">The Ip address on the right hand side of the 
        /// less than or equal operator</param>
        /// <returns>True if ToCompare is greater than or equal to CompareAgainst, else false</returns>
        bool IsLessOrEqual(string toCompare, string compareAgainst);

        /// <summary>
        /// Converts a uint representation of an Ip address to a string.
        /// </summary>
        /// <param name="ipAddress">The IP address to convert</param>
        /// <returns>A string representation of the IP address.</returns>
        string LongToIpAddress(uint ipAddress);

        /// <summary>
        /// Converts a string representation of an IP address to a uint. This
        /// encoding is proper and can be used with other networking functions such
        /// as the System.Net.IPAddress class.
        /// </summary>
        /// <param name="ipAddress">The Ip address to convert.</param>
        /// <returns>Returns a uint representation of the IP address.</returns>
        uint IpAddressToLong(string ipAddress);
    }
}
