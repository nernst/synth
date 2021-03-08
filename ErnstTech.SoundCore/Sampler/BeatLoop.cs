using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ErnstTech.SoundCore.Sampler
{
    public class BeatLoop
    {
        readonly object _SyncRoot = new object();
        const int _DefaultBeats = 16;

        public ISampler? Sampler { get; set; }
        public double BeatsPerMinute { get; set; } = 165.0;
        public double BeatDuration => 1.0 / (4 * BeatsPerMinute / 60); //BeatsPerMinute / 60 / 4;
        public double FullHeight { get; set; } = 1.0;
        public double HalfHeight { get; set; } = 0.5;

        static double[] DefaultLevels = new[] { Beat.Full, Beat.Off, Beat.Half, Beat.Off, Beat.Full, Beat.Off, Beat.Off, Beat.Off, Beat.Full, Beat.Off, Beat.Half, Beat.Off, Beat.Full, Beat.Off, Beat.Off, Beat.Off };
        public IList<Beat> Beats { get; init; } = DefaultLevels.Select(l => new Beat { Level = l }).ToList();
        public int BeatCount => Beats.Count;

        Stream? _WAVStream;
        public Stream WAVStream
        {
            get
            {
                if (_WAVStream == null)
                    _WAVStream = GenerateStream();
                return _WAVStream;
            }
            private set { _WAVStream = null; }
        }

        public BeatLoop(ISampler? sampler = null)
        {
            this.Sampler = sampler;
        }

        public void InvalidateWAVStream() => _WAVStream = null;

        Stream GenerateStream()
        {
            if (Sampler == null)
                throw new InvalidOperationException("Sampler has not been specified.");
            if (BeatsPerMinute <= 0)
                throw new InvalidOperationException("BeatsPerMinute must be positive.");

            var samplesPerBeat = BeatDuration * Sampler.SampleRate;

            long bufferSize = 0;
            // Fixup all of the beats
            for (int i = 0; i < Beats.Count; ++i)
            {
                var beat = Beats[i];
                beat.Sampler ??= Sampler;
                beat.QueuePoint ??= (long)(i * BeatDuration * Sampler.SampleRate);
                if (beat.Sampler.SampleRate != Sampler.SampleRate)
                    throw new InvalidOperationException($"All samplers must have the sample sample rate. Sampler for beat {i} has value {beat.Sampler.SampleRate}. Base sampler has {Sampler.SampleRate}");

                bufferSize = Math.Max(bufferSize, beat.QueuePoint.Value + beat.Sampler.Length);
            }

            var buffer = new double[bufferSize];

            foreach (var beat in Beats)
            {
                for (long i = beat.QueuePoint ?? 0, len = i + beat.Sampler!.Length; i < len; ++i)
                    buffer[i] += beat.Sample(i);
            }


            var ms = new MemoryStream();
            var writer = new WaveWriter(ms, Sampler.SampleRate);
            writer.Write(buffer.Length, buffer.Select(s => (float)s));
            ms.Position = 0;

            return ms;
        }
    }
}
