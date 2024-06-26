﻿using Matrixden.Utils.Extensions;
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
    public static partial class EnumerableExtension
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
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            if (source == default || !source.Any() || action == default)
                return source;

            foreach (TSource item in source)
                action(item);

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
        /// <param name="source">When it is null, return it directly.</param>
        /// <param name="action">When it is null, return source directly.</param>
        public static IEnumerable<TSource> For<TSource>(this IEnumerable<TSource> source, Action<TSource, int> action)
        {
            if (source == default || !source.Any() || action == default)
                return source;

            var i = 0;
            foreach (TSource item in source)
                action(item, i++);

            return source;
        }

        /// <summary>
        /// Traversal the whole list, do the given action. It'll return the item's index from 0.
        /// From .NET 6.0, this feature has already supported by official. Use official function, please.
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
        /// Searches for the specified object and returns the index of the first occurrence within the entire <see cref="T:System.Collections.Generic.IEnumerable`1" />.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="item"></param>
        /// <param name="fieldToBeCompare"></param>
        /// <returns>The zero-based index of the first occurrence of value within the entire sequence if found; otherwise, –1.</returns>
        public static int IndexOf<T>(this IEnumerable<T> source, T item, string fieldToBeCompare = default)
        {
            if (source == default || !source.Any())
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

        /// <summary>
        /// Searches for the specified object and returns the index of the first occurrence within the entire <see cref="T:System.Collections.Generic.IEnumerable`1" />.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="filter"></param>
        /// <returns>The zero-based index of the first occurrence of value within the entire sequence if found; otherwise, –1.</returns>
        public static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> filter)
        {
            if (source == default || !source.Any()) return -1;
            if (filter == null) return -1;

            return Array.IndexOf(source.ToArray(), source.FirstOrDefault(f => filter(f)));
        }

        /// <summary>
        /// Safely appends a value to the end of the sequence.
        /// This also provide an append method for project with NET462 or earlier.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="element"></param>
        /// <returns>A new sequence that ends with element.</returns>
        public static IEnumerable<T> AppendM<T>(this IEnumerable<T> source, T element)
        {
            if (source == default || !source.Any())
                return new T[] { element };

            var array = new T[source.Count() + 1];
            array[array.Length - 1] = element;
            source.For((s, i) =>
            {
                array[i] = s;
            });

            return array;
        }

        public static IEnumerable<TResult> SelectM<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
        {
            int i = 0;
            return source.Select(s => selector(s, i++));
        }

        public static List<T> DeepCopy<T>(this List<T> source) where T : class, new()
        {
            List<T> newList = new();
            source.ForEach(f =>
            {
                if (f is ICloneable)
                {
                    newList.Add((T)(f as ICloneable).Clone());
                }
                else
                {
                    newList.Add(f.Sync<T>());
                }
            });

            return newList;
        }
    }
}
