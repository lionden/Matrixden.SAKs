using System;
using System.Windows.Media;

namespace Matrixden.Utils.Converters
{
    public class ColorHelper
    {
        /// <summary>
        /// 将一个HTML color字符串转换为 <see cref="T:System.Windows.Media.Color" /> 格式颜色.
        /// </summary>
        /// <param name="htmlColor"></param>
        /// <returns></returns>
        public static Color ConvertToColorFromHtml(string htmlColor)
        {
            Color result = new Color();
            if (htmlColor == null || htmlColor.Length == 0)
            {
                return result;
            }

            if (htmlColor[0] == '#' && (htmlColor.Length == 7 || htmlColor.Length == 4))
            {
                if (htmlColor.Length == 7)
                {
                    result = Color.FromRgb(
                                    BitConverter.GetBytes(Convert.ToInt32(htmlColor.Substring(1, 2), 16))[0],
                                    BitConverter.GetBytes(Convert.ToInt32(htmlColor.Substring(3, 2), 16))[0],
                                    BitConverter.GetBytes(Convert.ToInt32(htmlColor.Substring(5, 2), 16))[0]);
                }
                else
                {
                    string text = char.ToString(htmlColor[1]);
                    string text2 = char.ToString(htmlColor[2]);
                    string text3 = char.ToString(htmlColor[3]);
                    result = Color.FromRgb(
                                    BitConverter.GetBytes(Convert.ToInt32(text + text, 16))[0],
                                    BitConverter.GetBytes(Convert.ToInt32(text2 + text2, 16))[0],
                                    BitConverter.GetBytes(Convert.ToInt32(text3 + text3, 16))[0]);
                }
            }

            return result;
        }
    }
}
