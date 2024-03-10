using System.Drawing;
using System.Windows.Forms;

namespace Matrixden.SAK.Controls.WinForm
{
    public class MProgressBar : ProgressBar
    {
        public MProgressBar()
        {
            SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rect = ClientRectangle;
            Graphics g = e.Graphics;

            ProgressBarRenderer.DrawHorizontalBar(g, rect);
            rect.Width = (int)(rect.Width * ((double)Value / Maximum)) - 4;
            rect.Height -= 4;
            g.FillRectangle(new SolidBrush(ForeColor), 2, 2, rect.Width, rect.Height);

            // 绘制进度值
            string progressText = $"{Value}%";
            SizeF textSize = g.MeasureString(progressText, Font);
            PointF textLocation = new PointF((Width - textSize.Width) / 2, (Height - textSize.Height) / 2);
            g.DrawString(progressText, Font, Brushes.Black, textLocation);
        }
    }
}
