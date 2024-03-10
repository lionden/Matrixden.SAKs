#define DebugMode

using Matrixden.SAK.Extensions;
using Matrixden.SwissArmyKnives;
using OpenCvSharp;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;

namespace Matrixden.SAK.OpenCVSharp
{
    public class ImageUtility
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageData"></param>
        /// <param name="points">四个顶点坐标（可以无序）</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Mat RotateThenExtraction(Mat image, params Point2f[] points)
        {
            if (image == default || points == default || points.Length != 4)
            {
                throw new ArgumentException();
            }

            // 对点进行排序，确保按顺时针或逆时针排列
            var sortedPoints = SortPointsClockwise(points);
#if DEBUG
            var r = string.Empty;
            var tp = $"{Path.GetTempPath()}MatrixDen";
            if (!Directory.Exists(tp))
                Directory.CreateDirectory(tp);
#if DebugMode
            r = RandomValueGenerator.GetRandomString();
            var pts = points.Select(s => (Point)s);
            // 计算凸包
            var convexHull = Cv2.ConvexHull(pts);

            // 连接凸包的点形成矩形
            using Mat hull_mat = image.Clone();
            Cv2.Polylines(hull_mat, new Point[][] { convexHull }, true, Scalar.Red, 2, LineTypes.AntiAlias);

            // 显示图像
            hull_mat.SaveImage($"{tp}\\{nameof(RotateThenExtraction)}5.imageWithDetectedRect.hull_mat.{r}.png");
#endif
            using Mat rec_mat = image.Clone();
            Cv2.Rectangle(rec_mat, sortedPoints[0].ToPoint(), sortedPoints[2].ToPoint(), Scalar.Green, 2);
            rec_mat.SaveImage($"{tp}\\{nameof(RotateThenExtraction)}5.imageWithDetectedRect.rec_mat.{r}.png");
#endif

            // 定义目标矩形的宽高
            var width = (float)sortedPoints[0].DistanceTo(sortedPoints[1]);
            var height = (float)sortedPoints[1].DistanceTo(sortedPoints[2]);

            // 定义目标矩形的四个顶点坐标
            Point2f[] targetPoints =
                [
                    new(0, 0),
                    new(width, 0),
                    new(width, height),
                    new(0, height)
                ];

            // 计算透视变换矩阵
            var matrix = Cv2.GetPerspectiveTransform(sortedPoints, targetPoints);

            // 执行透视变换
            Mat warpedImage = new();
            Cv2.WarpPerspective(image, warpedImage, matrix, new Size(width, height));
#if DEBUG
            // 显示抠图结果
            warpedImage.SaveImage($"{tp}\\{nameof(RotateThenExtraction)}6.warpedImage.{r}.png");
#endif

            return warpedImage;
        }

        public static byte[] RotateThenExtraction(byte[] imageData, params Point2f[] points) => RotateThenExtraction(Cv2.ImDecode(imageData, ImreadModes.Color), points).ToBytes();

        public static Point2f[] SortPointsClockwise(Point2f[] points)
        {
            if (points.Length <= 1) return points;

            // 计算每个点相对于第一个点的极坐标角度
            var centerPoint = points[0];
            var sortedPoints = points.Skip(1).OrderBy(p => System.Math.Atan2(p.Y - centerPoint.Y, p.X - centerPoint.X)).ToArray();

            var result = new Point2f[sortedPoints.Length + 1];
            result[0] = centerPoint;
            sortedPoints.For((p, i) =>
            {
                result[i + 1] = p;
            });

            return result;
        }
    }

    public static class CVExtensions
    {
        public static Point[] Sort(this Point2f[] points)
        {
            return ImageUtility.SortPointsClockwise(points).Select(p => p.ToPoint()).ToArray();
        }
    }
}
