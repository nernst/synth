using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace ErnstTech.Math
{
    public class FastFourierTransform
    {
        public static void Transform(int sign, Complex[] data)
        {
            long length = data.LongLength;
            double scale = System.Math.Sqrt(1.0 / length);

            long i, j;
            for (i = j = 0; i < length; ++i)
            {
                if (j >= i)
                {
                    double tempr = data[j].Real * scale;
                    double tempi = data[j].Imaginary * scale;
                    data[j] = new Complex(data[i].Real * scale, data[i].Imaginary * scale);
                    data[i] = new Complex(tempr * scale, tempi * scale);
                }
                long m = length / 2;
                while (m >= 1 && j >= m)
                {
                    j -= m;
                    m /= 2;
                }
                j += m;
            }

            for (long mmax = 1, istep = 2;
                mmax < length;
                mmax = istep, istep *= 2)
            {
                double delta = sign * System.Math.PI / mmax;
                for (long m = 0; m < mmax; ++m)
                {
                    double w = m * delta;
                    double wr = System.Math.Cos(w);
                    double wi = System.Math.Sin(w);
                    for (i = m; i < length && j < length; i += istep, j = i + mmax)
                    {
                        double tr = wr * data[j].Real - wi * data[j].Imaginary;
                        double ti = wr * data[j].Imaginary + wi * data[j].Real;
                        data[j] = new Complex(data[i].Real - tr, data[i].Imaginary - ti);
                        data[i] = new Complex(data[i].Real + tr, data[i].Imaginary + ti);
                    }
                }
                mmax = istep;
            }
        }
    }
}
