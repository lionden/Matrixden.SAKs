using Matrixden.Utils.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Matrixden.SAK.Extensions
{
    /// <summary>
    /// Extension methods form IList collection.
    /// </summary>
    public static partial class EnumerableExtension
    {
        /// <summary>
        /// Remove the item of the first occurrence within the entire <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="filter"></param>
        /// <returns>true if item was successfully removed from the System.Collections.Generic.ICollection`1; otherwise, false. This method also returns false if item is not found in the original System.Collections.Generic.ICollection`1.</returns>
        public static bool Remove<TSource>(this ICollection<TSource> source, Func<TSource, bool> filter)
        {
            var itm = source.FirstOrDefault(filter);

            return source.Remove(itm);
        }
    }
}
