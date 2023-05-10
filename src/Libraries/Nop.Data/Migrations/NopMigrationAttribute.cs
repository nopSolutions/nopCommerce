using System.Globalization;
using FluentMigrator;
using Nop.Core;

namespace Nop.Data.Migrations
{
    /// <summary>
    /// Attribute for a migration
    /// </summary>
    public partial class NopMigrationAttribute : MigrationAttribute
    {
        #region Fields

        protected readonly MigrationConfig _config;

        #endregion

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the NopMigrationAttribute class
        /// </summary>
        /// <param name="dateTime">The migration date time string to convert on version</param>
        /// <param name="targetMigrationProcess">The target migration process</param>
        public NopMigrationAttribute(string dateTime, MigrationProcessType targetMigrationProcess = MigrationProcessType.NoMatter) :
            this(new MigrationConfig
            {
                DateTime = dateTime,
                TargetMigrationProcess = targetMigrationProcess
            })
        {
        }

        /// <summary>
        /// Initializes a new instance of the NopMigrationAttribute class
        /// </summary>
        /// <param name="dateTime">The migration date time string to convert on version</param>
        /// <param name="description">The migration description</param>
        /// <param name="targetMigrationProcess">The target migration process</param>
        public NopMigrationAttribute(string dateTime, string description, MigrationProcessType targetMigrationProcess = MigrationProcessType.NoMatter) :
            this(new MigrationConfig
            {
                DateTime = dateTime,
                Description = description,
                TargetMigrationProcess = targetMigrationProcess
            })
        {
        }

        /// <summary>
        /// Initializes a new instance of the NopMigrationAttribute class
        /// </summary>
        /// <param name="dateTime">The migration date time string to convert on version</param>
        /// <param name="nopVersion">nopCommerce full version</param>
        /// <param name="updateMigrationType">The update migration type</param>
        /// <param name="targetMigrationProcess">The target migration process</param>
        public NopMigrationAttribute(string dateTime, string nopVersion, UpdateMigrationType updateMigrationType, MigrationProcessType targetMigrationProcess = MigrationProcessType.NoMatter) :
            this(new MigrationConfig
            {
                DateTime = dateTime,
                NopVersion = nopVersion,
                UpdateMigrationType = updateMigrationType,
                TargetMigrationProcess = targetMigrationProcess,
            })
        {
        }

        /// <summary>
        /// Initializes a new instance of the NopMigrationAttribute class
        /// </summary>
        /// <param name="config">The migration configuration data</param>
        protected NopMigrationAttribute(MigrationConfig config) : base(config.Version, config.Description)
        {
            _config = config;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Target migration process
        /// </summary>
        public virtual MigrationProcessType TargetMigrationProcess => _config.TargetMigrationProcess;

        /// <summary>
        /// Gets the value which indicate is this schema migration
        /// </summary>
        ///<remarks>
        /// If set to true than this migration will apply right after the migration runner will become available.
        /// Do not us dependency injection in migrations that are marked as schema migration,
        /// because IoC container not ready yet.
        ///</remarks>
        public virtual bool IsSchemaMigration
        {
            get => _config.IsSchemaMigration;
            protected set => _config.IsSchemaMigration = value;
        }


        /// <summary>
        /// Gets the flag which indicate whether the migration should be applied into DB on the debug mode
        /// </summary>
        public virtual bool ApplyInDbOnDebugMode
        {
            get => _config.ApplyInDbOnDebugMode;
            protected set => _config.ApplyInDbOnDebugMode = value;
        }

        #endregion

        #region Nested class

        protected partial class MigrationConfig
        {
            #region Fields

            protected long? _version;
            protected string _description;

            #endregion

            /// <summary>
            /// Gets or sets the migration date time string to convert on version
            /// </summary>
            public string DateTime { get; set; }

            /// <summary>
            /// nopCommerce full version
            /// </summary>
            public string NopVersion { get; set; }

            /// <summary>
            /// Gets or sets the update migration type
            /// </summary>
            public virtual UpdateMigrationType? UpdateMigrationType { get; set; }

            /// <summary>
            /// Gets or sets the target migration process type
            /// </summary>
            public virtual MigrationProcessType TargetMigrationProcess { get; set; } = MigrationProcessType.NoMatter;

            /// <summary>
            /// Gets or sets the migration version
            /// </summary>
            public virtual long Version
            {
                get
                {
                    if (_version.HasValue)
                        return _version.Value;

                    if (string.IsNullOrEmpty(DateTime))
                        throw new NopException("One of the following properties must be initialized: either Version or DateTime");

                    var version = System.DateTime
                        .ParseExact(DateTime, NopMigrationDefaults.DateFormats, CultureInfo.InvariantCulture).Ticks;

                    if (UpdateMigrationType.HasValue)
                        version += (int)UpdateMigrationType;

                    return version;
                }
                set => _version = value;
            }

            /// <summary>
            /// Gets or sets the migration description
            /// </summary>
            public virtual string Description
            {
                get
                {
                    if (!string.IsNullOrEmpty(_description))
                        return _description;

                    string description = null;

                    if (string.IsNullOrEmpty(NopVersion))
                        throw new NopException("One of the following properties must be initialized: either Description or NopVersion");

                    if (UpdateMigrationType.HasValue)
                        description = string.Format(NopMigrationDefaults.UpdateMigrationDescription, NopVersion, UpdateMigrationType.ToString());

                    return description;
                }
                set => _description = value;
            }

            /// <summary>
            /// Gets the flag which indicate whether the migration should be applied into DB on the debug mode
            /// </summary>
            public bool ApplyInDbOnDebugMode { get; set; } = true;

            /// <summary>
            /// Gets or sets the value which indicate is this schema migration
            /// </summary>
            ///<remarks>
            /// If set to true than this migration will apply right after the migration runner will become available.
            /// Do not us dependency injection in migrations that are marked as schema migration,
            /// because IoC container not ready yet.
            ///</remarks>
            public virtual bool IsSchemaMigration { get; set; }
        }

        #endregion
    }
}
