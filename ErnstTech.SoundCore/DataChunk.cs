using System;

namespace ErnstTech.SoundCore
{
    /// <summary>
    /// Summary description for DataChunk.
    /// </summary>
    public class DataChunk : Chunk
    {
        public override string ID => "data";

        internal DataChunk(byte[] data)
            : base(data)
        { }
    }
}
