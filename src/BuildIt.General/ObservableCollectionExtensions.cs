using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BuildIt
{
    public static class ObservableCollectionExtensions
    {
        public static void AddRange<T>(this ObservableCollection<T> target, IEnumerable<T> source)
        {
            foreach (var s in source)
            {
                target.Add(s);
            }
        }
    }
}
