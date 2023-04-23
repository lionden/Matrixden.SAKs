using Matrixden.Utils.Logging;
using System;
using System.Drawing;

namespace Matrixden.SwissArmyKnives.Windows
{
    /// <summary>
    /// Extension methods for <c>MPoint</c>, <c>System.Windows.Point</c> or <c>System.Draw.Point</c>.
    /// Need to convert <c>System.Windows.Point</c> or <c>System.Draw.Point</c> to <c>MPoint</c>, or roolback.
    /// </summary>
    public static class MPointExtensions
    {
        private static readonly ILog _logger = LogProvider.GetCurrentClassLogger();

        #region -- MPoint --

        /// <summary>
        /// Convert a <c>MPoint</c> object to <c>System.Draw.Point</c>.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="default">The default value when the input is null or any other illegal value.</param>
        /// <returns></returns>
        public static Point ToDPoint(this MPoint @this, Point @default)
        {
            if (@this == null)
                return @default;

            return new Point((int)@this.X, (int)@this.Y);
        }

        /// <summary>
        /// Convert a <c>MPoint</c> object to <c>System.Draw.Point</c>.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static Point ToDPoint(this MPoint @this) => @this.ToDPoint(new Point());

        /// <summary>
        /// Convert a <c>MPoint</c> object to <c>System.Windows.Point</c>.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static System.Windows.Point ToWPoint(this MPoint @this)
        {
            if (@this == null)
                return new System.Windows.Point();

            return new System.Windows.Point(@this.X, @this.Y);
        }

        /// <summary>
        /// Make a offset of the current object.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="x">X-axis offset</param>
        /// <param name="y">Y-axis offset</param>
        /// <returns></returns>
        public static MPoint Off(this MPoint @this, double x, double y)
        {
            if (@this == null)
                return new MPoint(0 - x, 0 - y);

            return new MPoint(@this.X - x, @this.Y - y);
        }

        /// <summary>
        /// Make a offset with single value of the current object when the x and y-axis are equal.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="val">The offset value</param>
        /// <returns></returns>
        public static MPoint Off(this MPoint @this, double val) => @this.Off(val, val);

        /// <summary>
        /// Get the axisymmetric point of given point by line.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public static MPoint AxisymmetricPoint(this MPoint @this, MLine line)
        {
            if (line == null)
                throw new ArgumentNullException("line");

            if ((line.Direction & Directions.XAxis) == Directions.XAxis || (line.Direction & Directions.XAxis_Anti) == Directions.XAxis_Anti)
            {
                return @this.Off(0, (line.StartPoint.Y - @this.Y) * 2);
            }
            else if ((line.Direction & Directions.YAxis) == Directions.YAxis || (line.Direction & Directions.YAxis_Anti) == Directions.YAxis_Anti)
            {
                return @this.Off((line.StartPoint.X - @this.X) * 2, 0);
            }
            else
                throw new NotImplementedException("not support titl line.");
        }

        /// <summary>
        /// Get the axisymmetric point of given rectangle's cross line.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="rect"></param>
        /// <param name="direct"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public static MPoint AxisymmetricPoint(this MPoint @this, Rectangle rect, Directions direct)
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

            return @this.AxisymmetricPoint(line);
        }

        #endregion

        #region -- System.Draw.Point --

        /// <summary>
        /// Make a offset of the current object.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="x">X-axis offset</param>
        /// <param name="y">Y-axis offset</param>
        /// <returns></returns>
        public static MPoint Off(this Point @this, double x, double y) => new MPoint(@this).Off(x, y);

        /// <summary>
        /// Make a offset with single value of the current object when the x and y-axis are equal.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="val">The offset value</param>
        /// <returns></returns>
        public static MPoint Off(this Point @this, double val) => new MPoint(@this).Off(val);

        /// <summary>
        /// Get the axisymmetric point of given point by line.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public static MPoint AxisymmetricPoint(this Point @this, MLine line) => ((MPoint)@this).AxisymmetricPoint(line);

        /// <summary>
        /// Get the axisymmetric point of given rectangle's cross line.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="rect"></param>
        /// <param name="direct"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public static MPoint AxisymmetricPoint(this Point @this, Rectangle rect, Directions direct) => ((MPoint)@this).AxisymmetricPoint(rect, direct);

        #endregion
    }
}
