using Matrixden.Utils.Extensions;
using Matrixden.Utils.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrixden.SwissArmyKnives
{
    /// <summary>
    /// Converters
    /// </summary>
    public class MFileConverter
    {
        private static readonly ILog log = LogProvider.GetCurrentClassLogger();
        /// <summary>
        /// Convert a file to hex string.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="splitter">Normally, the splitter can be '-', ' ' or NULL.</param>
        /// <returns></returns>
        public static string File2HexStr(string filePath, char splitter = ' ')
        {
            if (!File.Exists(filePath))
                return string.Empty;

            var bd = File.ReadAllBytes(filePath);
            var hs = BitConverter.ToString(bd);
            if (splitter == '-')
                return hs;

            if (splitter == default(char))
                return hs.Replace("-", string.Empty);

            return hs.Replace('-', splitter);
        }

        /// <summary>
        /// Save hex string as file.
        /// </summary>
        /// <param name="hexStr"></param>
        /// <param name="filePath"></param>
        /// <param name="splitter"></param>
        public static void HexStr2File(string hexStr, string filePath, char splitter = ' ')
        {
            if (hexStr.IsNullOrEmptyOrWhiteSpace()) return;
            var bt = MConverter.HexString2ByteArray(hexStr, splitter);
            try
            {
                File.WriteAllBytes(filePath, bt);
            }
            catch (Exception e)
            {
                log.FatalException($"Exception occurred. File path: {filePath}.", e);
            }
        }
    }
}
