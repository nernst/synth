using System;

namespace ConduciveTech.Math
{
	/// <summary>
	/// Summary description for Complex.
	/// </summary>
	public struct Complex
	{
        public double Real { get; init set; }
		public double Imaginary { get; set; }

		public double Magnitude { get; set; }

		public double Angle { get; set; }

        private Complex()
        {
        }

        public Complex(double real, double imaginary)
        {
            this._Real = real;
            this._Imaginary = imaginary;
        }

        public static Complex FromPolarCoordinates( double magnitude, double angle )
		{
			Complex comp = new Complex();

            comp._Magnitude = magnitude;
            comp._Angle = angle;
            comp._Real = (magnitude * System.Math.Cos(angle));
            comp._Imaginary = (magnitude * System.Math.Sin(angle));
            comp._State[MAGNITUDE_CALCULATED | ANGLE_CALCULATED] = true;

			return comp;
		}

        private void CalculateMagnitude()
        {
            this._Magnitude = System.Math.Sqrt(
                System.Math.Pow(this.Real, 2.0) +
                System.Math.Pow(this.Imaginary, 2.0));
            this._State[MAGNITUDE_CALCULATED] = true;
        }

        private void CalculateAngle()
        {
            this._Angle = System.Math.Atan(this.Imaginary / this.Real);
            this._State[ANGLE_CALCULATED] = true;
        }

        public static Complex operator !( Complex left )
        {
            return new Complex(left.Real, -left.Imaginary);
        }

		public static Complex operator +( Complex left, Complex right )
		{
			return new Complex( 
                (left.Real + right.Real),
                (left.Imaginary + right.Imaginary) );
		}

		public static Complex operator -( Complex left, Complex right )
		{
			return new Complex( 
                (left.Real - right.Real),
                (left.Imaginary - right.Imaginary) );
		}

		public static Complex operator *( Complex left, Complex right )
		{
			return new Complex( 
				( left.Real * right.Real - left.Imaginary * right.Imaginary ),
				( left.Real * right.Imaginary + left.Imaginary * right.Real ) );		
		}

		public static Complex operator /( Complex quotient, Complex divisor )
		{
			if ( divisor.Magnitude == 0 )
				throw new DivideByZeroException( "Divisor's magnitude is zero." );

			return Complex.FromPolarCoordinates( 
				(quotient.Magnitude / divisor.Magnitude), 
				(quotient.Angle - divisor.Angle ) );
		}

		public override int GetHashCode()
		{
			return (Real.GetHashCode() ^ Imaginary.GetHashCode());
		}

		public static bool operator ==( Complex left, Complex right )
		{
			return ((left.Real == right.Real) && (left.Imaginary == right.Imaginary));
		}

		public static bool operator !=( Complex left, Complex right )
		{
			return !(left == right);
		}

        public bool Equals(Complex complex)
        {
            return this == complex;
        }

        public override bool Equals(object obj)
		{
            if (!(obj is Complex))
                return false;

            return this.Equals((Complex)obj);
        }

		public override string ToString()
		{
			return string.Format( "{0} j{1}", this.Real, this.Imaginary );
		}

	}
}
