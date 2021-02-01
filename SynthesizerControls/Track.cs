using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ErnstTech.SynthesizerControls
{
	/// <summary>
	/// Summary description for Track.
	/// </summary>
	[Designer(typeof(Track.TrackDesigner))]
	public class Track : System.Windows.Forms.UserControl
	{
		public class TrackDesigner : ControlDesigner
		{
		}

		private int _BeatCount = 16;
		private System.Windows.Forms.Panel panel1;
		private ErnstTech.SynthesizerControls.BeatLoop beatLoop;
	
		public int BeatCount
		{
			get{ return this._BeatCount; }
			set
			{
				if ( value < 1 )
					throw new ArgumentOutOfRangeException( "value", value, "BeatCount must be a positive integer." );

				if ( this._BeatCount != value )
				{
					this._BeatCount = value;
					this.OnBeatCountChanged( EventArgs.Empty );
				}
			}
		}

		#region BeatCountChanged Event
		private static readonly object BeatCountChangedEvent = new object();

		public event EventHandler BeatCountChanged
		{
			add{ this.Events.AddHandler( BeatCountChangedEvent, value ); }
			remove{ this.Events.RemoveHandler( BeatCountChangedEvent, value ); }
		}

		protected virtual void OnBeatCountChanged( EventArgs arguments )
		{
			EventHandler handler = this.Events[BeatCountChangedEvent] as EventHandler;
			if ( handler != null )
				handler( this, arguments );
		}

		#endregion

        private System.Windows.Forms.Label lblName;
		private System.Windows.Forms.LinkLabel lblEdit;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Track()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.BeatCountChanged += new EventHandler(Track_BeatCountChanged);
            this.TextChanged += new EventHandler(Track_TextChanged);
            this.Text = "Track";
            this.OnBeatCountChanged( EventArgs.Empty );
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
			this.lblName = new System.Windows.Forms.Label();
			this.lblEdit = new System.Windows.Forms.LinkLabel();
			this.beatLoop = new ErnstTech.SynthesizerControls.BeatLoop();
			this.panel1 = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// lblName
			// 
			this.lblName.Location = new System.Drawing.Point(0, 0);
			this.lblName.Name = "lblName";
			this.lblName.Size = new System.Drawing.Size(100, 16);
			this.lblName.TabIndex = 0;
			this.lblName.Text = "label1";
			// 
			// lblEdit
			// 
			this.lblEdit.Location = new System.Drawing.Point(96, 0);
			this.lblEdit.Name = "lblEdit";
			this.lblEdit.Size = new System.Drawing.Size(40, 16);
			this.lblEdit.TabIndex = 1;
			this.lblEdit.TabStop = true;
			this.lblEdit.Text = "Edit...";
			this.lblEdit.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblEdit_LinkClicked);
			// 
			// beatLoop
			// 
			this.beatLoop.BackColor = System.Drawing.Color.Black;
			this.beatLoop.BeatCount = 16;
			this.beatLoop.BeatSpacing = 0;
			this.beatLoop.BorderColor = System.Drawing.Color.Black;
			this.beatLoop.BorderWidth = 1;
			this.beatLoop.HalfColor = System.Drawing.Color.Cornsilk;
			this.beatLoop.Location = new System.Drawing.Point(0, 16);
			this.beatLoop.Name = "beatLoop";
			this.beatLoop.OffColor = System.Drawing.Color.Bisque;
			this.beatLoop.OnColor = System.Drawing.Color.Green;
			this.beatLoop.Size = new System.Drawing.Size(177, 12);
			this.beatLoop.TabIndex = 2;
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.Location = new System.Drawing.Point(0, 32);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(176, 112);
			this.panel1.TabIndex = 3;
			// 
			// Track
			// 
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.beatLoop);
			this.Controls.Add(this.lblEdit);
			this.Controls.Add(this.lblName);
			this.Name = "Track";
			this.Size = new System.Drawing.Size(176, 144);
			this.ResumeLayout(false);

		}
		#endregion

		private void lblEdit_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			// TODO:  Show waveform edit
		}

		private void Track_BeatCountChanged(object sender, EventArgs e)
		{
			BeatState[] states = this.beatLoop.GetLoopState();

			BeatState[] newstates = new BeatState[ this.BeatCount ];
			if ( states.Length <= newstates.Length )
			{
				Array.Copy( states, 0, newstates, 0, states.Length );
				for( int i = states.Length; i < newstates.Length; ++i )
					newstates[i] = BeatState.Off;
			}
			else
			{
				Array.Copy( states, 0, newstates, 0, newstates.Length );
			}

			this.beatLoop.BeatCount = this.BeatCount;
			this.beatLoop.SetLoopState( newstates );
			this.SetupSliders();
		}

		private void SetupSliders()
		{
			foreach( Control control in this.panel1.Controls )
			{
				control.Dispose();
			}

			this.panel1.Controls.Clear();

			int count = this.BeatCount;
			int width = this.Width % count - 1;
			const int offset = 1;

			for( int i = 0; i < count; ++i )
			{
				ThinSlider slider = new ThinSlider();
				slider.Width = width;
				slider.Top = this.beatLoop.Bottom + offset;
				slider.Height = 100;
				slider.Left = ( width + offset ) * i + offset;
				this.panel1.Controls.Add( slider );
			}
		}

        void Track_TextChanged(object sender, EventArgs e)
        {
            this.lblName.Text = this.Text;
        }
    }
}
