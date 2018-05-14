using System.ComponentModel;

namespace BuildIt
{
    /// <inheritdoc />
    /// <summary>
    /// Defines an entity that has a Data property
    /// </summary>
    /// <typeparam name="TData">The type of the Data property</typeparam>
    public interface IHasImmutableData<out TData> : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the data from the entity
        /// </summary>
        TData Data { get; }
    }
}