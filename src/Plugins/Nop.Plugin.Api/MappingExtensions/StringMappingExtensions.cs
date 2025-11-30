using System;
using System.Security.Cryptography;
using System.Text;

namespace Nop.Plugin.Api.MappingExtensions;

public static class StringMappingExtensions
{
    
    public static string GenerateHash(this string input)
    {
        using MD5 md5 = MD5.Create();
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = md5.ComputeHash(inputBytes);

        return Convert.ToHexString(hashBytes).ToLower(); // Converts to a lowercase hex string
    }
    
}