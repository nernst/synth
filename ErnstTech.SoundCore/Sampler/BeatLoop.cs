using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ErnstTech.SoundCore.Sampler
{
    public class BeatLoop
    {
        public ISampler? Sampler { get; set; }
        public double BeatsPerMinute { get; set; } = 165.0;
        public double BeatDuration => 1.0 / (4 * BeatsPerMinute / 60); //BeatsPerMinute / 60 / 4;
        public double FullHeight { get; set; } = 1.0;
        public double HalfHeight { get; set; } = 0.5;

        static readonly double[] DefaultLevels = new[] { Beat.Full, Beat.Off, Beat.Half, Beat.Off, Beat.Full, Beat.Off, Beat.Off, Beat.Off, Beat.Full, Beat.Off, Beat.Half, Beat.Off, Beat.Full, Beat.Off, Beat.Off, Beat.Off };
        public IList<Beat> Beats { get; init; } = DefaultLevels.Select(l => new Beat { Level = l }).ToList();
        public int BeatCount => Beats.Count;

        public BeatLoop()
            : this(16)
        { }

        public BeatLoop(int beatCount)
            : this(Enumerable.Range(0, beatCount).Select(_ => new Beat { Level = Beat.Off }).ToArray())
        {
            if (this.BeatCount == 0)
                throw new ArgumentOutOfRangeException(nameof(beatCount), beatCount, "Must be a positive integer.");
        }

        public BeatLoop(IList<Beat> beats)
        {
            Beats = beats;
        }

        Stream? _WAVStream;
        public Stream WAVStream
        {
            get => _WAVStream ??= GenerateStream();
        }

        public void InvalidateWAVStream() => _WAVStream = null;

        Stream GenerateStream()
        {
            if (Sampler == null)
                throw new InvalidOperationException("Sampler has not been specified.");
            if (BeatsPerMinute <= 0)
                throw new InvalidOperationException("BeatsPerMinute must be positive.");
            if (BeatCount <= 0)
                throw new InvalidOperationException("Cannot generate with no beats.");

            var samplesPerBeat = BeatDuration * (long)Sampler.SampleRate;

            long bufferSize = 0;
            // Fixup all of the beats
            for (int i = 0; i < Beats.Count; ++i)
            {
                var beat = Beats[i];
                beat.Sampler ??= Sampler;
                beat.QueuePoint ??= (long)(i * BeatDuration * (long)Sampler.SampleRate);
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
            var writer = new WaveWriter(ms, (int)Sampler.SampleRate);
            writer.Write(buffer.Length, buffer.Select(s => (float)s));
            ms.Position = 0;

            return ms;
        }
    }
}
