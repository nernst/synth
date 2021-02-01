using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using ErnstTech.SoundCore;

namespace ErnstTech.SynthesizerControls
{
	/// <summary>
	/// Summary description for WaveFormDialog.
	/// </summary>
	public class WaveFormDialog : System.Windows.Forms.Form
	{
		private WaveFormat _Format;
		public WaveFormat Format
		{
			get{ return _Format; }
			set
			{
				if ( value == null )
					throw new ArgumentNullException( "value" );

				_Format = value;

				FormatChange();
			}
		}

		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private ErnstTech.SynthesizerControls.BeatBox beatBox1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.GroupBox groupBox1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public WaveFormDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.beatBox1 = new ErnstTech.SynthesizerControls.BeatBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
// 
// btnCancel
// 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(540, 362);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "&Cancel";
// 
// btnOK
// 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(460, 362);
            this.btnOK.Name = "btnOK";
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "&Ok";
// 
// beatBox1
// 
            this.beatBox1.BaseFrequency = 200F;
            this.beatBox1.Location = new System.Drawing.Point(69, 67);
            this.beatBox1.Name = "beatBox1";
            this.beatBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.beatBox1.SampleQuality = ErnstTech.SoundCore.AudioBits.Bits16;
            this.beatBox1.Size = new System.Drawing.Size(464, 296);
            this.beatBox1.TabIndex = 2;
            this.beatBox1.Load += new System.EventHandler(this.beatBox1_Load);
// 
// textBox1
// 
            this.textBox1.Location = new System.Drawing.Point(320, 40);
            this.textBox1.Name = "textBox1";
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = "textBox1";
// 
// groupBox1
// 
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Wave Form Name";
// 
// WaveFormDialog
// 
            this.AutoScaleDimensions = new SizeF(5, 13);
            this.ClientSize = new System.Drawing.Size(620, 391);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.beatBox1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "WaveFormDialog";
            this.Text = "Edit Wave Form";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
		#endregion

		private void beatBox1_Load(object sender, System.EventArgs e)
		{
		
		}

		private void FormatChange()
		{

		}
	}
}
