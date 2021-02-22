using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ErnstTech.SoundCore;


namespace ErnstTech.SoundCore.Tests
{
    [TestClass]
    public class WaveReaderWriterTests
    {
        [TestMethod]
        public void Test8BitSingleChannelRoundTrip()
        {
            const int samplesPerSecond = 44_100;
            const double delta = 1.0 / samplesPerSecond;

            Func<double, double> func = (double t) => Math.Cos(2.0 * Math.PI * t);

            byte[] data = new byte[samplesPerSecond];
            for (int i = 0; i < samplesPerSecond; ++i)
                data[i] = (byte)(byte.MaxValue * ((func(i * delta) + 1.0) / 2.0));

            using var ms = new MemoryStream();
            var writer = new WaveWriter(ms, samplesPerSecond);
            writer.Write(data.Length, data);

            ms.Position = 0;

            var reader = new WaveReader(ms);
            Assert.AreEqual(reader.Format.Channels, 1);
            Assert.AreEqual(reader.Format.SamplesPerSecond, samplesPerSecond);
            Assert.AreEqual(reader.Format.BitsPerSample, 8);

            var channel = reader.GetChannelInt8(0);
            int h = 0;
            var e = channel.GetEnumerator();
            while (e.MoveNext())
                Assert.AreEqual(data[h++], e.Current);
        }

        [TestMethod]
        public void Test16BitSingleChannelRoundTrip()
        {
            const int samplesPerSecond = 44_100;
            const double delta = 1.0 / samplesPerSecond;

            Func<double, double> func = (double t) => Math.Cos(2.0 * Math.PI * t);

            short[] data = new short[samplesPerSecond];
            for (int i = 0; i < samplesPerSecond; ++i)
                data[i] = (short)(short.MaxValue * func(i * delta));

            using var ms = new MemoryStream();
            var writer = new WaveWriter(ms, samplesPerSecond);
            writer.Write(data.Length, data);

            ms.Position = 0;

            var reader = new WaveReader(ms);
            Assert.AreEqual(reader.Format.Channels, 1);
            Assert.AreEqual(reader.Format.SamplesPerSecond, samplesPerSecond);
            Assert.AreEqual(reader.Format.BitsPerSample, 16);

            var channel = reader.GetChannelInt16(0);
            int h = 0;
            var e = channel.GetEnumerator();
            while (e.MoveNext())
                Assert.AreEqual(data[h++], e.Current, $"Index: {h}");
        }

        [TestMethod]
        public void Test32BitSingleChannelRoundTrip()
        {
            const int samplesPerSecond = 44_100;
            const double delta = 1.0 / samplesPerSecond;

            Func<double, double> func = (double t) => Math.Cos(2.0 * Math.PI * t);

            float[] data = new float[samplesPerSecond];
            for (int i = 0; i < samplesPerSecond; ++i)
                data[i] = (float)func(i * delta);

            using var ms = new MemoryStream();
            var writer = new WaveWriter(ms, samplesPerSecond);
            writer.Write(data.Length, data);

            ms.Position = 0;

            var reader = new WaveReader(ms);
            Assert.AreEqual(reader.Format.FormatTag, FormatTag.WAVE_FORMAT_IEEE_FLOAT);
            Assert.AreEqual(reader.Format.Channels, 1);
            Assert.AreEqual(reader.Format.SamplesPerSecond, samplesPerSecond);
            Assert.AreEqual(reader.Format.BitsPerSample, 32);

            var channel = reader.GetChannelFloat(0);
            int h = 0;
            var e = channel.GetEnumerator();
            while (e.MoveNext())
                Assert.AreEqual(data[h++], e.Current);
        }
        
    }
}
