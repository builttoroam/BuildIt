using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BuildIt
{
    /// <summary>
    /// Helper methods designed to help with immutable data
    /// </summary>
    public static class ReduxHelpers
    {
        /// <summary>
        /// Takes a deep clone of a collection (ie does deep clone of all elements)
        /// </summary>
        /// <typeparam name="T">The type of item in collection</typeparam>
        /// <param name="source">The source collection</param>
        /// <returns>A deep clone of the collection</returns>
        public static ObservableCollection<T> DeepClone<T>(this ObservableCollection<T> source)
            where T : new()
        {
            if (source == null)
            {
                return null;
            }

            var helper = TypeHelper.RetrieveHelperForType(typeof(T));
            return source.Map<ObservableCollection<T>, T>(item =>
            {
                var newItem = new T();
                helper.DeepUpdater(newItem, item);
                return newItem;
            });
        }

        /// <summary>
        /// Maps one collection onto another based on some tranforming function
        /// </summary>
        /// <typeparam name="TCollection">Source collection type</typeparam>
        /// <typeparam name="TItem">collection item type</typeparam>
        /// <param name="sourceCollection">The source collection</param>
        /// <param name="map">The transform function</param>
        /// <returns>The generated collection</returns>
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