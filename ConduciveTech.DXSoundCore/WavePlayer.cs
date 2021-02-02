using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.XAudio2;
using System.Media;


namespace ErnstTech.DXSoundCore
{
    public class WavePlayer : IDisposable
    {
        private bool disposedValue;

        internal XAudio2 XAudio2 { get; init; }
        internal MasteringVoice MasteringVoice { get; init; }

        public WavePlayer()
        {
            XAudio2 = new XAudio2();
            MasteringVoice = new MasteringVoice(XAudio2);
        }

        public void Play(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var player = new SoundPlayer(stream);
            player.Play();
        }

        public void Stop()
        {

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.MasteringVoice.Dispose();
                    this.XAudio2.Dispose();
                }

                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~WavePlayer()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
