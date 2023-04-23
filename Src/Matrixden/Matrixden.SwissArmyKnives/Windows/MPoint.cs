using System;
using System.Drawing;

namespace Matrixden.SwissArmyKnives.Windows
{
    /// <summary>
    /// Represents an Matrixden 2D point object.
    /// Named as BMW's MPower😀.
    /// </summary>
    public class MPoint
    {
        /// <summary>
        /// X-axis value
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y-axis value
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Init a MPoint object.
        /// </summary>
        /// <param name="x">X-axis value</param>
        /// <param name="y">Y-axis value</param>
        public MPoint(double x, double y)
        {
            this.X = x; this.Y = y;
        }

        /// <summary>
        /// Init a MPoint object with a single value when the x and y-axis are equal.
        /// </summary>
        /// <param name="coord">X or y-axis value</param>
        public MPoint(double coord) : this(coord, coord) { }

        /// <summary>
        /// Init a MPoint object with a <c>System.Draw.Point</c> object.
        /// </summary>
        /// <param name="point"></param>
        public MPoint(Point point)
        {
            if (point == null)
                X = Y = 0;
            else
                X = point.X; Y = point.Y;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Judge whether the 2 points are equal.
        /// </summary>
        /// <param name="obj">The target object</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return this == null;

            if (this == null) return false;

            if (!(obj is MPoint))
                return false;

            var pnt = (MPoint)obj;
            return this.X == pnt.X && this.Y == pnt.Y;
        }

        /// <summary>
        /// Judge whether the 2 points are equal.
        /// </summary>
        /// <param name="left">The 1st point</param>
        /// <param name="right">The 2nd point</param>
        /// <returns></returns>
        public static bool operator ==(MPoint left, MPoint right) => left.Equals(right);

        /// <summary>
        /// Judge whether the 2 points are not equal.
        /// </summary>
        /// <param name="left">The 1st point</param>
        /// <param name="right">The 2nd point</param>
        /// <returns></returns>
        public static bool operator !=(MPoint left, MPoint right) => !left.Equals(right);

        /// <summary>
        /// The distance between 2 points.
        /// </summary>
        /// <param name="left">The 1st point</param>
        /// <param name="right">The 2nd point</param>
        /// <returns>The distance</returns>
        public static double operator -(MPoint left, MPoint right)
        {
            if (left == null)
            {
                left = new MPoint(0);
            }

            if (right == null)
            {
                right = new MPoint(0);
            }

            return Math.Sqrt((Math.Pow(right.X - left.X, 2) + Math.Pow(right.Y - left.Y, 2)));
        }

        /// <summary>
        /// Cast a point value to MPoint object.
        /// </summary>
        /// <param name="v"></param>
        public static explicit operator MPoint(Point v) => new MPoint(v);
    }
}
