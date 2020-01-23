using System.Collections.Generic;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Conventions;

namespace Nop.Data.Migrations
{
    /// <summary>
    /// A set conventions to be applied to expressions
    /// </summary>
    public class NopConventionSet : IConventionSet
    {
        public NopConventionSet(INopDataProvider dataProvider, IMigrationContext context)
            : this(new DefaultConventionSet(), new NopForeignKeyConvention(dataProvider, context), new NopIndexConvention(dataProvider))
        {
            
        }

        public NopConventionSet(IConventionSet innerConventionSet, NopForeignKeyConvention foreignKeyConvention, NopIndexConvention indexConvention)
        {
            ForeignKeyConventions = new List<IForeignKeyConvention>()
            {
                foreignKeyConvention,
                innerConventionSet.SchemaConvention,
            };

            IndexConventions = new List<IIndexConvention>()
            {
                indexConvention
            };
            
            ColumnsConventions = innerConventionSet.ColumnsConventions;
            ConstraintConventions = innerConventionSet.ConstraintConventions;
            
            SequenceConventions = innerConventionSet.SequenceConventions;
            AutoNameConventions = innerConventionSet.AutoNameConventions;
            SchemaConvention = innerConventionSet.SchemaConvention;
            RootPathConvention = innerConventionSet.RootPathConvention;
        }

        public IRootPathConvention RootPathConvention { get; }

        public DefaultSchemaConvention SchemaConvention { get; }

        public IList<IColumnsConvention> ColumnsConventions { get; }

        public IList<IConstraintConvention> ConstraintConventions { get; }

        public IList<IForeignKeyConvention> ForeignKeyConventions { get; }

        public IList<IIndexConvention> IndexConventions { get; }

        public IList<ISequenceConvention> SequenceConventions { get; }

        public IList<IAutoNameConvention> AutoNameConventions { get; }
    }
}
