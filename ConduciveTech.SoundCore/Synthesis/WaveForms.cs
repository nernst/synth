using System;

namespace ErnstTech.SoundCore.Synthesis
{
	/// <summary>
	/// Summary description for WaveForms.
	/// </summary>
	public sealed class WaveForms
	{
		private WaveForms()
		{}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="time"></param>
		/// <param name="peak"></param>
		/// <param name="frequency"></param>
		/// <param name="dutyCycle"></param>
		/// <returns></returns>
		public static int TriangleWave( long sample, int sampleRate, int peak, int frequency, float dutyCycle )
		{
			int point = GetPoint( sample, frequency );
			int period = GetPeriod( sampleRate, frequency );

			return (int)( peak * (double)((double)point /(double)period));
		}

		public static int SquareWave( long sample, int sampleRate, int peak, int frequency, float dutyCycle )
		{
			if ( dutyCycle < 0 || dutyCycle > 1 )
				throw new ArgumentOutOfRangeException( "dutyCycle", dutyCycle, "DutyCycle must be greater than or equal to zero and less than or equal to one." );

			int point = GetPoint( sample, frequency );
			int period = GetPeriod( sampleRate, frequency );

			return (((double)point/(double)period) < dutyCycle ) ? peak : 0;
		}

		public static int SineWave( long sample, int sampleRate, int peak, int frequency, float dutyCycle )
		{
			int point = GetPoint( sample, frequency );
			int period = GetPeriod( sampleRate, frequency );

			double ratio = (double)point / (double)period;
			double angle  = 2.0 * Math.PI * ratio;

			return (int)(peak * Math.Sin(angle));
		}

		/// <summary>
		/// Returns the period, in the number of samples, of the specified frequency
		/// </summary>
		/// <param name="sampleRate">Sample rate to be used.</param>
		/// <param name="frequency">Frequency to calculate the period for.</param>
		/// <returns>The period corresponding to the passed frequency, in samples.</returns>
		private static int GetPeriod( int sampleRate, int frequency )
		{
			return (int)((double)sampleRate / (double)frequency);
		}

		private static int GetPoint( long sample, int frequency )
		{
            if (frequency == 0)
                throw new ArgumentException( "Frequency cannot be zero.", "frequency" );
			return (int)( sample % frequency );
		}
	}
}
