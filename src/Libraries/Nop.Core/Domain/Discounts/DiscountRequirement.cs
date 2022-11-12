<<<<<<< HEAD
<<<<<<< HEAD
﻿namespace Nop.Core.Domain.Discounts
{
    /// <summary>
    /// Represents a discount requirement
    /// </summary>
    public partial class DiscountRequirement : BaseEntity
    {
        /// <summary>
        /// Gets or sets the discount identifier
        /// </summary>
        public int DiscountId { get; set; }

        /// <summary>
        /// Gets or sets the discount requirement rule system name
        /// </summary>
        public string DiscountRequirementRuleSystemName { get; set; }

        /// <summary>
        /// Gets or sets the parent requirement identifier
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// Gets or sets an interaction type identifier (has a value for the group and null for the child requirements)
        /// </summary>
        public int? InteractionTypeId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this requirement has any child requirements
        /// </summary>
        public bool IsGroup { get; set; }

        /// <summary>
        /// Gets or sets an interaction type
        /// </summary>
        public RequirementGroupInteractionType? InteractionType
        {
            get => (RequirementGroupInteractionType?)InteractionTypeId;
            set => InteractionTypeId = (int?)value;
        }
    }
=======
=======
=======
<<<<<<< HEAD
>>>>>>> 974287325803649b246516d81982b95e372d09b9
﻿namespace Nop.Core.Domain.Discounts
{
    /// <summary>
    /// Represents a discount requirement
    /// </summary>
    public partial class DiscountRequirement : BaseEntity
    {
        /// <summary>
        /// Gets or sets the discount identifier
        /// </summary>
        public int DiscountId { get; set; }

        /// <summary>
        /// Gets or sets the discount requirement rule system name
        /// </summary>
        public string DiscountRequirementRuleSystemName { get; set; }

        /// <summary>
        /// Gets or sets the parent requirement identifier
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// Gets or sets an interaction type identifier (has a value for the group and null for the child requirements)
        /// </summary>
        public int? InteractionTypeId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this requirement has any child requirements
        /// </summary>
        public bool IsGroup { get; set; }

        /// <summary>
        /// Gets or sets an interaction type
        /// </summary>
        public RequirementGroupInteractionType? InteractionType
        {
            get => (RequirementGroupInteractionType?)InteractionTypeId;
            set => InteractionTypeId = (int?)value;
        }
    }
<<<<<<< HEAD
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
=======
=======
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
﻿namespace Nop.Core.Domain.Discounts
{
    /// <summary>
    /// Represents a discount requirement
    /// </summary>
    public partial class DiscountRequirement : BaseEntity
    {
        /// <summary>
        /// Gets or sets the discount identifier
        /// </summary>
        public int DiscountId { get; set; }

        /// <summary>
        /// Gets or sets the discount requirement rule system name
        /// </summary>
        public string DiscountRequirementRuleSystemName { get; set; }

        /// <summary>
        /// Gets or sets the parent requirement identifier
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// Gets or sets an interaction type identifier (has a value for the group and null for the child requirements)
        /// </summary>
        public int? InteractionTypeId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this requirement has any child requirements
        /// </summary>
        public bool IsGroup { get; set; }

        /// <summary>
        /// Gets or sets an interaction type
        /// </summary>
        public RequirementGroupInteractionType? InteractionType
        {
            get => (RequirementGroupInteractionType?)InteractionTypeId;
            set => InteractionTypeId = (int?)value;
        }
    }
<<<<<<< HEAD
=======
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
>>>>>>> 974287325803649b246516d81982b95e372d09b9
}