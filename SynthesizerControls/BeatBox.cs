using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using ErnstTech.SoundCore;

namespace ErnstTech.SynthesizerControls
{
	/// <summary>
	/// Summary description for BeatBox.
	/// </summary>
	public class BeatBox : System.Windows.Forms.UserControl
	{
		private int MaxPointHeight
		{
			get
			{
				switch(this.SampleQuality)
				{
					case AudioBits.Bits8:
						return byte.MaxValue;
					case AudioBits.Bits16:
						return ushort.MaxValue;
					default:
						throw new IndexOutOfRangeException( "SampleQuality is out of range." );
				}
			}
		}
		const int MaxPointWidth = 100;

		private System.Windows.Forms.TrackBar tbTotalTime;
		private System.Windows.Forms.Panel pnWaveForm;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ListView lvPoints;
		private System.Windows.Forms.Button btnAddPoint;
		private System.Windows.Forms.Button btnRemovePoints;
		private System.Windows.Forms.TrackBar tbPointWidth;
		private System.Windows.Forms.TrackBar tbPointHeight;
		private System.Windows.Forms.ColumnHeader chIndex;
		private System.Windows.Forms.ColumnHeader chX;
		private System.Windows.Forms.ColumnHeader chY;

//        private BindingList<PointF> _Points = new BindingList<PointF>();
        public IList<PointF> Points
        {
            get { return this.waveGraph.Points; }
        }

		int SelectedPointIndex = -1;

        private AudioBits _SampleQuality = AudioBits.Bits16;
        private LineGraph waveGraph;
        private ThinSlider thinSlider1;

        public AudioBits SampleQuality
		{
			get{ return _SampleQuality; }
			set
			{
				if ( value != AudioBits.Bits16 && value != AudioBits.Bits8 )
					throw new ArgumentOutOfRangeException( "SampleQuality", value, "Sample quality must be 8 or 16 bits." );

				_SampleQuality = value;
			}
		}

		private float _BaseFrequency = 200;
		public float BaseFrequency
		{
			get{ return _BaseFrequency; }
			set
			{
				if ( value <= 0 )
					throw new ArgumentOutOfRangeException( "BaseFrequency", value, "BaseFrequency must be a positive number." );

				_BaseFrequency = value;
			}
		}

		public BeatBox()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.tbPointHeight.Enabled = false;
			this.tbPointHeight.Maximum = MaxPointHeight;
			this.tbPointWidth.Enabled = false;
			this.tbPointWidth.Maximum = MaxPointWidth;

			this.ParentChanged += new EventHandler(BeatBox_ParentChanged);
			lvPoints.SelectedIndexChanged += new EventHandler(lvPoints_SelectedIndexChanged);

			Points.Add( new PointF( 0.0f, 0.0f ) );
			Points.Add( new PointF( (float)(MaxPointWidth / 4.0f), Convert.ToSingle( MaxPointHeight ) ) );
			Points.Add( new PointF( (float)(MaxPointWidth / 2.0f), Convert.ToSingle( MaxPointHeight ) ) );
			Points.Add( new PointF( Convert.ToSingle( MaxPointWidth ), 0.0f) );
			BindPointsView();

			pnWaveForm.Refresh();

			this.btnRemovePoints.Enabled = false;
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
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
            this.tbTotalTime = new System.Windows.Forms.TrackBar();
            this.pnWaveForm = new System.Windows.Forms.Panel();
            this.waveGraph = new ErnstTech.SynthesizerControls.LineGraph();
            this.lvPoints = new System.Windows.Forms.ListView();
            this.chIndex = new System.Windows.Forms.ColumnHeader("");
            this.chX = new System.Windows.Forms.ColumnHeader("");
            this.chY = new System.Windows.Forms.ColumnHeader("");
            this.btnAddPoint = new System.Windows.Forms.Button();
            this.btnRemovePoints = new System.Windows.Forms.Button();
            this.tbPointWidth = new System.Windows.Forms.TrackBar();
            this.tbPointHeight = new System.Windows.Forms.TrackBar();
            this.thinSlider1 = new ErnstTech.SynthesizerControls.ThinSlider();
            ((System.ComponentModel.ISupportInitialize)(this.tbTotalTime)).BeginInit();
            this.pnWaveForm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbPointWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPointHeight)).BeginInit();
            this.SuspendLayout();
// 
// tbTotalTime
// 
            this.tbTotalTime.Location = new System.Drawing.Point(0, 256);
            this.tbTotalTime.Maximum = 100;
            this.tbTotalTime.Name = "tbTotalTime";
            this.tbTotalTime.Size = new System.Drawing.Size(464, 45);
            this.tbTotalTime.TabIndex = 5;
            this.tbTotalTime.TickFrequency = 10;
// 
// pnWaveForm
// 
            this.pnWaveForm.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnWaveForm.BackColor = System.Drawing.Color.Transparent;
            this.pnWaveForm.Controls.Add(this.waveGraph);
            this.pnWaveForm.Location = new System.Drawing.Point(0, 0);
            this.pnWaveForm.Name = "pnWaveForm";
            this.pnWaveForm.Size = new System.Drawing.Size(464, 120);
            this.pnWaveForm.TabIndex = 7;
// 
// waveGraph
// 
            this.waveGraph.BackColor = System.Drawing.Color.White;
            this.waveGraph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.waveGraph.ForeColor = System.Drawing.Color.Black;
            this.waveGraph.Location = new System.Drawing.Point(0, 0);
            this.waveGraph.Name = "waveGraph";
            this.waveGraph.Size = new System.Drawing.Size(464, 120);
            this.waveGraph.TabIndex = 0;
            this.waveGraph.Text = "lineGraph1";
// 
// lvPoints
// 
            this.lvPoints.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chIndex,
            this.chX,
            this.chY});
            this.lvPoints.FullRowSelect = true;
            this.lvPoints.Location = new System.Drawing.Point(0, 120);
            this.lvPoints.MultiSelect = false;
            this.lvPoints.Name = "lvPoints";
            this.lvPoints.Size = new System.Drawing.Size(160, 97);
            this.lvPoints.TabIndex = 8;
            this.lvPoints.View = System.Windows.Forms.View.Details;
// 
// chIndex
// 
            this.chIndex.Text = "Index";
// 
// chX
// 
            this.chX.Text = "X";
// 
// chY
// 
            this.chY.Text = "Y";
// 
// btnAddPoint
// 
            this.btnAddPoint.Location = new System.Drawing.Point(0, 224);
            this.btnAddPoint.Name = "btnAddPoint";
            this.btnAddPoint.TabIndex = 9;
            this.btnAddPoint.Text = "Add";
            this.btnAddPoint.Click += new System.EventHandler(this.btnAddPoint_Click);
// 
// btnRemovePoints
// 
            this.btnRemovePoints.Location = new System.Drawing.Point(80, 224);
            this.btnRemovePoints.Name = "btnRemovePoints";
            this.btnRemovePoints.TabIndex = 10;
            this.btnRemovePoints.Text = "Remove";
            this.btnRemovePoints.Click += new System.EventHandler(this.btnRemovePoints_Click);
// 
// tbPointWidth
// 
            this.tbPointWidth.Location = new System.Drawing.Point(208, 176);
            this.tbPointWidth.Margin = new System.Windows.Forms.Padding(2, 3, 3, 3);
            this.tbPointWidth.Name = "tbPointWidth";
            this.tbPointWidth.TabIndex = 11;
            this.tbPointWidth.TickFrequency = 10;
            this.tbPointWidth.Scroll += new System.EventHandler(this.tbPointWidth_Scroll);
// 
// tbPointHeight
// 
            this.tbPointHeight.Location = new System.Drawing.Point(160, 120);
            this.tbPointHeight.Margin = new System.Windows.Forms.Padding(3, 3, 1, 3);
            this.tbPointHeight.Name = "tbPointHeight";
            this.tbPointHeight.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbPointHeight.Size = new System.Drawing.Size(45, 104);
            this.tbPointHeight.TabIndex = 12;
            this.tbPointHeight.TickFrequency = 10;
            this.tbPointHeight.Scroll += new System.EventHandler(this.tbPointHeight_Scroll);
// 
// thinSlider1
// 
            this.thinSlider1.BackColor = System.Drawing.Color.White;
            this.thinSlider1.ForeColor = System.Drawing.Color.Navy;
            this.thinSlider1.Location = new System.Drawing.Point(340, 143);
            this.thinSlider1.Maximum = 100;
            this.thinSlider1.Minimum = 0;
            this.thinSlider1.Name = "thinSlider1";
            this.thinSlider1.Size = new System.Drawing.Size(10, 74);
            this.thinSlider1.TabIndex = 13;
            this.thinSlider1.Text = "thinSlider1";
            this.thinSlider1.Value = 0;
// 
// BeatBox
// 
            this.Controls.Add(this.thinSlider1);
            this.Controls.Add(this.tbPointHeight);
            this.Controls.Add(this.tbPointWidth);
            this.Controls.Add(this.btnRemovePoints);
            this.Controls.Add(this.btnAddPoint);
            this.Controls.Add(this.lvPoints);
            this.Controls.Add(this.pnWaveForm);
            this.Controls.Add(this.tbTotalTime);
            this.Name = "BeatBox";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Size = new System.Drawing.Size(464, 296);
            ((System.ComponentModel.ISupportInitialize)(this.tbTotalTime)).EndInit();
            this.pnWaveForm.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tbPointWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPointHeight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
		#endregion

		private void BindPointsView()
		{
			this.lvPoints.Items.Clear();

            this.lvPoints.BeginUpdate();

            for( int i = 0, length = Points.Count; i < length; i++ )
			{
				ListViewItem item = new ListViewItem();

				item.Text = i.ToString();
				item.SubItems.Add( Points[i].X.ToString() );
				item.SubItems.Add( Points[i].Y.ToString() );

				lvPoints.Items.Add( item );
			}

            this.lvPoints.EndUpdate();

            foreach (ColumnHeader ch in lvPoints.Columns)
            {
                ch.Width = -3;
            }

            lvPoints.Refresh();
		}

		private void tbPointHeight_Scroll(object sender, System.EventArgs e)
		{
			PointF p = this.Points[this.SelectedPointIndex];
			p.Y = Convert.ToSingle( tbPointHeight.Value );
			this.Points[this.SelectedPointIndex] = p;
			this.BindPointsView();
		}

		private void tbPointWidth_Scroll(object sender, System.EventArgs e)
		{
			PointF p = this.Points[this.SelectedPointIndex];
			p.X = Convert.ToSingle( tbPointWidth.Value );
			this.Points[this.SelectedPointIndex] = p;
			this.BindPointsView();
		}

		private void btnAddPoint_Click(object sender, System.EventArgs e)
		{
			this.Points.Add( new Point( 0, 0 ) );

			BindPointsView();
		}

		private void lvPoints_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ( lvPoints.SelectedIndices.Count <= 0 )
			{
				btnRemovePoints.Enabled = false;
				tbPointHeight.Enabled = false;
				tbPointWidth.Enabled = false;

				this.SelectedPointIndex = -1;
			}
			else
			{
				btnRemovePoints.Enabled = ( this.Points.Count > 3 );

				tbPointHeight.Enabled = true;
				tbPointWidth.Enabled = true;

				this.SelectedPointIndex = lvPoints.SelectedIndices[0];
				
				PointF p = Points[this.SelectedPointIndex];
				tbPointHeight.Value = Convert.ToInt32( p.Y );
				tbPointWidth.Value = Convert.ToInt32( p.X );
			}
		}

		private void btnRemovePoints_Click(object sender, System.EventArgs e)
		{
			Points.RemoveAt( this.SelectedPointIndex );
			this.SelectedPointIndex = -1;
			this.btnRemovePoints.Enabled = false;
			BindPointsView();
		}

		private void BeatBox_ParentChanged(object sender, EventArgs e)
		{
			this.pnWaveForm.Refresh();
		}
	}
}
