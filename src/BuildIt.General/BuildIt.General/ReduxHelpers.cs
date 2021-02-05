using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BuildIt
{
    /// <summary>
    /// Helper methods designed to help with immutable data.
    /// </summary>
    public static class ReduxHelpers
    {
        /// <summary>
        /// Takes a deep clone of a collection (ie does deep clone of all elements).
        /// </summary>
        /// <typeparam name="T">The type of item in collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <returns>A deep clone of the collection.</returns>
        public static ObservableCollection<T> DeepClone<T>(this ObservableCollection<T> source)
            where T : new()
        {
            return source?.Map<ObservableCollection<T>, T>(item => item.DeepEntityClone());
        }

        /// <summary>
        /// Does a deep clone of an entity.
        /// </summary>
        /// <typeparam name="T">The type of entity to clone.</typeparam>
        /// <param name="source">The source entity.</param>
        /// <returns>The cloned entity.</returns>
        public static T DeepEntityClone<T>(this T source)
            where T : new()
        {
            if (source == null)
            {
                return default;
            }

            var helper = TypeHelper.RetrieveHelperForType(typeof(T));
            var newItem = new T();
            helper.DeepUpdater(newItem, source);
            return newItem;
        }

        /// <summary>
        /// Clones and adds item.
        /// </summary>
        /// <typeparam name="T">Type of item in collection.</typeparam>
        /// <param name="source">source collection.</param>
        /// <param name="itemToAdd">Item to add.</param>
        /// <returns>new collection.</returns>
        public static ObservableCollection<T> DeepAdd<T>(this ObservableCollection<T> source, T itemToAdd)
            where T : new()
        {
            var clonedItem = itemToAdd.DeepEntityClone();
            var collection = source.DeepClone().AddItem(clonedItem);
            return collection;
        }

        /// <summary>
        /// Clones and inserts item.
        /// </summary>
        /// <typeparam name="T">Type of item in collection.</typeparam>
        /// <param name="source">source collection.</param>
        /// <param name="index">position to insert item.</param>
        /// <param name="itemToAdd">Item to insert.</param>
        /// <returns>new collection.</returns>
        public static ObservableCollection<T> DeepInsert<T>(this ObservableCollection<T> source, int index, T itemToAdd)
            where T : new()
        {
            var clonedItem = itemToAdd.DeepEntityClone();
            var collection = source.DeepClone().InsertItem(index, clonedItem);
            return collection;
        }

        /// <summary>
        /// Clones and removes item.
        /// </summary>
        /// <typeparam name="T">Type of item in collection.</typeparam>
        /// <param name="source">source collection.</param>
        /// <param name="itemToRemove">item to remove.</param>
        /// <returns>new collection.</returns>
        public static ObservableCollection<T> DeepRemove<T>(this ObservableCollection<T> source, T itemToRemove)
            where T : new()
        {
            return source?.Map<ObservableCollection<T>, T>(item =>
            {
                if (item != null && item.Equals(itemToRemove))
                {
                    return default;
                }

                return item.DeepEntityClone();
            });
        }

        /// <summary>
        /// Clones and removes item.
        /// </summary>
        /// <typeparam name="T">Type of item in collection.</typeparam>
        /// <param name="source">source collection.</param>
        /// <param name="index">position of item to remove.</param>
        /// <returns>new collection.</returns>
        public static ObservableCollection<T> DeepRemoveAt<T>(this ObservableCollection<T> source, int index)
            where T : new()
        {
            var collection = source.DeepClone().RemoveItemAt<ObservableCollection<T>, T>(index);
            return collection;
        }

        /// <summary>
        /// Maps one collection onto another based on some tranforming function.
        /// </summary>
        /// <typeparam name="TCollection">Source collection type.</typeparam>
        /// <typeparam name="TItem">collection item type.</typeparam>
        /// <param name="sourceCollection">The source collection.</param>
        /// <param name="map">The transform function.</param>
        /// <returns>The generated collection.</returns>
        public static TCollection Map<TCollection, TItem>(this TCollection sourceCollection, Func<TItem, TItem> map)
        where TCollection : IList<TItem>, new()
        {
            var targetCollection = new TCollection();
            foreach (var item in sourceCollection)
            {
                var newItem = map(item);
                if (newItem != null)
                {
                    targetCollection.Add(newItem);
                }
            }

            return targetCollection;
        }
    }
}