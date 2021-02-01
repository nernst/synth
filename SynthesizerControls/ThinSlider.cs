using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace ErnstTech.SynthesizerControls
{
	/// <summary>
	/// Summary description for ThinSlider.
	/// </summary>
	public class ThinSlider : System.Windows.Forms.Control
	{
        /// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        private Orientation _Orientation;
        public Orientation Orientation
        {
            get { return this._Orientation; }
            set
            {
                switch( value )
                {
                    case Orientation.Horizontal:
                    case Orientation.Vertical:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("value", value, "The specified value is not a valid Orientation.");
                }

                this._Orientation = value;
                this.OnOrientationChanged(EventArgs.Empty);
            }
        }

        #region OrientationChanged Event
        private static readonly object OrientationChangedEvent = new object();

        /// <summary>
        ///     Raised when <see cref="Orientation"/> has changed.
        /// </summary>
        /// <value>
        ///     The delegate to add or remove as an event handler.
        /// </value>
        public event EventHandler OrientationChanged
        {
            add { this.Events.AddHandler(OrientationChangedEvent, value); }
            remove { this.Events.RemoveHandler(OrientationChangedEvent,value); }
        }

        /// <summary>
        ///     Raises the <see cref="OrientationChanged"/> event.
        /// </summary>
        /// <param name="arguments">
        ///     Event arguments.
        /// </param>
        protected virtual void OnOrientationChanged(EventArgs arguments)
        {
            EventHandler handler = this.Events[OrientationChangedEvent] as EventHandler;
            if (handler != null)
                handler(this, arguments);
        }
        #endregion

        private int _Minimum;
		private int _Maximum = 100;
		private int _Value;

		public int Minimum
		{
			get{ return this._Minimum; }
			set
			{
				if ( this._Minimum != value )
				{
					this._Minimum = value;
					this.OnMinimumChanged( EventArgs.Empty );
				}
			}
		}

		#region MinimumChanged Event
		private static readonly object MinimumChangedEvent = new object();

		public event EventHandler MinimumChanged
		{
			add{ this.Events.AddHandler( MinimumChangedEvent, value ); }
			remove{ this.Events.RemoveHandler( MinimumChangedEvent, value ); }
		}

		protected virtual void OnMinimumChanged( EventArgs arguments )
		{
			EventHandler handler = this.Events[MinimumChangedEvent] as EventHandler;
			if ( handler != null )
				handler( this, arguments );
		}
		#endregion


		public int Maximum
		{
			get{ return this._Maximum; }
			set
			{
				if ( this._Maximum != value )
				{
					this._Maximum = value; 
					this.OnMaximumChanged( EventArgs.Empty );
				}
			}
		}

		#region MaximumChanged Event
		private static readonly object MaximumChangedEvent = new object();

		public event EventHandler MaximumChanged
		{
			add{ this.Events.AddHandler( MaximumChangedEvent, value ); }
			remove{ this.Events.RemoveHandler( MaximumChangedEvent, value ); }
		}

		protected virtual void OnMaximumChanged( EventArgs arguments )
		{
			EventHandler handler = this.Events[MaximumChangedEvent] as EventHandler;
			if ( handler != null )
				handler( this, arguments );
		}
		#endregion

		private double _SliderScale;
		public double SliderScale
		{
			get{ return this._SliderScale; }
		}

		public int Value
		{
			get{ return this._Value; }
			set
			{
				if ( value < this.Minimum )
					value = this.Minimum;
				else if ( value > this.Maximum )
					value = this.Maximum;

				if ( this._Value != value )
				{
					this._Value = value;
					this.OnValueChanged( EventArgs.Empty );
				}
			}
		}

		private int _Range;
		public int Range
		{
			get{ return this._Range; }
		}

		#region ValueChanged Event
		private static readonly object ValueChangedEvent = new object();

		public event EventHandler ValueChanged
		{
			add{ this.Events.AddHandler( ValueChangedEvent, value ); }
			remove{ this.Events.RemoveHandler( ValueChangedEvent, value ); }
		}

		protected virtual void OnValueChanged( EventArgs arguments )
		{
			EventHandler handler = this.Events[ValueChangedEvent] as EventHandler;
			if ( handler != null )
				handler( this, arguments );
		}
		#endregion

		public ThinSlider()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.MinimumChanged += new EventHandler(RangeChanged);
			this.MaximumChanged += new EventHandler(RangeChanged);
			this.ValueChanged += new EventHandler(ThinSlider_ValueChanged);
			this.SizeChanged += new EventHandler(ThinSlider_SizeChanged);
			this.MouseMove += new MouseEventHandler(ThinSlider_MouseMove);

			this.CalculateScale();
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

		protected override void OnPaint(PaintEventArgs pe)
		{
			if ( double.IsInfinity( this.SliderScale ) )
				return;

			Graphics g = pe.Graphics;

			Rectangle rect = new Rectangle( 0, 0, this.Width, this.Height );

			using ( Brush b = new SolidBrush( this.BackColor ) )
			{
				g.FillRectangle( b, rect );
			}

			int drawHeight = Convert.ToInt32( this.SliderScale * ( this.Value - this.Minimum ) );
			rect.Y = this.Height - drawHeight;
			rect.Height = drawHeight;

			using ( Brush b = new SolidBrush( this.ForeColor ) )
			{
				g.FillRectangle( b, rect );
			}
		}

		private void ThinSlider_ValueChanged(object sender, EventArgs e)
		{
			this.Invalidate();
		}

		private void RangeChanged(object sender, EventArgs e)
		{
			this.CalculateScale();
		}

		private void CalculateScale()
		{
			this._Range = this.Maximum - this.Minimum;
			this._SliderScale = Convert.ToDouble( this.Range ) / 
				Convert.ToDouble( this.Height );
		}

		private void ThinSlider_SizeChanged(object sender, EventArgs e)
		{
			this.CalculateScale();
		}

		private void ThinSlider_MouseMove(object sender, MouseEventArgs e)
		{
			if ( e.Button == MouseButtons.Left )
			{
				int pos = this.Range - e.Y;
				this.Value = Convert.ToInt32( this.SliderScale * pos );
			}
		}
	}
}
