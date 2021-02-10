using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ErnstTech.SoundCore.Synthesis
{
    public class DigitalFilter
    {
        double[] _Source;

        public DigitalFilter(double[] source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            _Source = source;
        }

        public double[] Process( double[] sourceFactors, double[] feedbackFactors )
        {
            double[] retvalue = new double[_Source.LongLength];
            
            if (sourceFactors == null)
                sourceFactors = new double[0];
            if ( feedbackFactors == null )
                feedbackFactors = new double[0];

            long sourceCount = sourceFactors.LongLength;
            long feedbackCount = feedbackFactors.LongLength;

            for (long i = 0, length = _Source.LongLength; i < length; ++i )
            {
                double sample = this._Source[i];
                double value = sample;
                for (long h = 0; h < sourceCount; ++h)
                {
                    if (i < h || ( i - h ) >= sourceCount)
                        break;
                    value += sample * sourceFactors[i - h] * sample;
                }
                for (long h = 0; h < feedbackCount; ++h)
                {
                    if (i < h || ( i - h ) >= feedbackCount)
                        break;
                    value += sample * feedbackFactors[i - h] * sample;
                }

                retvalue[i] = value;
            }

            return retvalue;
        }

        double GetValue(int index, int offset)
        {
            long position = index - offset;
            return ( position < 0 ) ? 0.0 : _Source[position];
        }
    }
}
