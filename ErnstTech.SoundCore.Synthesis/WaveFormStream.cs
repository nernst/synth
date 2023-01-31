using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ErnstTech.SoundCore.Synthesis
{
    public abstract class WaveFormStream : Stream
    {
        #region Public Properties
        /// <summary>
        ///     Inidicates if the stream can be read.
        /// </summary>
        /// <value>
        ///     Always returns <i>true</i>.
        /// </value>
        public override bool CanRead
        {
            get { return true; }
        }

        /// <summary>
        ///     Indicates if the stream can timeout.
        /// </summary>
        /// <value>
        ///     Always returns <i>false</i>.
        /// </value>
        public override bool CanTimeout
        {
            get { return false; }
        }

        /// <summary>
        ///     Indicates if the stream can be written to.
        /// </summary>
        /// <value>
        ///     Always returns <i>false</i>.
        /// </value>
        public override bool CanWrite
        {
            get { return false; }
        }

        /// <summary>
        ///     Indiciates if seeking is allowed on the stream.
        /// </summary>
        /// <value>
        ///     Always returns <i>false</i>.
        /// </value>
        public override bool CanSeek
        {
            get { return false; }
        }
        #endregion

        private long _Position = 0;
        /// <summary>
        ///     Gets the position in the stream.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///     Always thrown if the property is set.
        /// </exception>
        public override long Position
        {
            get
            {
                return _Position;
            }
            set
            {
                throw new NotSupportedException("Position cannot be set.");
            }
        }

        private long _SampleNumber = 0;
        public virtual long SampleNumber
        {
            get { return this._SampleNumber; }
        }

        private const int _BufferSize = 4096;
        private byte[] _Buffer = new byte[_BufferSize];

        private AudioMode _AudioMode;
        public AudioMode AudioMode
        {
            get { return _AudioMode; }
        }

        private AudioBits _AudioBits;
        public AudioBits AudioBits
        {
            get { return _AudioBits; }
        }

        protected WaveFormStream( AudioBits quality, AudioMode channels )
        {
            this._AudioBits = quality;
            this._AudioMode = channels;

            this.WriteHeader();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int ReadByte()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        private void FillBuffer()
        {
        }

        private void WriteHeader()
        {

        }
    }
}
