using System;

namespace BuildIt.States
{
    /// <summary>
    /// Definition of a string state group
    /// </summary>
    public class StateGroupDefinition : TypedStateGroupDefinition<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateGroupDefinition"/> class.
        /// </summary>
        /// <param name="groupName">The name of the state group</param>
        public StateGroupDefinition(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                throw new ArgumentNullException(nameof(groupName), "Group name is required");
            }

            GroupName = groupName;
        }

        /// <summary>
        /// Gets the name of the state group (based on enum type)
        /// </summary>
        public override string GroupName { get; }

        /// <summary>
        /// Parses the state name and returns the state (in this case a string)
        /// </summary>
        /// <param name="stateName">The state name</param>
        /// <returns>The state</returns>
        public override string FromString(string stateName)
        {
            return stateName;
        }
    }
}