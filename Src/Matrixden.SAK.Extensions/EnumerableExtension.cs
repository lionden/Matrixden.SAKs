using Matrixden.Utils.Extensions;
using System;
using System.Collections;
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
            if (source == default)
                return source;

            foreach (TSource item in source)
            {
                predicate(item);
            }

            return source;
        }

        /// <summary>
        /// Traversal the whole list, do the given action.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerable ForEach(this IEnumerable source, Action<object> action)
        {
            if (source == default)
                return source;

            foreach (var item in source)
            {
                action(item);
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
            if (source == default)
                return source;

            IList<TSource> list = (source as IList<TSource>) ?? source.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                predicate(list[i], i);
            }

            return list;
        }

        /// <summary>
        /// Traversal the whole list, do the given action. It'll return the item's index from 0.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerable For(this IEnumerable source, Action<object, int> action)
        {
            if (source == default)
                return source;

            var i = 0;
            foreach (var item in source)
            {
                action(item, i++);
            }

            return source;
        }

        /// <summary>
        /// Traversal the whole list, do the given action. It'll return the item's index from 0.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerable For<T>(this IEnumerable source, Action<T, int> action) => source.For((o, i) => action((T)o, i));

        /// <summary>
        /// Searches for the specified object and returns the index of the first occurrence within the entire <c>IEnumerable<T></c>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="item"></param>
        /// <param name="fieldToBeCompare"></param>
        /// <returns>The zero-based index of the first occurrence of value within the entire sequence if found; otherwise, –1.</returns>
        public static int IndexOf<T>(this IEnumerable<T> source, T item, string fieldToBeCompare = default)
        {
            if (source == default)
                return -1;

            if (typeof(T).IsPrimitive)
                fieldToBeCompare = default;

            if (fieldToBeCompare.IsNullOrEmptyOrWhiteSpace())
            {
                return Array.IndexOf(source.ToArray(), item);
            }
            else
            {
                if (!item.HasProperty(fieldToBeCompare))
                    return -1;

                return Array.IndexOf(source.Select(s => s.Value(fieldToBeCompare)).ToArray(), item.Value(fieldToBeCompare));
            }
        }
    }
}
