namespace Matrixden.Utils.Web
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Web;

    public static class Extensions
    {
        #region  -- HttpCookie --

        /// <summary>
        /// Validate is cookie expired.
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static bool Expired(this HttpCookie cookie)
        {
            return cookie.Expires != new DateTime() && cookie.Expires <= DateTime.Now;
        }

        #endregion

        /// <summary>
        /// Encodes a URL string.
        /// </summary>
        /// <param name="str">The text to encode.</param>
        /// <param name="isToUpper">Whether change char's case.</param>
        /// <returns>An encoded string.</returns>
        public static string UrlEncode(this string str, bool isToUpper)
        {
            if (!isToUpper)
                return HttpUtility.UrlEncode(str);

            StringBuilder builder = new StringBuilder();
            foreach (char c in str)
            {
                if (HttpUtility.UrlEncode(c.ToString()).Length > 1)
                {
                    builder.Append(HttpUtility.UrlEncode(c.ToString()).ToUpper());
                }
                else
                {
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Encodes a URL string.
        /// </summary>
        /// <param name="str">The text to encode.</param>
        /// <returns>An encoded string.</returns>
        public static string UrlEncode(this string str)
        {
            return str.UrlEncode(true);
        }
    }
}
