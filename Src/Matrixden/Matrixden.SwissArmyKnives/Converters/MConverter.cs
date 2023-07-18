using Matrixden.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrixden.SwissArmyKnives
{
    /// <summary>
    /// Converters
    /// </summary>
    public class MConverter
    {
        /// <summary>
        /// Convert byte array to general string(ASCⅡ chars).
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static string Hex2String(byte[] hex)
        {
            if (hex == default(byte[]) || hex.Length <= 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            foreach (var b in hex)
            {
                sb.Append((char)b);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Convert hex string to general string(ASCⅡ chars).
        /// </summary>
        /// <param name="hexStr"></param>
        /// <param name="splitter">Normally, the splitter can be '-', ' ' or NULL.</param>
        /// <returns></returns>
        public static string HexString2String(string hexStr, char splitter = default(char))
        {
            hexStr = hexStr.CleanUp();
            if (hexStr.IsNullOrEmptyOrWhiteSpace()) return string.Empty;

            byte[] hex = default(byte[]);
            if (splitter == default(char))
            {
                hex = new byte[hexStr.Length / 2];
                for (var i = 0; i < hexStr.Length; i += 2)
                {
                    var hexChar = hexStr.Substring(i, 2);
                    hex[i / 2] = Convert.ToByte(hexChar, 16);
                }
            }
            else
            {
                hex = hexStr.Split(splitter).Select(s => Convert.ToByte(s, 16)).ToArray();
            }

            return Hex2String(hex);
        }
    }
}
