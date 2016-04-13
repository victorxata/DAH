using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTracker.Common.Utils.Extensions
{
    // ReSharper disable once InconsistentNaming
    public static class IListExtensions
    {
        public static void Replace<T>(this IList<T> list, T originalItem, T replacementItem)
        {
            Replace(list, originalItem, new List<T> {replacementItem});
        }

        public static void Replace<T>(this IList<T> list, T originalItem, IList<T> replacements)
        {
            var index = list.IndexOf(originalItem);
            if (index < 0) throw new NullReferenceException("The 'originalItem' object " + originalItem + "is not present in 'list'.");

            list.Remove(originalItem);
            foreach (var newUnit in replacements)
                list.Insert(index++, newUnit);
        }

        public static string ToCommaSeperatedString<T>(this IEnumerable<T> list)
        {
            return string.Join(",", list.ToArray());
        }
    }
}