using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace ErnstTech.SynthesizerControls
{
	/// <summary>
	/// Summary description for Beat.
	/// </summary>
	public class Beat : System.Windows.Forms.Control
	{
		protected BeatLoop BeatLoop
		{
			get{ return this.Parent as BeatLoop; }
		}

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private BeatState _State = BeatState.Off;
		public BeatState State
		{
			get{ return _State; }
			set
			{ 
				switch(value)
				{
					case BeatState.Half:
					case BeatState.Off:
					case BeatState.On:
						break;
					default:
						throw new ArgumentOutOfRangeException( "value", value, "Value is does not correspond to a valid BeatState enum value." );
				}

				_State = value;
			}
		}

		protected override Size DefaultSize
		{
			get
			{
				return new Size( 20, 20 );
			}
		}

		/// <summary>
		///		This constructor ensures that we are only ever contained
		///		by a BeatLoop.
		/// </summary>
		/// <param name="parent">
		///		The parent <see cref="BeatLoop"/> control.
		///		
		///		Cannot be null.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///		Thrown when <see cref="parent"/> is null.
		/// </exception>
		internal Beat( BeatLoop parent )
		{
			if ( parent == null )
				throw new ArgumentNullException( "parent" );

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.MouseDown += new MouseEventHandler(Beat_MouseDown);
			parent.Controls.Add( this );
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
			Rectangle r = new Rectangle( 0, 0, this.Width - 1, this.Height - 1 );

			using( Brush b = new SolidBrush( this.GetFillColor() ) )
			{
				pe.Graphics.FillRectangle( b, r );
			}

			int border = this.BeatLoop.BorderWidth;

			if ( border > 0 )
			{
				using( Pen p = new Pen( this.BeatLoop.BorderColor, Convert.ToSingle( border ) ) )
				{
					pe.Graphics.DrawRectangle( p, r );
				}
			}
			// Calling the base class OnPaint
			base.OnPaint(pe);
		}

		protected virtual Color GetFillColor()
		{
			switch(this.State)
			{
				case BeatState.On:
					return this.BeatLoop.OnColor;
				case BeatState.Off:
					return this.BeatLoop.OffColor;
				case BeatState.Half:
					return this.BeatLoop.HalfColor;
				default:
					throw new IndexOutOfRangeException( "State has an unknown or unsupported value." );
			}
		}

		private void Beat_ParentChanged(object sender, EventArgs e)
		{
			if ( this.Parent != null )
			{
				if ( !( this.Parent is BeatLoop ) )
					throw new InvalidOperationException( "Beat control may only be contained by a BeatLoop." );

				this.Height = this.Parent.Height;
			}
		}

		private void Beat_MouseDown(object sender, MouseEventArgs e)
		{
			switch( this.State )
			{
				case BeatState.Off:
					this.State = BeatState.Half;
					break;
				case BeatState.Half:
					this.State = BeatState.On;
					break;
				case BeatState.On:
					this.State = BeatState.Off;
					break;
				default:
					this.State = BeatState.Off;
					break;
			}

			this.Refresh();
		}
	}
}
