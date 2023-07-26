using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Matrixden.SwissArmyKnives
{
    /// <summary>
    /// 图片帮助类
    /// </summary>
    public class MImageConverter
    {
        /// <summary>
        /// 图片文件转bytes
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static byte[] Image2Bytes(Image img)
        {
            //将Image转换成流数据，并保存为byte[]
            using (var mstream = new MemoryStream())
            {
                img.Save(mstream, img.RawFormat);
                var byData = new Byte[mstream.Length];
                mstream.Position = 0;
                mstream.Read(byData, 0, byData.Length);

                return byData;
            }
        }

        /// <summary>
        /// 图片文件转为Base64String。
        /// </summary>
        /// <param name="img">图片</param>
        /// <returns></returns>
        public static string Image2Base64String(Image img)
        {
            return Convert.ToBase64String(Image2Bytes(img));
        }

        /// <summary>
        /// bytes转图片文件
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Image Bytes2Image(byte[] b)
        {
            using (var ms = new MemoryStream(b))
            {
                ms.Position = 0;
                var img = Image.FromStream(ms);

                return img;
            }
        }

        /// <summary>
        /// 图片文件Base64String转图片。
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Image Base64String2Image(string s)
        {
            var b = Convert.FromBase64String(s);
            if (b == null)
                return null;

            return Bytes2Image(b);
        }

        /// <summary>
        /// Convert hex string to image.
        /// </summary>
        /// <param name="hexStr"></param>
        /// <param name="splitter">Normally, the splitter can be '-', ' ' or NULL.</param>
        /// <returns></returns>
        public static Image HexString2Image(string hexStr, char splitter = default)
        {
            var hex = MConverter.HexString2ByteArray(hexStr, splitter);
            var ms = new System.IO.MemoryStream(hex);

            return Image.FromStream(ms);
        }
    }

    /// <summary>
    /// 图片相关拓展方法。
    /// </summary>
    public static class Extension
    {
        /// <summary>
        /// 图片转byte数组。
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] Bytes(this Image image) => MImageConverter.Image2Bytes(image);
    }
}
