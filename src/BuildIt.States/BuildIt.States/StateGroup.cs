namespace BuildIt.States
{
    /// <summary>
    /// Defines a group of states
    /// </summary>
    public class StateGroup : TypedStateGroup<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateGroup"/> class.
        /// Constructs a group based on the supplied group name
        /// </summary>
        /// <param name="groupName">The name of the state group</param>
        public StateGroup(string groupName)
            : base(new StateGroupDefinition(groupName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateGroup"/> class.
        /// Constructs a group based on the supplied group name
        /// </summary>
        /// <param name="groupDefinition">An existing group definition</param>
        public StateGroup(StateGroupDefinition groupDefinition)
            : base(groupDefinition)
        {
        }
    }
}