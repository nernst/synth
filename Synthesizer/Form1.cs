using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ErnstTech.SynthesizerControls;

#if ERNST_DX_AUDIO
using ErnstTech.DXSoundCore;
#else
using System.Media;
#pragma warning disable CA1416
#endif

namespace ErnstTech.Synthesizer
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Button btnTestWaveStream;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button btnPlay;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.Button btnCapTest;
		
		private BeatLoop blLoop;
		private System.Windows.Forms.Button btnTestSynthesis;
        private ThinSlider testSlider;
        private Track track1;
        private BeatBox beatBox1;
        private Button btnShowWaveForm;
        private Button btnViewWaveForm;
        private Label label1;
        private TextBox txtExpression;
        private Button btnParse;
        private TextBox txtDuration;
        private Label label2;
        private Button btnExprShow;
#if ERNST_DX_AUDIO
        WavePlayer _Player;
#else
        SoundPlayer _Player;
#endif

        public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			blLoop = new BeatLoop();
			this.Controls.Add( blLoop );
			blLoop.Left = 0;
			blLoop.Top = 0;

			blLoop.SetLoopState( new byte[16]{ 2, 0, 0, 0, 2, 0, 0, 0, 2, 0, 0, 0, 2, 0, 0, 0 } );

			this.testSlider = new ThinSlider();
			this.Controls.Add( this.testSlider );

			this.testSlider.Left = 5;
			this.testSlider.Top = 30;

			this.testSlider.Size = new Size( 10, 100 );
			this.testSlider.BackColor = Color.White;
			this.testSlider.ForeColor = Color.Navy;

            this._Player = new SoundPlayer();

			this.Refresh();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
            if( disposing )
			{
#if ERNST_DX_AUDIO
#else
                _Player?.Stop();
                _Player?.Dispose();
#endif

                if (components != null) 
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
            this.btnTestWaveStream = new System.Windows.Forms.Button();
            this.btnPlay = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnCapTest = new System.Windows.Forms.Button();
            this.btnTestSynthesis = new System.Windows.Forms.Button();
            this.btnShowWaveForm = new System.Windows.Forms.Button();
            this.track1 = new ErnstTech.SynthesizerControls.Track();
            this.beatBox1 = new ErnstTech.SynthesizerControls.BeatBox();
            this.btnViewWaveForm = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtExpression = new System.Windows.Forms.TextBox();
            this.btnParse = new System.Windows.Forms.Button();
            this.txtDuration = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnExprShow = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnTestWaveStream
            // 
            this.btnTestWaveStream.Location = new System.Drawing.Point(112, 64);
            this.btnTestWaveStream.Margin = new System.Windows.Forms.Padding(2, 3, 3, 3);
            this.btnTestWaveStream.Name = "btnTestWaveStream";
            this.btnTestWaveStream.Size = new System.Drawing.Size(112, 23);
            this.btnTestWaveStream.TabIndex = 1;
            this.btnTestWaveStream.Text = "Test Wave Stream";
            this.btnTestWaveStream.Click += new System.EventHandler(this.btnTestWaveStream_Click);
            // 
            // btnPlay
            // 
            this.btnPlay.Location = new System.Drawing.Point(232, 64);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(75, 23);
            this.btnPlay.TabIndex = 2;
            this.btnPlay.Text = "Play";
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(320, 64);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "Stop";
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnCapTest
            // 
            this.btnCapTest.Location = new System.Drawing.Point(402, 64);
            this.btnCapTest.Name = "btnCapTest";
            this.btnCapTest.Size = new System.Drawing.Size(112, 23);
            this.btnCapTest.TabIndex = 3;
            this.btnCapTest.Text = "Capabilities Test";
            this.btnCapTest.Click += new System.EventHandler(this.btnCapTest_Click);
            // 
            // btnTestSynthesis
            // 
            this.btnTestSynthesis.Location = new System.Drawing.Point(13, 64);
            this.btnTestSynthesis.Margin = new System.Windows.Forms.Padding(3, 3, 1, 3);
            this.btnTestSynthesis.Name = "btnTestSynthesis";
            this.btnTestSynthesis.Size = new System.Drawing.Size(96, 23);
            this.btnTestSynthesis.TabIndex = 4;
            this.btnTestSynthesis.Text = "Test Synthesizer";
            this.btnTestSynthesis.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnShowWaveForm
            // 
            this.btnShowWaveForm.Location = new System.Drawing.Point(13, 93);
            this.btnShowWaveForm.Name = "btnShowWaveForm";
            this.btnShowWaveForm.Size = new System.Drawing.Size(96, 23);
            this.btnShowWaveForm.TabIndex = 7;
            this.btnShowWaveForm.Text = "Show Waveform";
            this.btnShowWaveForm.Click += new System.EventHandler(this.btnShowWaveForm_Click);
            // 
            // track1
            // 
            this.track1.BeatCount = 16;
            this.track1.Location = new System.Drawing.Point(16, 144);
            this.track1.Name = "track1";
            this.track1.Size = new System.Drawing.Size(176, 144);
            this.track1.TabIndex = 6;
            // 
            // beatBox1
            // 
            this.beatBox1.BaseFrequency = 200F;
            this.beatBox1.Location = new System.Drawing.Point(224, 192);
            this.beatBox1.Name = "beatBox1";
            this.beatBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.beatBox1.SampleQuality = ErnstTech.SoundCore.AudioBits.Bits16;
            this.beatBox1.Size = new System.Drawing.Size(480, 296);
            this.beatBox1.TabIndex = 0;
            // 
            // btnViewWaveForm
            // 
            this.btnViewWaveForm.Location = new System.Drawing.Point(112, 93);
            this.btnViewWaveForm.Name = "btnViewWaveForm";
            this.btnViewWaveForm.Size = new System.Drawing.Size(112, 23);
            this.btnViewWaveForm.TabIndex = 8;
            this.btnViewWaveForm.Text = "View Wave Form";
            this.btnViewWaveForm.Click += new System.EventHandler(this.btnViewWaveForm_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(232, 94);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 15);
            this.label1.TabIndex = 9;
            this.label1.Text = "Expression Test:";
            // 
            // txtExpression
            // 
            this.txtExpression.Location = new System.Drawing.Point(232, 113);
            this.txtExpression.Name = "txtExpression";
            this.txtExpression.Size = new System.Drawing.Size(309, 23);
            this.txtExpression.TabIndex = 10;
            this.txtExpression.Text = "cos(2 * PI * (220 + 4 * cos(2 * PI * 10 * t)) * t) * 0.5";
            // 
            // btnParse
            // 
            this.btnParse.Location = new System.Drawing.Point(483, 143);
            this.btnParse.Name = "btnParse";
            this.btnParse.Size = new System.Drawing.Size(75, 23);
            this.btnParse.TabIndex = 11;
            this.btnParse.Text = "Test";
            this.btnParse.UseVisualStyleBackColor = true;
            this.btnParse.Click += new System.EventHandler(this.btnParse_Click);
            // 
            // txtDuration
            // 
            this.txtDuration.Location = new System.Drawing.Point(232, 142);
            this.txtDuration.Name = "txtDuration";
            this.txtDuration.Size = new System.Drawing.Size(51, 23);
            this.txtDuration.TabIndex = 12;
            this.txtDuration.Text = "1.0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(289, 147);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 15);
            this.label2.TabIndex = 9;
            this.label2.Text = "Test Duration (secs)";
            // 
            // btnExprShow
            // 
            this.btnExprShow.Location = new System.Drawing.Point(402, 142);
            this.btnExprShow.Name = "btnExprShow";
            this.btnExprShow.Size = new System.Drawing.Size(75, 23);
            this.btnExprShow.TabIndex = 13;
            this.btnExprShow.Text = "Show";
            this.btnExprShow.UseVisualStyleBackColor = true;
            this.btnExprShow.Click += new System.EventHandler(this.btnExprShow_Click);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(712, 486);
            this.Controls.Add(this.btnExprShow);
            this.Controls.Add(this.txtDuration);
            this.Controls.Add(this.btnParse);
            this.Controls.Add(this.txtExpression);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnViewWaveForm);
            this.Controls.Add(this.btnShowWaveForm);
            this.Controls.Add(this.track1);
            this.Controls.Add(this.btnTestSynthesis);
            this.Controls.Add(this.btnCapTest);
            this.Controls.Add(this.btnPlay);
            this.Controls.Add(this.btnTestWaveStream);
            this.Controls.Add(this.beatBox1);
            this.Controls.Add(this.btnStop);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void btnTestWaveStream_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Open Wave Audio...";
			ofd.Filter = "Wave Files (*.wav)|*.wav";
			ofd.FilterIndex = 1;

			if ( ofd.ShowDialog() == DialogResult.OK )
			{
				var stream = new System.IO.FileStream( ofd.FileName, 
					System.IO.FileMode.Open,
					System.IO.FileAccess.Read, System.IO.FileShare.Read, 16 * 1024 );

#if ERNST_DX_AUDIO
                _Player = new WavePlayer();
				_Player = new WavePlayer( this, stream );
				_Player.BufferLength = 1000;
#else
                _Player.Stream = stream;
                _Player.Play();
#endif
            }
		}

		private void btnPlay_Click(object sender, System.EventArgs e)
		{
            _Player?.Play();
		}

		private void btnStop_Click(object sender, System.EventArgs e)
		{
            _Player?.Stop();
		}

		private void btnCapTest_Click(object sender, System.EventArgs e)
		{
//			MessageBox.Show( WavePlayer.NumDevices.ToString() );
//
//			WaveOutDeviceCapabilities cap = new WaveOutDeviceCapabilities( 0 );
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			ErnstTech.SoundCore.WaveForm form = new ErnstTech.SoundCore.WaveForm( new ErnstTech.SoundCore.WaveFormat(  2, 44100, 16 ) );
            form.BaseFrequency = 200;

			Point[] points = new Point[ this.beatBox1.Points.Count ];
            for (int i = 0, length = points.Length; i < length; ++i)
            {
                PointF pt = this.beatBox1.Points[i];
                points[i] = new Point(Convert.ToInt32(pt.X), Convert.ToInt32(pt.Y));
            }

            form.Points.AddRange( points );
			System.IO.Stream s = form.GenerateWave();

#if ERNST_DX_AUDIO
            System.Diagnostics.Debug.Assert(false);

			WavePlayer player = new WavePlayer( this, s );
			player.Play();
#else
            this._Player.Stop();
            this._Player.Stream = s;
            this._Player.Play();
#endif
        }

		private void Form1_Load(object sender, System.EventArgs e)
		{
		
		}

		private void beatLoop1_Click(object sender, System.EventArgs e)
		{
		
		}

        private void btnShowWaveForm_Click(object sender, EventArgs e)
        {
            ErnstTech.SoundCore.WaveForm form = new ErnstTech.SoundCore.WaveForm(new ErnstTech.SoundCore.WaveFormat(2, 44100, 16));
            form.BaseFrequency = 100;

            System.IO.Stream s = form.GenerateWave();

            using (WaveFormView view = new WaveFormView(s))
            {
                view.ZoomFactor = 0.25f;
                view.ShowDialog();
            }

            s.Close();
        }

        private void btnViewWaveForm_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    System.IO.Stream s = ofd.OpenFile();
                    using (WaveFormView view = new WaveFormView(s))
                    {
                        //view.ZoomFactor = 0.25f;
                        view.ShowDialog();
                    }
                    s.Close();
                }
            }
        }

        static IEnumerable<float> ToEnumerable(int sampleRate, Func<double, double> func)
        {
            var count = 0;
            var delta = 1.0 / sampleRate;

            while (true)
                yield return (float)func(count++ * delta);
        }

        Stream GenerateFromExpression()
        {
            const int sampleRate = 44100;
            var parser = new SoundCore.Synthesis.ExpressionParser();
            var func = parser.Parse(txtExpression.Text);
            var duration = double.Parse(this.txtDuration.Text);
            int nSamples = (int)(sampleRate * duration);
            var dataSize = nSamples * sizeof(float);

            var format = new SoundCore.WaveFormat(1, sampleRate, 32);

            var ms = new MemoryStream(dataSize + SoundCore.WaveFormat.HeaderSize);
            new SoundCore.WaveWriter(ms, sampleRate).Write(nSamples, ToEnumerable(sampleRate, func));

            ms.Position = 0;
            return ms;
        }

        private void btnParse_Click(object sender, EventArgs e) =>
            new SoundPlayer(GenerateFromExpression()).Play();

        private void btnExprShow_Click(object sender, EventArgs e)
        {
            using var s = GenerateFromExpression();
            //using var v = new global::Synthesizer.WaveFormView2(s);
            using var v = new WaveFormView(s);
            v.ShowDialog();
        }
    }
}
