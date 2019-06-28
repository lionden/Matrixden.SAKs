using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using S = System.Drawing;

namespace Matrixden.Utils.Image
{
    /// <summary>
    /// Matrixden image operations.
    /// </summary>
    public class MatImage
    {
        /// <summary>
        /// 缩放调整图片尺寸
        /// </summary>
        /// <param name="src">原始图片</param>
        /// <param name="width">目标宽度</param>
        /// <param name="height">目标高度</param>
        /// <param name="maintainAspectRatio">保持横纵比</param>
        /// <returns></returns>
        public static S.Image Resize(S.Image src, int width, int height, bool maintainAspectRatio = true)
        {
            ComputeRealDestSize(src.Width, src.Height, ref width, ref height, maintainAspectRatio);
            var newImage = new Bitmap(width, height);

            using (var g = Graphics.FromImage(newImage))
            {
                g.InterpolationMode = InterpolationMode.High;
                g.DrawImage(src, 0, 0, width, height);
            }

            return newImage;
        }

        /// <summary>
        /// 缩放调整图片尺寸
        /// </summary>
        /// <param name="src">原始图片</param>
        /// <param name="size">目标尺寸</param>
        /// <returns></returns>
        public static S.Image Resize(S.Image src, Size size) => Resize(src, size.Width, size.Height);

        /// <summary>
        /// 缩放调整图片尺寸
        /// </summary>
        /// <param name="src">原始图片</param>
        /// <param name="percentage">缩放比例</param>
        /// <returns></returns>
        public static S.Image Resize(S.Image src, int percentage) => Resize(src, src.Width * percentage, src.Height * percentage);

        /// <summary>
        /// 剪裁调整图片尺寸
        /// </summary>
        /// <param name="src">原始图片</param>
        /// <param name="rec">矩形区域</param>
        /// <returns></returns>
        public static S.Image Cut(S.Image src, Rectangle rec)
        {
            var newImage = new Bitmap(rec.Width, rec.Height);

            using (var g = Graphics.FromImage(newImage))
            {
                g.InterpolationMode = InterpolationMode.High;
                g.DrawImage(src, 0, 0, rec, GraphicsUnit.Pixel);
            }

            return newImage;
        }

        private static void ComputeRealDestSize(int originalWidth, int originalHeight, ref int width, ref int height, bool cutToFill)
        {
            if (width == 0 && height == 0)
                width = originalWidth;

            if (cutToFill)
            {
                if (width > originalWidth || height > originalHeight)
                {
                    width = originalWidth;
                    height = originalHeight;

                    return;
                }

                double w = width;
                double h = height;
                double p = w / h;

                double ow = originalWidth;
                double oh = originalHeight;
                double op = ow / oh;

                double w1 = 1;
                double h1 = 1;

                if (op > p)
                {
                    h1 = h;
                    w1 = ow * (h / oh);
                }
                else
                {
                    w1 = w;
                    h1 = oh * (w / ow);
                }

                width = (int)w1;
                height = (int)h1;
            }
            else
            {
                if (width > originalWidth && height > originalHeight)
                {
                    width = originalWidth;
                    height = originalHeight;

                    return;
                }

                double w = width;
                double h = height;
                double p = w / h;

                double ow = originalWidth;
                double oh = originalHeight;
                double op = ow / oh;

                double w1 = 1;
                double h1 = 1;

                if (op < p)
                {
                    h1 = h;
                    w1 = ow * (h / oh);
                }
                else
                {
                    w1 = w;
                    h1 = oh * (w / ow);
                }

                width = (int)w1;
                height = (int)h1;
            }
        }
    }
}
