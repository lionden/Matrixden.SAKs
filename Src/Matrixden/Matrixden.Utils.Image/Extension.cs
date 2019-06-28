using Matrixden.Utils.Image;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using S=System.Drawing;

namespace Matrixden.Utils.Extensions
{
    /// <summary>
    /// Extended methods
    /// </summary>
    public static class Extension
    {
        /// <summary>
        /// 缩放调整图片尺寸
        /// </summary>
        /// <param name="src">原始图片</param>
        /// <param name="size">目标尺寸</param>
        /// <returns></returns>
        public static S.Image Resize(this S.Image src, Size size) => MatImage.Resize(src, size);
    }
}
