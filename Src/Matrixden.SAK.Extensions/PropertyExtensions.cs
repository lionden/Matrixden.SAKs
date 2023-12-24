using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Matrixden.Utils.Extensions
{
    public static partial class Extensions
    {
        /// <summary>
        /// Determine whether a given type has the special property.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool HasProperty(this Type type, string property)
        {
            if (type == default || property == default)
                return false;

            var ps = type.GetProperty(property);
            return ps != default;
        }

        /// <summary>
        /// Determine whether a given object has the special property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool HasProperty(this object @object, string property) => @object.GetType().HasProperty(property);

        /// <summary>
        /// Get the value of given object's special property.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static object Value(this object @this, string property)
        {
            if (@this == default || property == default)
                return default;

            var pi = @this.GetType().GetProperty(property);
            if (pi == default)
                return default;

            return pi.GetValue(@this);
        }

        /// <summary>
        /// Get the value of given object's special property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static T Value<T>(this object @this, string property) where T : class, new() => (T)@this.Value(property);
    }
}
