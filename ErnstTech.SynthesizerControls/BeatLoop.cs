using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace ErnstTech.SynthesizerControls
{
	/// <summary>
	/// Summary description for BeatLoop.
	/// </summary>
	[Designer( typeof( BeatLoop.BeatLoopDesigner ) ) ]
	public class BeatLoop : System.Windows.Forms.Control
	{
		public static readonly Color DefaultOnColor = Color.Green;
		public static readonly Color DefaultOffColor = Color.Transparent;
		public static readonly Color DefaultHalfColor = Color.Orange;
		public static readonly Color DefaultBorderColor = Color.Black;

		public class BeatLoopDesigner : System.Windows.Forms.Design.ControlDesigner
		{
		}

		private BeatCollection _Beats = new BeatCollection();
		protected virtual BeatCollection Beats
		{
			get{ return _Beats; }
		}
		
		#region BeatCount Property
		private int _BeatCount = 16;
		public int BeatCount
		{
			get{ return _BeatCount; }
			set
			{
				if ( value < 1 )
					throw new ArgumentOutOfRangeException( "value", value, "BeatCount must be a positive integer." );

				if ( this._BeatCount == value )
					return;

				this._BeatCount = value;
				this.OnBeatCountChanged( EventArgs.Empty );
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
		#endregion

		#region BeatWidth Property
		private int _BeatWidth;
		public int BeatWidth
		{
			get{ return this._BeatWidth; }
		}

		private void SetBeatWidth()
		{
			int width = this.Width;

			width -= ( this.BeatCount * ( this.BorderWidth - 1 + this.BeatSpacing ) );
			width /= this.BeatCount;

			if ( width == this._BeatWidth )
				return;

			this._BeatWidth = width;
			this.OnBeatWidthChanged( EventArgs.Empty );
		}

		#region BeatWidthChanged Event
		private static readonly object BeatWidthChangedEvent = new object();

		/// <summary>
		///		Raised when <see cref="BeatWidth"/> has changed.
		/// </summary>
		public event EventHandler BeatWidthChanged
		{
			add{ this.Events.AddHandler( BeatWidthChangedEvent, value ); }
			remove{ this.Events.RemoveHandler( BeatWidthChangedEvent, value ); }
		}

		/// <summary>
		///		Raises the <see cref="BeatWidthChanged"/> event.
		/// </summary>
		/// <param name="arguments">
		///		Event arguments.
		/// </param>
		protected virtual void OnBeatWidthChanged( EventArgs arguments )
		{
			EventHandler handler = this.Events[BeatWidthChangedEvent] as EventHandler;
			if ( handler != null )
				handler( this, arguments );
		}
		#endregion
		#endregion

		#region BorderWidth Property
		private int _BorderWidth = 1;
		public int BorderWidth
		{
			get{ return this._BorderWidth; }
			set
			{
				if ( value < 0 )
					value = 0;

				if ( this._BorderWidth == value )
					return;

				this._BorderWidth = value;
				this.OnBorderWidthChanged( EventArgs.Empty	);
			}
		}
		#region BorderWidthChanged Event
		private static readonly object BorderWidthChangedEvent = new object();

		public event EventHandler BorderWidthChanged
		{
			add{ this.Events.AddHandler( BorderWidthChangedEvent, value ); }
			remove{ this.Events.RemoveHandler( BorderWidthChangedEvent, value ); }
		}

		protected virtual void OnBorderWidthChanged( EventArgs arguments )
		{
			EventHandler handler = this.Events[BorderWidthChangedEvent] as EventHandler;
			if ( handler != null )
				handler( this, arguments );
		}
		#endregion
		#endregion

		#region BeatSpacing Property
		private int _BeatSpacing = -1;
		/// <summary>
		///		The number of pixels of padding to place between <see cref="Beat"/> controls.
		/// </summary>
		public int BeatSpacing
		{
			get{ return _BeatSpacing; }
			set
			{
				if ( this._BeatSpacing == value )
					return;

				_BeatSpacing = value;
				this.OnBeatSpacingChanged( EventArgs.Empty );
			}
		}

		#region BeatSpacingChanged Event
		private static readonly object BeatSpacingChangedEvent = new object();

		/// <summary>
		///		Raised when <see cref="BeatSpacing"/> has changed.
		/// </summary>
		public event EventHandler BeatSpacingChanged
		{
			add{ this.Events.AddHandler( BeatSpacingChangedEvent, value ); }
			remove{ this.Events.RemoveHandler( BeatSpacingChangedEvent, value ); }
		}

		/// <summary>
		///		Raises the <see cref="BeatSpacingChanged"/> event.
		/// </summary>
		/// <param name="arguments">
		///		Event arguments.
		/// </param>
		protected virtual void OnBeatSpacingChanged( EventArgs arguments )
		{
			EventHandler handler = this.Events[BeatSpacingChangedEvent] as EventHandler;
			if ( handler != null )
				handler( this, arguments );
		}
		#endregion
		#endregion

		private Color _OnColor = DefaultOnColor;
		[Category("Appearance")]
		public Color OnColor
		{
			get{ return _OnColor; }
			set
			{
				if ( object.Equals( this._OnColor, value ) )
					return;
				
				this._OnColor = value;
				this.OnOnColorChanged( EventArgs.Empty );
			}
		}

		#region OnColorChanged Event
		private static readonly object OnColorChangedEvent = new object();

		public event EventHandler OnColorChanged
		{
			add{ this.Events.AddHandler( OnColorChangedEvent, value ); }
			remove{ this.Events.RemoveHandler( OnColorChangedEvent, value ); }
		}

		protected virtual void OnOnColorChanged( EventArgs arguments )
		{
			EventHandler handler = this.Events[OnColorChangedEvent] as EventHandler;
			if ( handler != null )
				handler( this, arguments );
		}
		#endregion
		
		private Color _HalfColor = DefaultHalfColor;
		[Category("Appearance")]
		public Color HalfColor
		{
			get{ return _HalfColor; }
			set
			{
				if ( object.Equals( this._HalfColor, value ) )
					return;

				this._HalfColor = value;
				this.OnHalfColorChanged( EventArgs.Empty );
			}
		}

		#region HalfColorChanged Event
		private static readonly object HalfColorChangedEvent = new object();

		public event EventHandler HalfColorChanged
		{
			add{ this.Events.AddHandler( HalfColorChangedEvent, value ); }
			remove{ this.Events.RemoveHandler( HalfColorChangedEvent, value ); }
		}

		protected virtual void OnHalfColorChanged( EventArgs arguments )
		{
			EventHandler handler = this.Events[HalfColorChangedEvent] as EventHandler;
			if ( handler != null )
				handler( this, arguments );
		}
		#endregion

		private Color _OffColor = DefaultOffColor;
		[Category("Appearance")]
		public Color OffColor
		{
			get{ return _OffColor; }
			set
			{
				if ( object.Equals( this._OffColor, value ) )
					return;

				this._OffColor = value;
				this.OnOffColorChanged( EventArgs.Empty );
			}
		}

		#region OffColorChanged Event
		private static readonly object OffColorChangedEvent = new object();

		public event EventHandler OffColorChanged
		{
			add{ this.Events.AddHandler( OffColorChangedEvent, value ); }
			remove{ this.Events.RemoveHandler( OffColorChangedEvent, value ); }
		}

		protected virtual void OnOffColorChanged( EventArgs arguments )
		{
			EventHandler handler = this.Events[OffColorChangedEvent] as EventHandler;
			if ( handler != null )
				handler( this, arguments );
		}
		#endregion

		private Color _BorderColor = DefaultBorderColor;
		[Category("Appearance")]
		public Color BorderColor
		{
			get{ return _BorderColor; }
			set
			{
				if ( object.Equals( this._BorderColor, value ) )
					return;

				_BorderColor = value;
				this.OnBorderColorChanged(EventArgs.Empty);
			}
		}

		#region BorderColorChanged Event
		private static readonly object BorderColorChangedEvent = new object();

		public event EventHandler BorderColorChanged
		{
			add{ this.Events.AddHandler( BorderColorChangedEvent, value ); }
			remove{ this.Events.RemoveHandler( BorderColorChangedEvent, value ); }
		}

		protected virtual void OnBorderColorChanged( EventArgs arguments )
		{
			EventHandler handler = this.Events[BorderColorChangedEvent] as EventHandler;
			if ( handler != null )
				handler( this, arguments );
		}
		#endregion

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		protected override Size DefaultSize
		{
			get
			{
				int width = this.BeatCount * ( this.BeatSpacing + this.BeatWidth ) + this.BeatSpacing ;
				int height = 2 * this.BeatSpacing + this.BeatWidth;

				return new Size( width, height );
			}
		}

		public BeatLoop()
		{
			this.ControlAdded += new ControlEventHandler(BeatLoop_ControlAdded);

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			this.ResizeBeatsCollection();

			this.BorderColorChanged += new EventHandler(BeatLoop_ColorChanged);
			this.OnColorChanged += new EventHandler(BeatLoop_ColorChanged);
			this.OffColorChanged += new EventHandler(BeatLoop_ColorChanged);
			this.HalfColorChanged += new EventHandler(BeatLoop_ColorChanged);
			this.SizeChanged += new EventHandler(BeatLoop_SizeChanged);
			this.BeatSpacingChanged += new EventHandler(BeatLoop_BeatSpacingChanged);
			this.BeatCountChanged += new EventHandler(BeatLoop_BeatCountChanged);
			this.BeatWidthChanged += new EventHandler(BeatLoop_BeatWidthChanged);
			this.BorderWidthChanged += new EventHandler(BeatLoop_BorderWidthChanged);

			this.BorderWidth = 1;
			
			this.PositionControls();
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

		private void BeatLoop_ControlAdded(object sender, ControlEventArgs e)
		{
			if ( !( e.Control is Beat ) )
				throw new ApplicationException( "BeatLoop can only host Beat controls." );
			if ( this.Beats.Count > this.BeatCount )
				throw new ApplicationException( "BeatLoop cannot host more controls than BeatCount." );
		}

		private void ResizeBeatsCollection()
		{
			int oldSize = this.Beats.Count;
			int newSize = this.BeatCount;

			if ( oldSize == newSize )
				return;

			if ( oldSize > newSize )
			{
				for( int i = oldSize - 1; i >= newSize; --i )
				{
					Beat b = this.Beats[i];

					this.Controls.Remove( b );
					this.Beats.RemoveAt( i );

					b.Dispose();
				}
			}
			else
			{
				for( int i = oldSize; i < newSize; ++i )
				{
					Beat b = new Beat( this );

					this.Beats.Add( b );
				}
			}

			this.PositionControls();
		}

		private void PositionControls()
		{
			if ( this.BeatCount <= 0 )
				return;

			Beats[0].Location = new Point( 0, 0 );
			Beats[0].Size = new Size( this.BeatWidth, this.Height );

			int width = 0;

			for ( int i = 1; i < this.BeatCount; i++ )
			{
				width += this.BeatWidth;

				this.Beats[i].Location = new Point( width + this.BeatSpacing, 0 );
				this.Beats[i].Size = new Size( this.BeatWidth, this.Height );
			}
		}

		public BeatState[] GetLoopState()
		{
			BeatState[] ret = new BeatState[ this.BeatCount ];

			for ( int i = 0; i < this.BeatCount; i++ )
				ret[i] = Beats[i].State;

			return ret;
		}

		public void SetLoopState( BeatState[] state )
		{
			if ( state == null )
				throw new ArgumentNullException( "state" );

			if ( state.Length != this.BeatCount )
				throw new ArgumentOutOfRangeException( "state", state.Length, "State length must equal BeatCount." );

			for( int i = 0; i < this.BeatCount; i++ )
				this.Beats[i].State = state[i];

			this.Refresh();
		}

		public void SetLoopState( byte[] state )
		{
			if ( state == null )
				throw new ArgumentNullException( "state" );

			if ( state.Length != this.BeatCount )
				throw new ArgumentOutOfRangeException( "state", state.Length, "State length must equal BeatCount." );

			for( int i = 0; i < this.BeatCount; i++ )
				this.Beats[i].State = (BeatState)state[i];

			this.Refresh();
		}

		private void BeatLoop_ColorChanged(object sender, EventArgs e)
		{
			this.Invalidate();
		}

		private void BeatLoop_SizeChanged(object sender, EventArgs e)
		{
			this.SetBeatWidth();
			this.Invalidate();
		}

		private void BeatLoop_BeatSpacingChanged(object sender, EventArgs e)
		{
			this.SetBeatWidth();
			this.Invalidate();
		}

		private void BeatLoop_BeatCountChanged(object sender, EventArgs e)
		{
			this.ResizeBeatsCollection();
		}

		private void BeatLoop_BeatWidthChanged(object sender, EventArgs e)
		{
			this.PositionControls();
		}

		private void BeatLoop_BorderWidthChanged(object sender, EventArgs e)
		{
			this.PositionControls();
		}
	}
}
