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
        #region -- System.Draw.Point --

        /// <summary>
        /// Make a offset of the current object.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="x">X-axis offset</param>
        /// <param name="y">Y-axis offset</param>
        /// <returns></returns>
        public static MPoint Offset(this Point @this, double x, double y) => new MPoint(@this).Offset(x, y);

        /// <summary>
        /// Make a offset with single value of the current object when the x and y-axis are equal.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="val">The offset value</param>
        /// <returns></returns>
        public static MPoint Offset(this Point @this, double val) => new MPoint(@this).Offset(val);

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
