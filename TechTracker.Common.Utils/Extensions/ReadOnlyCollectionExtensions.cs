using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TechTracker.Common.Utils.Extensions
{
    public static class ReadOnlyCollectionExtensions
    {
        public static ReadOnlyCollection<K> ByType<T, K>(this IEnumerable<T> list)
        {
            return new ReadOnlyCollection<K>(list.Where(i => i is K).Cast<K>().ToList());
        }

        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this List<T> t)
        {
            return new ReadOnlyCollection<T>(t);
        }

        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> t)
        {
            return new ReadOnlyCollection<T>(t.ToList());
        }
    }
}