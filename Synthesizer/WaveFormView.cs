using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ErnstTech.Math;
using ErnstTech.Synthesizer;
using ErnstTech.SoundCore;
using System.IO;
using System.Numerics;

namespace ErnstTech.Synthesizer
{
    public partial class WaveFormView : Form
    {
        private WaveStream _WaveForm;
        private PointF[] _DataPoints;
        private PointF[] _ScaledPoints;
        private PointF[] _MagnitudePoints;
        private PointF[] _ScaledMagnitudePoints;
        private PointF[] _PhasePoints;
        private PointF[] _ScaledPhasePoints;

        private float _ZoomFactor = 1.0f;
        public float ZoomFactor
        {
            get { return this._ZoomFactor; }
            set 
            { 
                value = System.Math.Abs(value);
                if (value == 0.0f)
                    value = 1.0f;

                if (value == this._ZoomFactor)
                    return;

                this._ZoomFactor = value;
                this.OnZoomFactorChanged(EventArgs.Empty);
            }
        }

        public event EventHandler ZoomFactorChanged
        {
            add { this.Events.AddHandler(ZoomFactorChangedEvent, value); }
            remove { this.Events.RemoveHandler(ZoomFactorChangedEvent, value); }
        }

        private static readonly object ZoomFactorChangedEvent = new object();
        protected virtual void OnZoomFactorChanged(EventArgs arguments)
        {
            EventHandler handler = this.Events[ZoomFactorChangedEvent] as EventHandler;
            if (handler != null)
                handler(this, arguments);
        }

        private bool _ShowFrequencyDomain = false;
        public bool ShowFrequencyDomain
        {
            get { return this._ShowFrequencyDomain; }
            set
            {
                if (this._ShowFrequencyDomain == value)
                    return;

                this._ShowFrequencyDomain = value;
                this.OnShowFrequencyDomainChanged(EventArgs.Empty);
            }
        }

        private static readonly object ShowFrequencyDomainChangedEvent = new object();
        public event EventHandler ShowFrequencyDomainChanged
        {
            add { this.Events.AddHandler(ShowFrequencyDomainChangedEvent, value); }
            remove { this.Events.RemoveHandler(ShowFrequencyDomainChangedEvent, value); }
        }

        protected virtual void OnShowFrequencyDomainChanged(EventArgs arguments)
        {
            EventHandler handler = this.Events[ShowFrequencyDomainChangedEvent] as EventHandler;
            if (handler != null)
                handler(this, arguments);
        }

        #region MagnitudeColor Property
        private Color _MagnitudeColor = Color.Blue;
        public Color MagnitudeColor
        {
            get { return this._MagnitudeColor; }
            set
            {
                if (this._MagnitudeColor == value)
                    return;

                this._MagnitudeColor = value;
                this.OnMagnitudeColorChanged(EventArgs.Empty);
            }
        }

        private static readonly object MagnitudeColorChangedEvent = new object();
        public event EventHandler MagnitudeColorChanged
        {
            add { this.Events.AddHandler(MagnitudeColorChangedEvent, value); }
            remove { this.Events.RemoveHandler(MagnitudeColorChangedEvent, value); }
        }

        protected virtual void OnMagnitudeColorChanged(EventArgs arguments)
        {
            EventHandler handler = this.Events[MagnitudeColorChangedEvent] as EventHandler;
            if (handler != null)
                handler(this, arguments);
        }
        #endregion

        #region PhaseColor Property
        private Color _PhaseColor = Color.Red;
        public Color PhaseColor
        {
            get { return this._PhaseColor; }
            set
            {
                if (this._PhaseColor == value)
                    return;

                this._PhaseColor = value;
                this.OnPhaseColorChanged(EventArgs.Empty);
            }
        }

        private static readonly object PhaseColorChangedEvent = new object();
        public event EventHandler PhaseColorChanged
        {
            add { this.Events.AddHandler(PhaseColorChangedEvent, value); }
            remove { this.Events.RemoveHandler(PhaseColorChangedEvent, value); }
        }

        protected virtual void OnPhaseColorChanged(EventArgs arguments)
        {
            EventHandler handler = this.Events[PhaseColorChangedEvent] as EventHandler;
            if (handler != null)
                handler(this, arguments);
        }
        #endregion


        private ManualResetEvent _FFTReady = new ManualResetEvent(false);

        public WaveFormView(Stream waveForm)
        {
            if (waveForm == null)
                throw new ArgumentNullException("waveForm");

            _WaveForm = new WaveStream(waveForm);
            this.ReadPointsFromWaveForm();
            
            InitializeComponent();

            this.panelWaveForm.Paint += new PaintEventHandler(panelWaveForm_Paint);
            this.AutoScroll = true;
            this.ZoomFactorChanged += delegate{
                this.ScalePoints();
                this.Refresh();
            };
            this.MagnitudeColorChanged += delegate
            {
                if (this.ShowFrequencyDomain)
                    this.Refresh();
            };
            this.PhaseColorChanged += delegate
            {
                if (this.ShowFrequencyDomain)
                    this.Refresh();
            };
            this.ShowFrequencyDomainChanged += delegate
            {
                this.Refresh();
            };
            this.miFFTOff.Click += delegate
            {
                this.ShowFrequencyDomain = false;
            };
            this.miFFTOn.Click += delegate
            {
                this.ShowFrequencyDomain = true;
            };

            Thread fftThread = new Thread(new ThreadStart(this.CalculateFFT));
            fftThread.Start();
        }

        private void CalculateFFT()
        {
            long nPoints = this._DataPoints.LongLength;
            Complex[] data = new Complex[nPoints];
            for (long i = 0; i < nPoints; ++i)
                data[i] = new Complex(Convert.ToDouble(i), this._DataPoints[i].Y );

            ErnstTech.Math.FastFourierTransform.Transform(1, data);

            this._MagnitudePoints = new PointF[nPoints];
            this._PhasePoints = new PointF[nPoints];

            for (long i = 0; i < nPoints; ++i)
            {
                float x = Convert.ToSingle(i);
                this._MagnitudePoints[i] = new PointF(x,
                    Convert.ToSingle(data[i].Magnitude));
                this._PhasePoints[i] = new PointF(x,
                    Convert.ToSingle(data[i].Phase));
            }

            this.Normalize(this._MagnitudePoints);
            this.Normalize(this._PhasePoints);
            this._FFTReady.Set();
        }

        private int CalculateHeight()
        {
            return this.ClientSize.Height - this.toolStrip1.Height;
        }

        void panelWaveForm_Paint(object sender, PaintEventArgs e)
        {
            using (Brush b = new SolidBrush(this.panelWaveForm.BackColor))
            {
                e.Graphics.FillRectangle(b, this.Bounds);
            }

            using (Brush b = new SolidBrush(this.ForeColor))
            using (Pen p = new Pen(b))
            {
                e.Graphics.DrawLines(p, this._ScaledPoints);
                //e.Graphics.DrawCurve(p, this._ScaledPoints);
            }

            if (this.ShowFrequencyDomain)
            {
                this._FFTReady.WaitOne();

                using (Brush b = new SolidBrush(this.MagnitudeColor))
                using (Pen p = new Pen(b))
                {
                    e.Graphics.DrawLines(p, this._ScaledMagnitudePoints);
                }

                using (Brush b = new SolidBrush(this.PhaseColor))
                using (Pen p = new Pen(b))
                {
                    e.Graphics.DrawLines(p, this._ScaledPhasePoints);
                }
            }
        }

        private void ReadPointsFromWaveForm()
        {
            switch (this._WaveForm.Format.BitsPerSample)
            {
                case 8:
                    this.ReadBytePointsFromWaveForm();
                    break;
                case 16:
                    this.ReadShortPointsFromWaveForm();
                    break;
                case 32:
                    this.ReadSinglePointsFromWaveForm();
                    break;
                default:
                    throw new NotSupportedException($"An unsupported sample size was encountered: {this._WaveForm.Format.BitsPerSample}.");
            }
        }

        void Normalize(PointF[] points)
        {
            float max = 0.0f;
            foreach (PointF pt in points)
            {
                float abs = System.Math.Abs(pt.Y);
                if (abs > max)
                    max = abs;
            }

            for (long i = 0, len = points.LongLength; i < len; ++i)
            {
                points[i] = new PointF(points[i].X, points[i].Y / max);
            }
        }

        private void ScalePoints()
        {
            this.panelWaveForm.ClientSize = new Size(Convert.ToInt32(this._DataPoints.LongLength * this.ZoomFactor),
                this.CalculateHeight());
            this.panelWaveForm.Top = this.toolStrip1.Height;

            long nPoints = this._DataPoints.LongLength;
            this._ScaledPoints = new PointF[nPoints];
            this._ScaledMagnitudePoints = new PointF[nPoints];
            this._ScaledPhasePoints = new PointF[nPoints];

            int height = this.panelWaveForm.ClientSize.Height;
            float vScale = height / -2.0f;      // Invert the points
            int vTranslation = height / 2;
 
            for (long idx = 0; idx < nPoints; ++idx)
            {
                float x = Convert.ToSingle(idx) * this.ZoomFactor;

                this._ScaledPoints[idx] = new PointF(x,
                    Convert.ToSingle( vScale * this._DataPoints[idx].Y + vTranslation) );

                if (this.ShowFrequencyDomain)
                {
                    this._ScaledMagnitudePoints[idx] = new PointF(x,
                        Convert.ToSingle(vScale * this._MagnitudePoints[idx].Y + vTranslation));
                    this._ScaledPhasePoints[idx] = new PointF(x,
                        Convert.ToSingle(vScale * this._PhasePoints[idx].Y + vTranslation ));
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            this.panelWaveForm.ClientSize = new Size(this.panelWaveForm.ClientSize.Width, this.Height);
            this.ScalePoints();
            base.OnResize(e);
        }

        private void ReadBytePointsFromWaveForm()
        {
            int nChannels = this._WaveForm.Format.Channels;
            long nSamples = this._WaveForm.NumberOfSamples;
            this._DataPoints = new PointF[nSamples];

            for (long i = 0, len = nSamples; i < len; ++i)
            {
                sbyte sample = (sbyte)this._WaveForm.ReadByte();
                this._DataPoints[i] = new PointF(Convert.ToSingle(i), Convert.ToSingle(sample) / sbyte.MaxValue);
                for (int h = 1; h < nChannels; ++h)
                    this._WaveForm.ReadByte();
            }
        }

        private void ReadShortPointsFromWaveForm()
        {
            int nChannels = this._WaveForm.Format.Channels;
            long nSamples = this._WaveForm.NumberOfSamples;
            this._DataPoints = new PointF[nSamples];

            for (long i = 0, len = nSamples; i < len; ++i)
            {
                short sample = (short)this._WaveForm.ReadByte();
                sample |= (short)(this._WaveForm.ReadByte() << 8);

                this._DataPoints[i] = new PointF(Convert.ToSingle(i), Convert.ToSingle(sample) / short.MaxValue);
                for (int h = 1; h < nChannels; ++h)
                {
                    this._WaveForm.ReadByte();
                    this._WaveForm.ReadByte();
                }
            }
        }

        private void ReadSinglePointsFromWaveForm()
        {
            int nChannels = this._WaveForm.Format.Channels;
            if (nChannels != 1)
                throw new NotSupportedException("Only 1 channel is supported, currently.");

            long nSamples = this._WaveForm.NumberOfSamples;
            this._DataPoints = new PointF[nSamples];

            double rate = 1.0 / this._WaveForm.Format.SamplesPerSecond;
            byte[] buffer = new byte[4];

            for (long i = 0; i < nSamples; ++i)
            {
                this._WaveForm.Read(buffer);
                this._DataPoints[i] = new PointF(Convert.ToSingle(i), BitConverter.ToSingle(buffer));
            }
        }

        private void txtZoomFactor_Validating(object sender, CancelEventArgs e)
        {
            float result;
            e.Cancel = !float.TryParse(this.txtZoomFactor.Text, out result);
        }

        private void txtZoomFactor_TextChanged(object sender, EventArgs e)
        {
            float factor;
            if (!float.TryParse(this.txtZoomFactor.Text, out factor))
                factor = 100.0f;
            factor /= 100;
            this.ZoomFactor = factor;
        }
    }
}