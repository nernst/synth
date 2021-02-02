
using System;
using System.Threading;
using SharpDX;
using SharpDX.Multimedia;
using SharpDX.XAudio2;
using SharpDX.XAudio2.Fx;
using BufferFlags = SharpDX.XAudio2.BufferFlags;

namespace PlayDynamicSound
{
    class Program
    {
        /// <summary>
        /// SharpDX XAudio2 sample. Plays a generated sound with some reverb.
        /// </summary>
        static void Main(string[] args)
        {
            var xaudio2 = new XAudio2();
            var masteringVoice = new MasteringVoice(xaudio2);

            var waveFormat = new WaveFormat(44100, 32, 2);
            var sourceVoice = new SourceVoice(xaudio2, waveFormat);

            int bufferSize = waveFormat.ConvertLatencyToByteSize(60000);
            var dataStream = new DataStream(bufferSize, true, true);

            int numberOfSamples = bufferSize / waveFormat.BlockAlign;
            for (int i = 0; i < numberOfSamples; i++)
            {
                double vibrato = Math.Cos(2 * Math.PI * 10.0 * i / waveFormat.SampleRate);
                float value = (float)(Math.Cos(2 * Math.PI * (220.0 + 4.0 * vibrato) * i / waveFormat.SampleRate) * 0.5);
                dataStream.Write(value);
                dataStream.Write(value);
            }
            dataStream.Position = 0;

            var audioBuffer = new AudioBuffer { Stream = dataStream, Flags = BufferFlags.EndOfStream, AudioBytes = bufferSize };

            var reverb = new Reverb(xaudio2);
            var effectDescriptor = new EffectDescriptor(reverb);
            sourceVoice.SetEffectChain(effectDescriptor);
            sourceVoice.EnableEffect(0);

            sourceVoice.SubmitSourceBuffer(audioBuffer, null);

            sourceVoice.Start();

            Console.WriteLine("Play sound");
            for (int i = 0; i < 60; i++)
            {
                Console.Write(".");
                Console.Out.Flush();
                Thread.Sleep(1000);
            }
        }
    }
}