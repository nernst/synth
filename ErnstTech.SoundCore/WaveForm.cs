using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace ErnstTech.SoundCore
{
	/// <summary>
	/// Summary description for WaveForm.
	/// </summary>
	public class WaveForm
	{
        public WaveFormat Format { get; private set; }
        public List<Point> Points { get; private set; }

		private int _BaseFrequency;
		public int BaseFrequency
		{
			get{ return this._BaseFrequency; }
			set
			{
				if ( value <= 0 )
					value = 1;
				this._BaseFrequency = value;
			}
		}

		public WaveForm( WaveFormat format )
		{
			if ( format == null )
				throw new ArgumentNullException( "format" );

			this.Format = format;
            this.Format.Channels = 1;
			this.Points = new List<Point>();
		}

		private void Validate()
		{
            //if ( Points.Count < 3 )
            //    throw new SoundCoreException( "Can only generate a wave form with at least 3 points." );
		}

		public Stream GenerateWave()
		{
			Validate();

			int duration = 0;				// Number of samples to generate
			int pointCount = Points.Count;

			for ( int i = 0; i < pointCount; i++ )
				duration += Points[i].X;

			int dataSize = (duration * Format.BlockAlignment );
			Stream ms = new MemoryStream( dataSize + WaveFormat.HeaderSize );

			Format.WriteHeader( ms, dataSize );

            Stream dataStream = new MemoryStream(dataSize);

            dataStream.Position = 0;
            double[] test = new Synthesis.SineWave(200).Generate(44000);
            Synthesis.DigitalFilter filter = new ErnstTech.SoundCore.Synthesis.DigitalFilter(test);
            double[] filtered = filter.Process(
                new double[]{
                    -2.0,
                    -1.5,
                    1.0,
                    -.5,
                    0.25
                },
                new double[]{
                });

            Normalize(filtered);
            short[] quant = QuantizeShort(filtered);

            for (long i = 0, len = quant.LongLength; i < len; ++i)
            {
                ms.WriteByte((byte)(quant[i] & 0xff));
                ms.WriteByte((byte)((quant[i] >> 8) & 0xff));
            }

			ms.Position = 0;
			return ms;
		}

        void Normalize( double[] data )
        {
            double max_value = 0;

            foreach (double d in data)
            {
                double abs = Math.Abs(d);
                if (abs > max_value)
                    max_value = abs;
            }

            for (long i = 0, length = data.LongLength; i < length; ++i)
                data[i] = max_value == 0 ? 0.0 : data[i] / max_value;
        }

        IEnumerator<sbyte> QuantizeByte(IEnumerator<double> e)
        {
            while (e.MoveNext())
            {
                yield return Convert.ToSByte(e.Current * sbyte.MaxValue);
            }
        }

        IEnumerator<short> QuantizeShort(IEnumerator<double> e)
        {
            while (e.MoveNext())
            {
                yield return Convert.ToInt16(e.Current * short.MaxValue);
            }
        }

        sbyte[] QuantizeByte(double[] data)
        {
            sbyte[] retvalue = new sbyte[data.LongLength];
            for (long i = 0, len = data.LongLength; i < len; ++i)
            {
                retvalue[i] = Convert.ToSByte(data[i] * sbyte.MaxValue);
            }
            return retvalue;
        }

        short[] QuantizeShort(double[] data)
        {
            short[] retvalue = new short[data.LongLength];
            for (long i = 0, len = data.LongLength; i < len; ++i)
            {
                retvalue[i] = Convert.ToInt16(data[i] * short.MaxValue);
            }
            return retvalue;
        }
        
    }
}
