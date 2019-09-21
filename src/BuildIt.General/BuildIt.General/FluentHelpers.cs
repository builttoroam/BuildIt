using System.Collections.Generic;

namespace BuildIt
{
    /// <summary>
    /// Helper methods to make it easier to write fluent style code.
    /// </summary>
    public static class FluentHelpers
    {
        /// <summary>
        /// Adds item to a collection, and returns the collection.
        /// </summary>
        /// <typeparam name="TCollection">Type of collection.</typeparam>
        /// <typeparam name="T">The type of item in collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="itemToAdd">The item to add.</param>
        /// <returns>The same source collection.</returns>
        public static TCollection AddItem<TCollection, T>(this TCollection source, T itemToAdd)
        where TCollection : IList<T>
        {
            if (source != null && itemToAdd != null)
            {
                source.Add(itemToAdd);
            }

            return source;
        }

        /// <summary>
        /// Adds item to a collection, and returns the collection.
        /// </summary>
        /// <typeparam name="TCollection">Type of collection.</typeparam>
        /// <typeparam name="T">The type of item in collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="index">The index to insert at.</param>
        /// <param name="itemToInsert">The item to add.</param>
        /// <returns>The same source collection.</returns>
        public static TCollection InsertItem<TCollection, T>(this TCollection source, int index, T itemToInsert)
            where TCollection : IList<T>
        {
            if (source != null && itemToInsert != null && index >= 0 && index <= source.Count)
            {
                source.Insert(index, itemToInsert);
            }

            return source;
        }

        /// <summary>
        /// Removes item from a collection, and returns the collection.
        /// </summary>
        /// <typeparam name="TCollection">Type of collection.</typeparam>
        /// <typeparam name="T">The type of item in collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="index">The index of the item to remove.</param>
        /// <returns>The original source collection.</returns>
        public static TCollection RemoveItemAt<TCollection, T>(this TCollection source, int index)
            where TCollection : IList<T>
        {
            if (source != null && index >= 0 && index < source.Count)
            {
                source.RemoveAt(index);
            }

            return source;
        }

        /// <summary>
        /// Removes item from a collection, and returns the collection.
        /// </summary>
        /// <typeparam name="TCollection">Type of collection.</typeparam>
        /// <typeparam name="T">The type of item in collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="itemToRemove">The item to remove.</param>
        /// <returns>The original source collection.</returns>
        public static TCollection RemoveItem<TCollection, T>(this TCollection source, T itemToRemove)
            where TCollection : IList<T>
        {
            if (source != null && itemToRemove != null)
            {
                source.Remove(itemToRemove);
            }

            return source;
        }
    }
}