#region Using directives

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

#endregion

namespace ErnstTech.SynthesizerControls
{
    public class LineGraph : GraphControl
    {
        public LineGraph()
        {
            this.BackColor = Color.White;
            this.ForeColor = Color.Black;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (Brush b = new SolidBrush(this.BackColor))
            {
                e.Graphics.FillRectangle(b, e.ClipRectangle);
            }

            if (this.ScaledPoints.Count < 2)
                return;

            PointF[] points = new PointF[this.ScaledPoints.Count];
            this.ScaledPoints.CopyTo(points, 0);

            using (Pen p = new Pen( this.ForeColor, 1.0f ))
            {
                e.Graphics.DrawLines(p, points);
            }
        }
    }
}
