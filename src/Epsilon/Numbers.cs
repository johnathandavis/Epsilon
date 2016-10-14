using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epsilon
{
    public class Numbers
    {
        private static BigDecimal CreateFromString(string str)
        {
            return new BigDecimal(str);
        }

        private static Lazy<BigDecimal> _cNegOne = new Lazy<BigDecimal>(() => CreateFromString(NegOneStr));
        private static Lazy<BigDecimal> _cUnity = new Lazy<BigDecimal>(() => CreateFromString(UnityStr));
        private static Lazy<BigDecimal> _cZero = new Lazy<BigDecimal>(() => CreateFromString(ZeroStr));
        private static Lazy<BigDecimal> _cTwo = new Lazy<BigDecimal>(() => CreateFromString(TwoStr));
        private static Lazy<BigDecimal> _cTen = new Lazy<BigDecimal>(() => CreateFromString(TenStr));
        private static Lazy<BigDecimal> _cHalfPi = new Lazy<BigDecimal>(() => CreateFromString(HalfPiStr));
        private static Lazy<BigDecimal> _cPi = new Lazy<BigDecimal>(() => CreateFromString(PiStr));
        private static Lazy<BigDecimal> _cTwoPi = new Lazy<BigDecimal>(() => CreateFromString(TwoPiStr));
        private static Lazy<BigDecimal> _cEulerMascheroni = new Lazy<BigDecimal>(() => CreateFromString(EulerMascheroniStr));
        private static Lazy<BigDecimal> _cEuler = new Lazy<BigDecimal>(() => CreateFromString(EulerStr));
        private static Lazy<BigDecimal> _cSqrtTwo = new Lazy<BigDecimal>(() => CreateFromString(SqrtTwoStr));
        private static Lazy<BigDecimal> _cGoldenRatio = new Lazy<BigDecimal>(() => CreateFromString(GoldenRatioStr));
        private static Lazy<BigDecimal> _cDegree = new Lazy<BigDecimal>(() => CreateFromString(DegreeStr));

        public static BigDecimal NegOne { get { return _cNegOne.Value; } }
        public static BigDecimal Unity { get { return _cUnity.Value; } }
        public static BigDecimal Zero { get { return _cZero.Value; } }
        public static BigDecimal Two { get { return _cTwo.Value; } }
        public static BigDecimal Ten { get { return _cTen.Value; } }
        public static BigDecimal HalfPi { get { return _cHalfPi.Value; } }
        public static BigDecimal Pi { get { return _cPi.Value; } }
        public static BigDecimal TwoPi { get { return _cTwoPi.Value; } }
        public static BigDecimal EulerMascheroni { get { return _cEulerMascheroni.Value; } }
        public static BigDecimal Euler { get { return _cEuler.Value; } }
        public static BigDecimal SqrtTwo { get { return _cSqrtTwo.Value; } }
        public static BigDecimal GoldenRatio { get { return _cGoldenRatio.Value; } }
        public static BigDecimal Degree { get { return _cDegree.Value; } }


        private const String NegOneStr = "-1.0";
        private const String UnityStr = "1";
        private const String ZeroStr = "0";
        private const String TwoStr = "2";
        private const String TenStr = "10";
        private const String HalfPiStr = "1.57079632679489661923132169163975144209858469968755291048747229615";
        private const String PiStr = "3.14159265358979323846264338327950288419716939937510582097494459231";
        private const String TwoPiStr = "6.28318530717958647692528676655900576839433879875021164194988918462";
        private const String EulerMascheroniStr = "0.57721566490153286060651209008240243104215933593992359880576723488";
        private const String EulerStr = "2.71828182845904523536028747135266249775724709369995957496696762772";
        private const String SqrtTwoStr = "1.41421356237309504880168872420969807856967187537694807317667973799";
        private const String GoldenRatioStr = "1.61803398874989484820458683436563811772030917980576286213544862271";
        private const String DegreeStr = "0.01745329251994329576923690768488612713442871888541725456097191440";

    }
}
