using Matrixden.Utils.Extensions;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Matrixden.Utils
{
    public partial class Util
    {
        private static Random RandomObject
        {
            get
            {
                var randomNumberBuffer = new byte[10];
                new RNGCryptoServiceProvider().GetBytes(randomNumberBuffer);
                return new Random(BitConverter.ToInt32(randomNumberBuffer, 0));
            }
        }

        /// <summary>
        /// Get random integer between min (inclusive) and max (exclusive)
        /// </summary>
        /// <param name="min">The minimum number inclusive</param>
        /// <param name="max">The maximum number exclusive</param>
        /// <returns>A random integer between min and max</returns>
        public static int RandomInteger(int min, int max)
        {
            int val = RandomObject.Next(min, max);
            return val;
        }

        /// <summary>
        /// Get random integer between 0 and max
        /// </summary>
        /// <param name="max">The maximum integer to return.</param>
        /// <returns>A random integer between 0 and max.</returns>
        public static int RandomInteger(int max)
        {
            return RandomInteger(0, max);
        }

        /// <summary>
        /// Get random integer number.
        /// </summary>
        /// <returns></returns>
        public static int RandomInteger()
        {
            return RandomInteger(Int32.MaxValue);
        }

        /// <summary>
        /// Gets a value indicating whether a random result
        /// </summary>
        public static bool RandomBool()
        {
            return RandomInteger(0, 2) == 0;
        }

        /// <summary>
        /// Get a random string
        /// </summary>
        /// <returns>A random string</returns>
        public static string GetRandomString()
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", string.Empty);
            return path;
        }

        /// <summary>
        /// Initializes a new System.Guid value.
        /// </summary>
        /// <param name="format">A single format specifier that indicates how to format the value of this Guid.
        /// The format parameter can be "N", "D", "B", "P", or "X".</param>
        /// <returns>Return Upper case string when <c>format</c> is Upper case, else is lower.</returns>
        public static string RandomUuid(string format)
        {
            if (format.IsNullOrEmptyOrWhiteSpace() || !format.ToUpper().IsEqualWithSpecificValue("N", "D", "B", "P", "X"))
                format = "N";

            var g = Guid.NewGuid().ToString(format);
            return char.IsUpper(format, 0) ? g.ToUpper() : g;
        }

        public static string RandomUuid()
        {
            return RandomUuid("N");
        }

        public static string RandomPhoneNumber()
        {
            return string.Format("{0}{1}{2}{3}", 1, RandomInteger(10, 99), RandomInteger(1000, 9999), RandomInteger(1000, 9999));
        }

        /// <summary>
        /// 成指定数的汉字
        /// </summary>
        /// <param name="charLen"></param>
        /// <returns></returns>
        public static string RandomName(int charLen)
        {
            int area, code;//汉字由区位和码位组成(都为0-94,其中区位16-55为一级汉字区,56-87为二级汉字区,1-9为特殊字符区)
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < charLen; i++)
            {
                area = RandomInteger(16, 88);
                if (area == 55)//第55区只有89个字符
                {
                    code = RandomInteger(1, 90);
                }
                else
                {
                    code = RandomInteger(1, 94);
                }

                sb.Append(Encoding.GetEncoding("GB2312").GetString(new byte[] { Convert.ToByte(area + 160), Convert.ToByte(code + 160) }));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Initializes a new System.Guid value.
        /// </summary>
        /// <param name="format">A single format specifier that indicates how to format the value of this Guid.
        /// The format parameter can be "N", "D", "B", "P", or "X".</param>
        /// <returns>Return Upper case string when <c>format</c> is Upper case, else is lower.</returns>
        [Obsolete("Please use RandomUuid instead.")]
        public static string GetUUID(string format)
        {
            return RandomUuid(format);
        }

        [Obsolete("Please use RandomUuid instead.")]
        public static string GetUUID()
        {
            return RandomUuid("N");
        }
    }
}
