using System;

namespace BuildIt.States.Typed.String
{
    /// <summary>
    /// Defines a group of states
    /// </summary>
    public class StateGroup
        : BaseStateGroup<StateDefinition, StateGroupDefinition>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateGroup"/> class.
        /// Constructs a group based on the supplied group name
        /// </summary>
        /// <param name="groupName">The name of the state group</param>
        /// <param name="cacheKey">The cacheKey for the state definition</param>
        public StateGroup(string groupName, string cacheKey = null)
            : base(cacheKey)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                throw new ArgumentNullException(nameof(groupName), "Group name should not be null or empty");
            }

            TypedGroupDefinition.GroupName = groupName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateGroup"/> class.
        /// Constructs a group based on the supplied group name
        /// </summary>
        /// <param name="groupDefinition">An existing group definition</param>
        public StateGroup(StateGroupDefinition groupDefinition)
            : base(groupDefinition)
        {
            if (groupDefinition == null)
            {
                throw new ArgumentNullException(nameof(groupDefinition), "Group definition should not be null or empty");
            }
        }
    }
}