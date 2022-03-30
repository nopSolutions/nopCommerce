
using Microsoft.AspNetCore.Hosting;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.AbcCore;
using System;
using System.Configuration;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.IO;

namespace Nop.Plugin.Misc.AbcCore
{
    public static class ImportSettings
    {
        public static DirectoryInfo GetPromoPdfDirectory()
        {
            return GetDirectory("promotion_forms/", "Promo PDF directory");
        }

        public static DirectoryInfo GetRebatePdfDirectory()
        {
            return GetDirectory("rebate_images/", "Rebate PDF directory");
        }

        public static string GetCategoryMappingFile()
        {
            return GetPath("Resources/Mappings to Nop Categories.xlsx", "Category mapping file");
        }

        public static string GetFeaturedProductsFile()
        {
            return GetPath("Resources/Featured Products.xlsx", "Featured products file");
        }

        public static DirectoryInfo GetLocalPicturesDirectory()
        {
            return GetDirectory("product_images/", "Local pictures directory");
        }

        public static string GetEnergyGuidePdfPath()
        {
            return GetPath("wwwroot/energy_guides/", "Energy guide PDF path");
        }

        public static string GetSpecificationPdfPath()
        {
            return GetPath("wwwroot/product_specs/", "Specification PDF path");
        }

        private static DirectoryInfo GetDirectory(string path, string description)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ConfigurationErrorsException($"{description} not provided, please set in AbcSync configuration.");
            }

            var directory = Path.Combine(CoreUtilities.WebRootPath(), path);
            if (!Directory.Exists(directory))
            {
                throw new ConfigurationErrorsException($"{description} could not be found at " + directory);
            }

            return new DirectoryInfo(directory);
        }

        private static string GetPath(string path, string description)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ConfigurationErrorsException($"{description} not provided, please set in AbcSync configuration.");
            }

            var fullPath = Path.Combine(CoreUtilities.AppPath(), path);
            if (!File.Exists(fullPath) && !Directory.Exists(fullPath))
            {
                throw new ConfigurationErrorsException($"{description} could not be found at " + fullPath);
            }

            return fullPath;
        }
    }
}