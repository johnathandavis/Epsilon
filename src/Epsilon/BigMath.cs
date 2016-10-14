using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epsilon
{
    public class BigMath
    {
        /// <summary>
        /// Adapted from here: http://stackoverflow.com/questions/4124189/performing-math-operations-on-decimal-datatype-in-c
        /// </summary>
        /// <param name="x">The number to square root.</param>
        /// <returns>The approximated square root of x with a margin of error of less than Configuration.Epsilon</returns>
        public static BigDecimal Sqrt(BigDecimal x)
        {
            var epsilon = Configuration.Epsilon;

            if (x < Numbers.Unity) throw new OverflowException("Cannot calculate square root from a negative number");
            x = x.SetScale(Configuration.EpsilonPrecisionDigits+1);

            //x.Truncate(28);
            decimal approxOfX = decimal.Parse(x.ToString());
            decimal initialSqrtSeed = (decimal)System.Math.Sqrt((double)approxOfX);
            BigDecimal previous;
            BigDecimal current = new BigDecimal(initialSqrtSeed.ToString());
            do
            {
                previous = current;
                if (previous == Numbers.Zero) return Numbers.Zero;
                current = (previous + x / previous) / Numbers.Two;
            }
            while ((previous - current).Abs() > epsilon);
            return current;
        }
        private static BigDecimal NormalizeThetaBetweenZeroAndTwoPi(BigDecimal theta)
        {
            // first, reduce this to between 0 and 2Pi
            if (theta > Numbers.TwoPi || theta < Numbers.Zero)
                theta = theta % Numbers.TwoPi;

            return theta;
        }
        public static BigDecimal Sin(BigDecimal theta)
        {
            // calculate sine using the taylor series, the infinite sum of x^r/r! but to n iterations
            BigDecimal retVal = Numbers.Zero.SetScale(Configuration.EpsilonPrecisionDigits);

            theta = NormalizeThetaBetweenZeroAndTwoPi(theta);// .SetScale(Configuration.EpsilonPrecisionDigits+1);

            Boolean subtract = false;
            
            for (int r = 0; r < Configuration.TrigTaylorSeriesIterations; r++)
            {
                BigDecimal xPowerR = theta ^ (int)(2 * r + 1);
                BigDecimal factori = new BigDecimal(Factorial(2 * r + 1).ToString());

                BigDecimal element = xPowerR.SetScale(Configuration.EpsilonPrecisionDigits) / factori;

                BigDecimal addThis = subtract ? -element : element;
                
                retVal += addThis;
                subtract = !subtract;
            }
            
            return retVal;
        }
        public static BigDecimal Cos(BigDecimal theta)
        {
            // calculate sine using the taylor series, the infinite sum of x^r/r! but to n iterations
            BigDecimal retVal = Numbers.Zero.SetScale(Configuration.EpsilonPrecisionDigits);

            theta = NormalizeThetaBetweenZeroAndTwoPi(theta);// .SetScale(Configuration.EpsilonPrecisionDigits+1);

            Boolean subtract = false;

            for (int r = 0; r < Configuration.TrigTaylorSeriesIterations; r++)
            {
                BigDecimal xPowerR = theta ^ (int)(2 * r);
                BigDecimal factori = new BigDecimal(Factorial(2 * r).ToString());

                BigDecimal element = xPowerR.SetScale(Configuration.EpsilonPrecisionDigits) / factori;

                BigDecimal addThis = subtract ? -element : element;

                retVal += addThis;
                subtract = !subtract;
            }

            return retVal;
        }

        public static BigInteger Factorial(int intNum)
        {
            return Internal.FactorialCache.Factorial(intNum);
        }
    }
}
