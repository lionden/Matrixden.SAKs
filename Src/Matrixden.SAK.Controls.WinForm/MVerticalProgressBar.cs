using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Matrixden.SAK.Controls.WinForm
{
    /// <summary>
    /// Vertical progress bar.
    /// </summary>
    public class MVerticalProgressBar : ProgressBar
    {
        public MVerticalProgressBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Debug.WriteLine($"{Value}");
            Rectangle rect = ClientRectangle;
            Graphics g = e.Graphics;

            ProgressBarRenderer.DrawVerticalBar(g, rect);
            rect.Height = (int)(rect.Height * ((double)Value / Maximum)) - 4;
            rect.Width -= 4;
            g.FillRectangle(new SolidBrush(ForeColor), 2, ClientRectangle.Bottom - rect.Height - 2, rect.Width, rect.Height);

            // 绘制进度值
            string progressText = $"{Value}";
            SizeF textSize = g.MeasureString(progressText, Font);
            PointF textLocation = new(Width / 2 - textSize.Width / 2, (Height - textSize.Height) / 2);
            g.DrawString(progressText, Font, Brushes.Black, textLocation);
        }
    }
}
