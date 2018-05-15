
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BatchLabs.Max2016.Plugin.Common
{
    public static class ObservableCollectionExtensions
    {
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            items.ToList().ForEach(collection.Add);
        }
    }
}
