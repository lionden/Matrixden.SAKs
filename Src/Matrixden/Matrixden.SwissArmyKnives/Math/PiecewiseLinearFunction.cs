using Matrixden.SAK.Extensions;
using Matrixden.SwissArmyKnives.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrixden.Math
{
    public class PiecewiseLinearFunction
    {
        public List<MPoint> InflectionPoints { get; private set; }

        public PiecewiseLinearFunction()
        {
            InflectionPoints = new List<MPoint>();
        }

        public double GetX(double y)
        {
            var InflectionPoints_Ordered = InflectionPoints.OrderBy(p => p.Y).ToArray();
            var lst = InflectionPoints_Ordered.Length - 1;
            for (int i = 1; i <= lst - 1; i++)
            {
                if (y <= InflectionPoints_Ordered[i].Y)
                {
                    return InflectionPoints_Ordered[i - 1].X + (y - InflectionPoints_Ordered[i - 1].Y) * (InflectionPoints_Ordered[i].X - InflectionPoints_Ordered[i - 1].X) / (InflectionPoints_Ordered[i].Y - InflectionPoints_Ordered[i - 1].Y);
                }
            }

            return InflectionPoints_Ordered[lst - 1].X + (y - InflectionPoints_Ordered[lst - 1].Y) * (InflectionPoints_Ordered[lst].X - InflectionPoints_Ordered[lst - 1].X) / (InflectionPoints_Ordered[lst].Y - InflectionPoints_Ordered[lst - 1].Y);
        }

        public double GetY(double x)
        {
            var InflectionPoints_Ordered = InflectionPoints.OrderBy(p => p.X).ToArray();
            var lst = InflectionPoints_Ordered.Length - 1;
            for (int i = 1; i <= lst - 1; i++)
            {
                if (x <= InflectionPoints_Ordered[i].X)
                {
                    return InflectionPoints_Ordered[i].Y - (InflectionPoints_Ordered[i].Y - InflectionPoints_Ordered[i - 1].Y) / (InflectionPoints_Ordered[i].X - InflectionPoints_Ordered[i - 1].X) * (InflectionPoints_Ordered[i].X - x);
                }
            }

            return InflectionPoints_Ordered[lst].Y - (InflectionPoints_Ordered[lst].Y - InflectionPoints_Ordered[lst - 1].Y) / (InflectionPoints_Ordered[lst].X - InflectionPoints_Ordered[lst - 1].X) * (InflectionPoints_Ordered[lst].X - x);
        }
    }
}
