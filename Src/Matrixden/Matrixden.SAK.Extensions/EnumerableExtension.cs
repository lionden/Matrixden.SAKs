using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Matrixden.SAK.Extensions
{
    /// <summary>
    /// Extension methods form IEnumerable collection.
    /// </summary>
    public static class EnumerableExtension
    {
        /// <summary>
        /// Traversal the whole list, do the given action.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        [Obsolete("Please use ForEach instead.")]
        public static IEnumerable<TSource> Do<TSource>(this IEnumerable<TSource> source, Action<TSource> predicate) => source.ForEach(predicate);

        /// <summary>
        /// Traversal the whole list, do the given action.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> predicate)
        {
            foreach (TSource item in source)
            {
                predicate(item);
            }

            return source;
        }

        /// <summary>
        /// Traversal the whole list, do the given action. It'll return the item's index.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        [Obsolete("Please use For instead.")]
        public static IEnumerable<TSource> DoWithIndex<TSource>(this IEnumerable<TSource> source, Action<TSource, int> predicate) => source.For(predicate);

        /// <summary>
        /// Traversal the whole list, do the given action. It'll return the item's index.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        public static IEnumerable<TSource> For<TSource>(this IEnumerable<TSource> source, Action<TSource, int> predicate)
        {
            IList<TSource> list = (source as IList<TSource>) ?? source.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                predicate(list[i], i);
            }

            return list;
        }
    }
}
