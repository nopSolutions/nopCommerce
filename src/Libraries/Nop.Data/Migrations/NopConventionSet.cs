using System.Collections.Generic;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Conventions;

namespace Nop.Data.Migrations
{
    public class NopConventionSet : IConventionSet
    {
        public NopConventionSet(IMigrationContext context)
            : this(new DefaultConventionSet(), new NopForeignKeyConvention(context), new NopIndexConvention(context))
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
