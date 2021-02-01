using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace ErnstTech.SoundCore
{
    /// <summary>
    /// Summary description for WavePlayer.
    /// </summary>
    public class WavePlayer : IDisposable
    {
        private AutoResetEvent _DonePlaying = new AutoResetEvent(false);

        private WaveBufferEmptyHandler _FillerDelegate;

        private WaveFormat _Format;
        public WaveFormat Format
        {
            get { return _Format; }
        }

        private bool _IsPlaying = false;
        public bool IsPlaying
        {
            get { return _IsPlaying; }
        }

        private Stream _DataStream;
        private BinaryReader _Reader;
        private WaveOutBuffer _RootBuffer;
        //		private WaveOutBuffer[] _Buffers;

        private long _StartPosition;

        private int _Device = -1;       // Default to system default
        public int Device
        {
            get { return _Device; }
            set
            {
                if (IsPlaying)
                    throw new SoundCoreException("Device ID cannot be set while device is in use.");

                if (value < -1 || value >= NumDevices)
                    throw new ArgumentOutOfRangeException(string.Format("Device must be between -1 and {0}, inclusive.", NumDevices));

                _Device = value;
            }
        }

#if DEBUG
        private long _FillCount = 0;
#endif

        public static int NumDevices
        {
            get { return WaveFormNative.waveOutGetNumDevs(); }
        }

        private IntPtr _DeviceHandle;
        //		private Thread _PlayThread;
        //		private Thread _FillThread;

        private int _BufferLength = 4000;

        /// <summary>
        /// The length of the buffer in milliseconds.
        /// </summary>
        public int BufferLength
        {
            get { return _BufferLength; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("BufferLength must be a positive integer.", "value");

                _BufferLength = value;
            }
        }

        public int BufferCount
        {
            get { return 4; }
        }

        /// <summary>
        /// The calculated size of the buffer, in bytes.
        /// </summary>
        public int BufferSize
        {
            get
            {
                int length = Convert.ToInt32(Convert.ToDouble(_Format.AverageBytesPerSecond * BufferLength) / 1000.0);
                int waste = length % _Format.BlockAlignment;

                length += _Format.BlockAlignment - waste;
                return length;
            }
        }

        private byte[] _StreamBuffer;
        private WaveOutProcDelegate _CallBack = new WaveOutProcDelegate(WaveOutBuffer.WaveOutCallback);

        public WavePlayer(short channels, int rate, short bitsPerSample, WaveBufferEmptyHandler filler)
        {
            _Format = new WaveFormat(channels, rate, bitsPerSample);

            if (filler == null)
                throw new ArgumentNullException("filler");

            _FillerDelegate = filler;
        }

        public WavePlayer(Stream s)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            _DataStream = s;
            _Reader = new BinaryReader(_DataStream, System.Text.Encoding.ASCII);

            if (!SeekChunk("RIFF"))
                throw new SoundCoreException("Could not find 'RIFF' Chunk.");

            _Reader.ReadInt32();

            if (!SeekChunk("WAVE"))
                throw new SoundCoreException("Could not find 'WAVE' Chunk.");

            _Format = new WaveFormat(_DataStream);

            if (!SeekChunk("data"))
                throw new SoundCoreException("Could not find 'fmt ' Chunk.");

            _Reader.ReadInt32();

            _StartPosition = _DataStream.Position;
            _StreamBuffer = new byte[this.BufferSize];
            this._FillerDelegate = new WaveBufferEmptyHandler(BufferFiller);

        }

        private bool SeekChunk(string id)
        {
            if (_DataStream == null)
                throw new NullReferenceException("Cannot perform SeekChunk if _DataStream is null.");

            byte[] buffer = new byte[4];

            string temp;
            while (_DataStream.Length != _DataStream.Position)
            {
                _Reader.Read(buffer, 0, 4);
                temp = System.Text.Encoding.ASCII.GetString(buffer, 0, 4);
                if (temp == id)
                    return true;
            }

            return false;
        }

        private void AllocateBuffers()
        {
            WaveOutBuffer prev = null;
            this._RootBuffer = new WaveOutBuffer(this, this._DeviceHandle, this.BufferSize);

            prev = this._RootBuffer;

            for (int i = 1; i < this.BufferCount; i++)
            {
                prev.NextBuffer = new WaveOutBuffer(this, this._DeviceHandle, this.BufferSize);
                prev = prev.NextBuffer;
            }

            prev.NextBuffer = _RootBuffer;

            //			this._Buffers = new WaveOutBuffer[ this.BufferCount ];
            //
            //			for ( int i = 0; i < BufferCount; i++ )
            //				_Buffers[i] = new WaveOutBuffer( _DeviceHandle, this.BufferSize );
        }

        private void FreeBuffers()
        {
            WaveOutBuffer next;
            WaveOutBuffer current;

            current = _RootBuffer.NextBuffer;

            while (current != _RootBuffer)
            {
                next = current.NextBuffer;
                current.Dispose();

                current = next;
            }

            _RootBuffer.Dispose();
            _RootBuffer = null;
            //
            //			for ( int i = 0; i < BufferCount; i++ )
            //				_Buffers[i].Dispose();
            //
            //			_Buffers = null;
        }

        public void Play()
        {
#if DEBUG
            this._FillCount = 0;
#endif
            if (_DataStream != null)
            {
                _DataStream.Position = _StartPosition;
            }

            WaveFormatEx format = Format.GetFormat();

            int result = WaveFormNative.waveOutOpen(out _DeviceHandle, Device, ref format, _CallBack, 0,
                (int)DeviceOpenFlags.CallbackFunction);

            if (result != WaveError.MMSYSERR_NOERROR)
                throw new SoundCoreException(WaveError.GetMessage(result), result);

            _IsPlaying = true;

            AllocateBuffers();

            // Perform Initial fill
            WaveOutBuffer current = _RootBuffer;
            do
            {
                FillBuffer(current);
                current.BufferFilled();

                current = current.NextBuffer;
            } while (current != _RootBuffer);

            _RootBuffer.Play();
            //
            //			_FillThread = new Thread( new ThreadStart( FillProc ) );
            ////			_FillThread.Priority = ThreadPriority.Highest;
            //			_FillThread.Start();
            //
            //			_PlayThread = new Thread( new ThreadStart( PlayProc ) );
            //			_PlayThread.Start();
        }

        public void Stop()
        {
#if DEBUG
            Debug.WriteLine(string.Format("FilleBuffer called {0} times.", this._FillCount));
#endif

            if (IsPlaying)
            {
                _IsPlaying = false;

                _DonePlaying.WaitOne();

                //				// Force all buffers to open mutexs
                //				foreach(WaveOutBuffer buffer in _Buffers)
                //					buffer.Stop();
                //
                //				WaitForAllBuffers();
                //				
                //				_FillThread.Join();
                //				_PlayThread.Join();

                FreeBuffers();
            }
        }

        internal void FillBuffer(WaveOutBuffer buffer)
        {
#if DEBUG
            this._FillCount++;
#endif
            WaveBufferEmptyEventArgs args = new WaveBufferEmptyEventArgs(buffer.Data, buffer.Length);

            _FillerDelegate(buffer, args);

            // Stop playing if we no longer have data.
            if (args.BytesWritten == 0)
                Stop();
        }

        private void BufferFiller(object sender, WaveBufferEmptyEventArgs args)
        {
            lock (_StreamBuffer)
            {
                args.BytesWritten = _Reader.Read(_StreamBuffer, 0, args.Length);

                //				int index = 0;
                //				int length = args.Length;
                //
                //				while( index < length && _DataStream.Length != _DataStream.Position )
                //					_StreamBuffer[index++] = _Reader.ReadByte();
                //
                //				args.BytesWritten = length;
                //
                for (int i = args.BytesWritten, length = args.Length; i < length; i++)
                    _StreamBuffer[i] = 0;

                Marshal.Copy(_StreamBuffer, 0, args.Buffer, args.Length);
            }
        }
        //
        //		private void WaitForAllBuffers()
        //		{
        //			for ( int i = 0; i < BufferCount; i++ )
        //				_Buffers[i].WaitUntilCompleted();
        //		}

        internal void SignalDone()
        {
            this._DonePlaying.Set();
        }
        //
        //		private void PlayProc()
        //		{
        //			int index = 0;
        //
        //			// Loop as long as we have data to play and we haven't been told to stop.
        //			while(IsPlaying)
        //			{
        //				// Play the buffer
        //				_Buffers[index].Play();
        //
        //				// Wait until the buffer's done playing.
        //				_Buffers[index].WaitUntilCompleted();
        //
        //				// Move to next buffer
        //				index = ++index % BufferCount;
        //			}
        //		}
        //
        //
        //		private void FillProc()
        //		{
        //			int index = 0;
        //
        //			// Continue as long as we've got data.
        //			while ( IsPlaying )
        //			{
        //				// Wait until the buffer is empty
        //				_Buffers[index].WaitForBufferEmpty();
        //
        //				// Fill the current buffer
        //				FillBuffer( _Buffers[index] );
        //
        //				// Signal the buffer that it has data
        //				_Buffers[index].BufferFilled();
        ////				Thread.Sleep(0);
        //
        //				// Move to the next buffer
        //				index = ++index % BufferCount;
        //			}
        //		}
        //
        #region IDisposable Members

        public void Dispose()
        {
            Stop();
            if (_DataStream != null)
                _DataStream.Close();
        }

        #endregion
    }
}
