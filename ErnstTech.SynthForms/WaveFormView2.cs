using ErnstTech.SoundCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Synthesizer
{
    public partial class WaveFormView2 : Form
    {
        WaveReader _Reader;
        double xZoom = 1.0;

        public WaveFormView2(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            InitializeComponent();

            _Reader = new WaveReader(stream);

            for (short i = 0; i < _Reader.Format.Channels; ++i)
            {
                switch (_Reader.Format.BitsPerSample)
                {
                    case 8: this.wavePlot.plt.PlotSignalConst(_Reader.GetChannelInt8(i).Select(x => (float)x).ToArray()); break;
                    case 16: this.wavePlot.plt.PlotSignalConst(_Reader.GetChannelInt16(i).Select(x => (float)x).ToArray()); break;
                    case 32: this.wavePlot.plt.PlotSignalConst(_Reader.GetChannelFloat(i).ToArray()); break;
                    default:
                        throw new NotSupportedException($"Invalid sample size: {_Reader.Format.BitsPerSample}.");
                };
            }

            this.wavePlot.MouseWheel += WavePlot_MouseWheel;

            this.wavePlot.plt.AxisAuto(horizontalMargin: 0, verticalMargin: 0);
        }

        private void WavePlot_MouseWheel(object sender, MouseEventArgs e)
        {
            var delta = e.Delta / 120.0; // Windows constant
            var existing = this.wavePlot.plt.AxisZoom();
            if (e.Delta > 0)
                xZoom = 10; //  xZoom *= 10 * delta;
            else if (e.Delta < 0)
                xZoom = 0.1; // xZoom /= 10 * delta;

            this.wavePlot.plt.AxisZoom(xFrac: xZoom, yFrac: 1.0);
        }
    }
}
