using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErnstTech.SoundCore.Sampler
{
    public class BeatLoop
    {
        readonly object _SyncRoot = new object();
        const int _DefaultBeats = 16;

        public enum Beat { Off, Half, Full }

        public ISampler Sampler { get; set; }
        public double BeatsPerMinute { get; set; } = 165.0;
        public double? BeatDuration { get; set; } = null;
        public ObservableCollection<Beat> Beats { get; init; } = new ObservableCollection<Beat>(Enumerable.Repeat(Beat.Off, _DefaultBeats));
        public int BeatCount => Beats.Count;

        Stream _WAVStream = null;
        public Stream WAVStream
        {
            get
            {
                lock (_SyncRoot)
                {
                    if (_WAVStream == null)
                        _WAVStream = GenerateStream();
                    return _WAVStream;
                }
            }
            private set { lock(_SyncRoot) { _WAVStream = null; } }
        }

        public BeatLoop()
        {
            this.Beats.CollectionChanged += (o, e) => InvalidateStream();
        }

        void InvalidateStream() => WAVStream = null;

        Stream GenerateStream()
        {
            if (BeatsPerMinute <= 0)
                throw new InvalidOperationException("BeatsPerMinute must be positive.");
            if ((BeatDuration == null || BeatDuration.Value <= 0) && Sampler.Length < 0)
                throw new InvalidOperationException("Cannot determine length of sample.");

            var timePerBeat = BeatsPerMinute / 60;
            var samplesPerBeat = (long)timePerBeat * Sampler.SampleRate;

            var finalBuffer = new double[samplesPerBeat];
            var sample = Sampler.GetSamples();
            var halfSample = Beats.Any(beat => beat == Beat.Half) ? sample.Select(s => 0.5 * s).ToArray() : null;

            for (int i = 0; i < BeatCount; ++i)
            {
                var offset = i * samplesPerBeat;
                double[] source = null;
                switch(Beats[i])
                {
                    case Beat.Off:
                        continue;

                    case Beat.Half:
                        source = halfSample;
                        Debug.Assert(source != null, "Expected halfSample to be non-null.");
                        break;

                    case Beat.Full:
                        source = sample;
                        break;

                    default:
                        throw new InvalidOperationException($"Unexpected value for beat at index {i}: {Beats[i]}");
                }

                Array.Copy(source, 0, finalBuffer, offset, source.Length);
            }

            var ms = new MemoryStream();
            var writer = new WaveWriter(ms, Sampler.SampleRate);
            writer.Write(finalBuffer.Length, finalBuffer.Select(s => (float)s));
            ms.Position = 0;

            return ms;
        }
    }
}
