namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Defines properties and methods for a state definition based on an enum
    /// </summary>
    /// <typeparam name="TStateEnum">The enum type</typeparam>
    // ReSharper disable once TypeParameterCanBeVariant - not required as type should be an enum
    public interface IEnumStateDefinition<TStateEnum> : IStateDefinition 
        where TStateEnum : struct
    {
        /// <summary>
        /// The enum value for the state definition
        /// </summary>
        TStateEnum EnumState { get; }

    }
}