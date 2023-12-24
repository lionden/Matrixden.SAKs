using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Matrixden.SwissArmyKnives.Windows
{
    /// <summary>
    /// 
    /// </summary>
    public static class MRectangleExtensions
    {

        #region -- System.Draw.Point --

        /// <summary>
        /// Top left corner's location.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static Point TopLeft(this Rectangle @this)
        {
            if (@this == null)
                return new Point();

            return @this.Location;
        }

        /// <summary>
        /// Top right corner's location.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static Point TopRight(this Rectangle @this)
        {
            if (@this == null)
                return new Point();

            return new Point(@this.X + @this.Width, @this.Y);
        }

        /// <summary>
        /// Bottom left corner's location.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static Point BottomLeft(this Rectangle @this)
        {
            if (@this == null)
                return new Point();

            return new Point(@this.X, @this.Y + @this.Height);
        }

        /// <summary>
        /// Bottom right corner's location.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static Point BottomRight(this Rectangle @this)
        {
            if (@this == null)
                return new Point();

            return new Point(@this.X + @this.Width, @this.Y + @this.Height);
        }

        /// <summary>
        /// Center's location.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static Point Center(this Rectangle @this)
        {
            if (@this == null)
                return new Point();

            return new Point(@this.X + @this.Width / 2, @this.Y + @this.Height / 2);
        }

        #endregion
    }
}
