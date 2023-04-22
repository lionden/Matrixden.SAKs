using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Matrixden.SwissArmyKnives.Windows
{
    /// <summary>
    /// 
    /// </summary>
    public class MLine
    {
        /// <summary>
        /// Start point
        /// </summary>
        public MPoint StartPoint { get; set; }
        /// <summary>
        /// End point
        /// </summary>
        public MPoint EndPoint { get; set; }

        /// <summary>
        /// Length
        /// </summary>
        public double Length
        {
            get
            {
                return EndPoint - StartPoint;
            }
        }

        private Directions _direct;

        /// <summary>
        /// Direction
        /// </summary>
        public Directions Direction
        {
            get
            {
                if (_direct == default)
                {
                    if (StartPoint.X == EndPoint.X)
                    {
                        _direct = Directions.YParalle;
                    }
                    else if (StartPoint.Y == EndPoint.Y)
                        _direct = Directions.XParalle;
                    else
                        _direct = Directions.Tilt_axial;
                }

                return _direct;
            }
            private set
            {
                _direct = value;
            }
        }

        /// <summary>
        /// [not support now]
        /// 
        /// from x-axis to y-axis.
        /// Define the top-left as start point.
        /// </summary>
        [Obsolete("Not implemented yet", true)]
        public double Angle { get; set; }

        /// <summary>
        /// Init a MLine object with 2 points.
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        public MLine(MPoint startPoint, MPoint endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        /// <summary>
        /// Init a MLine object with 2 points.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public MLine(double x1, double y1, double x2, double y2) : this(new MPoint(x1, y1), new MPoint(x2, y2)) { }

        /// <summary>
        /// Init a MLine object with a start point, direction and length.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="direction"></param>
        /// <param name="length"></param>
        public MLine(MPoint point, Directions direction, double length)
        {
            if (point == null)
                point = new MPoint(0);

            Direction = direction;
            switch (direction)
            {
                case Directions.XAxis:
                    StartPoint = point;
                    EndPoint = point.Off(length, 0);
                    break;
                case Directions.YAxis:
                    StartPoint = point;
                    EndPoint = point.Off(0, length);
                    break;
                case Directions.XAxis_Anti:
                    StartPoint = point;
                    EndPoint = point.Off(length * -1, 0);
                    break;
                case Directions.YAxis_Anti:
                    StartPoint = point;
                    EndPoint = point.Off(0, length * -1);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Represents a line paralle to the X-axis
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public static MLine LineParallelXAxis(double y) => new MLine(double.MinValue, y, double.MaxValue, y);

        /// <summary>
        /// Represents a line paralle to the Y-axis
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static MLine LineParallelYAxis(double x) => new MLine(x, double.MinValue, x, double.MaxValue);
    }

    /// <summary>
    /// Direction enums
    /// </summary>
    [Flags]
    public enum Directions
    {
        /// <summary>
        /// Parallel to the direction of the +x-axis
        /// </summary>
        XAxis = 1,
        /// <summary>
        /// Parallel to the direction of the +y-axis
        /// </summary>
        YAxis = 1 << 1,
        /// <summary>
        /// Antiparallel to the direction of the +x-axis
        /// </summary>
        XAxis_Anti = 1 << 2,
        /// <summary>
        /// Antiparallel to the direction of the +y-axis
        /// </summary>
        YAxis_Anti = 1 << 3,
        /// <summary>
        /// Parallel to x-axis
        /// </summary>
        XParalle = XAxis | XAxis_Anti,
        /// <summary>
        /// Parallel to y-axis
        /// </summary>
        YParalle = YAxis | YAxis_Anti,
        /// <summary>
        /// a tilt line
        /// </summary>
        [Obsolete("Not implemented yet")]
        Tilt_axial = 0xffff
    }
}
