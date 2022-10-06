using System;
using System.Collections.Generic;
using Nop.Data.Mapping;
using Nop.Data.Migrations.UpgradeTo460.Domain;

namespace Nop.Data.Migrations
{
    public partial class MigrationNameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new()
        {
            { typeof(ProductAttributeCombination450), "ProductAttributeCombination" },
            { typeof(ProductAttributeValue450), "ProductAttributeValue" }
        };

        public Dictionary<(Type, string), string> ColumnName => new()
        {
        };
    }
}