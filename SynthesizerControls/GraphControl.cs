using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace ErnstTech.SynthesizerControls
{
	/// <summary>
	/// Summary description for GraphControl.
	/// </summary>
	public class GraphControl : System.Windows.Forms.Control
	{
        BindingList<PointF> _Points = new BindingList<PointF>();
        /// <summary>
        ///     The list of points to draw.
        /// </summary>
        /// <value>An ordered list of points.</value>
        public IList<PointF> Points
        {
            get { return this._Points; }
        }

        private List<PointF> _ScaledPoints = new List<PointF>(new List<PointF> ());
        /// <summary>
        ///     <see cref="Points"/> scaled, flipped and translated
        ///     to fit within the drawing area.
        /// </summary>
        /// <remarks>
        ///     Provide and X and Y scale-factor.
        /// </remarks>
        /// <value>
        ///     A list of <seealso cref="PointF"/>.
        /// </value>
        protected List<PointF> ScaledPoints
        {
            get { return this._ScaledPoints; }
        }

        /// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public GraphControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

            this._Points.ListChanged += new ListChangedEventHandler(_Points_ListChanged);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
					components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

        void _Points_ListChanged(object sender, ListChangedEventArgs e)
        {
            this.ScalePoints();
            this.Invalidate();
        }

        private void ScalePoints()
        {
            List<PointF> list = new List<PointF>(this.Points.Count);

            if (this.Points.Count == 0)
                goto end;

            PointF pt = this.Points[0];
            float min_x = pt.X;
            float max_x = pt.X;
            float min_y = pt.Y;
            float max_y = pt.Y;

            // Find the extremes
            for (int i = 0; i < this.Points.Count; ++i)
            {
                pt = this.Points[i];
                if (pt.X < min_x)
                    min_x = pt.X;
                else if (pt.X > max_x)
                    max_x = pt.X;

                if (pt.Y < min_y)
                    min_y = pt.Y;
                else if (pt.Y > max_y)
                    max_y = pt.Y;
            }

            float height = Convert.ToSingle(this.Height);
            float x_scale = this.Width / (max_x - min_x);
            float y_scale = -height / (max_y - min_y);

            foreach (PointF point in this.Points)
            {
                list.Add(new PointF(point.X * x_scale, (point.Y * y_scale + height)));
            }

        end:
            this._ScaledPoints = new List<PointF>(list);
        }
    }
}
