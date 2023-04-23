using System;
using System.Drawing;

namespace Matrixden.SwissArmyKnives.Windows
{
    /// <summary>
    /// Represents an Matrixden 2D point object.
    /// Named as BMW's MPower😀.
    /// </summary>
    public struct MPoint
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
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        /// <summary>
        /// Compares two <see cref="T:Matrixden.SwissArmyKnives.Windows.MPoint" /> structures for equality.
        /// </summary>
        /// <param name="point1">The first point to compare.</param>
        /// <param name="point2">The second point to compare.</param>
        /// <returns></returns>
        public static bool Equals(MPoint point1, MPoint point2)
        {
            if (point1.X.Equals(point2.X))
            {
                return point1.Y.Equals(point2.Y);
            }

            return false;
        }

        /// <summary>
        /// Judge whether the 2 points are equal.
        /// </summary>
        /// <param name="obj">The target object</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is MPoint))
                return false;

            var pnt = (MPoint)obj;
            return Equals(this, pnt);
        }

        /// <summary>Compares two <see cref="T:Matrixden.SwissArmyKnives.Windows.MPoint" /> structures for equality.</summary>
        /// <returns>true if both <see cref="T:Matrixden.SwissArmyKnives.Windows.MPoint" /> structures contain the same <see cref="P:Matrixden.SwissArmyKnives.Windows.MPoint.X" /> and <see cref="P:Matrixden.SwissArmyKnives.Windows.MPoint.Y" /> values; otherwise, false.</returns>
        /// <param name="value">The point to compare to this instance.</param>
        public bool Equals(MPoint value)
        {
            return Equals(this, value);
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

        /// <summary>
        /// Convert a <c>MPoint</c> object to <c>System.Draw.Point</c>.
        /// </summary>
        /// <param name="default">The default value when the input is null or any other illegal value.</param>
        /// <returns></returns>
        public Point ToDPoint(Point @default)
        {
            if (this == null)
                return @default;

            return new Point((int)this.X, (int)this.Y);
        }

        /// <summary>
        /// Convert a <c>MPoint</c> object to <c>System.Draw.Point</c>.
        /// </summary>
        /// <returns></returns>
        public Point ToDPoint() => this.ToDPoint(new Point());

        /// <summary>
        /// Convert a <c>MPoint</c> object to <c>System.Windows.Point</c>.
        /// </summary>
        /// <returns></returns>
        public System.Windows.Point ToWPoint()
        {
            if (this == null)
                return new System.Windows.Point();

            return new System.Windows.Point(this.X, this.Y);
        }

        /// <summary>
        /// Make a offset of the current object.
        /// </summary>
        /// <param name="x">X-axis offset</param>
        /// <param name="y">Y-axis offset</param>
        /// <returns></returns>
        public MPoint Offset(double x, double y)
        {
            if (this == null)
                return new MPoint(0 - x, 0 - y);

            return new MPoint(this.X - x, this.Y - y);
        }

        /// <summary>
        /// Make a offset with single value of the current object when the x and y-axis are equal.
        /// </summary>
        /// <param name="val">The offset value</param>
        /// <returns></returns>
        public MPoint Offset(double val) => this.Offset(val, val);

        /// <summary>
        /// Get the axisymmetric point of given point by line.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public MPoint AxisymmetricPoint(MLine line)
        {
            if (line == null)
                throw new ArgumentNullException("line");

            if ((line.Direction & Directions.XAxis) == Directions.XAxis || (line.Direction & Directions.XAxis_Anti) == Directions.XAxis_Anti)
            {
                return this.Offset(0, (line.StartPoint.Y - this.Y) * 2);
            }
            else if ((line.Direction & Directions.YAxis) == Directions.YAxis || (line.Direction & Directions.YAxis_Anti) == Directions.YAxis_Anti)
            {
                return this.Offset((line.StartPoint.X - this.X) * 2, 0);
            }
            else
                throw new NotImplementedException("not support titl line.");
        }

        /// <summary>
        /// Get the axisymmetric point of given rectangle's cross line.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="direct"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public MPoint AxisymmetricPoint(Rectangle rect, Directions direct)
        {
            if (rect == null)
                throw new ArgumentNullException("rect");

            MLine line;
            switch (direct)
            {
                case Directions.XAxis:
                case Directions.XAxis_Anti:
                case Directions.XParalle:
                    line = new MLine(new MPoint(rect.X, rect.Y + rect.Height / 2), Directions.XParalle, rect.Width);
                    break;

                case Directions.YAxis:
                case Directions.YAxis_Anti:
                case Directions.YParalle:
                    line = new MLine(new MPoint(rect.X + rect.Width / 2, rect.Y), Directions.YParalle, rect.Height);
                    break;
                default:
                    throw new NotImplementedException("not support titl line.");
            }

            return this.AxisymmetricPoint(line);
        }
    }
}
