using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsilon
{
    public class BigDecimal : Number, INumber<BigDecimal>
    {
        #region "Fields"

        public const int ROUND_CEILING = 2;
        public const int ROUND_DOWN = 1;
        public const int ROUND_FLOOR = 3;
        public const int ROUND_HALF_DOWN = 5;
        public const int ROUND_HALF_EVEN = 6;
        public const int ROUND_HALF_UP = 4;
        public const int ROUND_UNNECESSARY = 7;
        public const int ROUND_UP = 0;
        private const long serialVersionUID = -9030587172147296852;
        internal sbyte[] _bits;
        internal int _sign;
        internal int _scale;
        private BigDecimal _absOfthis;
        private BigDecimal _negate;
        private string _toString;
        private static readonly string DECIMAL_SEP;
        private double? _doubleOfthis;
        private float? _floatOfthis;
        private long? _longOfthis;

        #endregion

        #region "Constructors"

        public BigDecimal(double dval)
        {
            this._absOfthis = null;
            this._negate = null;
            this._toString = null;
            this._doubleOfthis = null;
            this._floatOfthis = null;
            this._longOfthis = null;
            if (double.IsInfinity(dval) || double.IsNaN(dval))
                throw new FormatException();
            long longBits = BitConverter.DoubleToInt64Bits(dval);
            long num1 = (longBits & long.MinValue) != 0L ? -1L : 1L;
            long num2 = (longBits & 9218868437227405312L) >> 52;
            long lnum = longBits & 4503599627370495L;
            if (num2 == 2047L)
                throw new FormatException();
            BigDecimal bigDecimal1;
            if (num2 == 0L)
            {
                // Not Sure. Was:
                // bigDecimal1 = lnum != 0L ? new BigDecimal(lnum.ToString()).Divide(new BigDecimal(IntegerBase.FromLong(2L).Pow(1074)), 1074, 2) : new BigDecimal("0");
                bigDecimal1 = lnum != 0L ? new BigDecimal(lnum.ToString()).Divide(new BigDecimal(BigInteger.FromLong(2L).Pow(1074).AsDouble()), 1074, 2) : new BigDecimal("0");
            }
            else
            {
                BigDecimal bigDecimal2 = new BigDecimal((lnum | 4503599627370496L).ToString());
                int exp = (int)num2 - 1075;
                if (exp < 0)
                {
                    // Not Sure. Was:
                    // BigDecimal val = new BigDecimal(IntegerBase.FromLong(2L).Pow(-exp));
                    BigDecimal val = new BigDecimal(BigInteger.FromLong(2L).Pow(-exp).ToString());
                    bigDecimal1 = bigDecimal2.Divide(val, -exp, 2);
                }
                else
                {
                    // Not Sure. Was:
                    // BigDecimal val = new BigDecimal(IntegerBase.FromLong(2L).Pow(exp));
                    BigDecimal val = new BigDecimal(BigInteger.FromLong(2L).Pow(exp).ToString());
                    bigDecimal1 = bigDecimal2.Multiply(val);
                }
            }
            string mrString = bigDecimal1.ToString();
            // VJSlibString.getString(10244)
            int num3 = mrString.IndexOf('.', 0);
            if (num3 != -1)
            {
                int index = mrString.Length - 1;
                while (index > num3 && (int)mrString[index] == 48)
                    index += -1;
                if (index < mrString.Length - 1)
                {
                    int newScale = index - num3;
                    bigDecimal1 = bigDecimal1.SetScale(newScale);
                }
            }
            if (num1 == -1L)
                bigDecimal1._sign = bigDecimal1._sign * -1;
            this._bits = bigDecimal1._bits;
            this._sign = bigDecimal1._sign;
            this._scale = bigDecimal1._scale;
        }

        public BigDecimal(string sval)
          : this(sval[0] != '-' ? 1 : -1, BigDecimal._parseBigDecimal(sval), sval.IndexOf('.') != -1 ? sval.Length - 1 - sval.IndexOf('.') : 0)
        {
        }

        public BigDecimal(BigInteger bval, int scale)
          : this(bval._sign, bval._bits, scale)
        {
        }

        private BigDecimal(int sign, sbyte[] magnitude, int scale)
        {
            _absOfthis = null;
            _negate = null;
            _toString = null;
            _doubleOfthis = null;
            _floatOfthis = null;
            _longOfthis = null;
            if (scale < 0)
                throw new FormatException();
            int length1 = magnitude.Length;
            int sourceIndex = 0;
            while (sourceIndex < length1 && magnitude[sourceIndex] == 0)
                ++sourceIndex;
            int length2 = length1 - sourceIndex;
            if (length2 < 0)
                throw new IndexOutOfRangeException();
            _bits = new sbyte[length2];
            _sign = _bits.Length != 0 ? sign : 0;
            Array.Copy(magnitude, sourceIndex, _bits, 0, _bits.Length);
            _scale = scale;
        }

        #endregion

        #region "Math"

        public BigDecimal Abs()
        {
            if (_absOfthis != null)
                return _absOfthis;
            if (_sign == -1)
                return _absOfthis = new BigDecimal(1, _bits, _scale);
            return _absOfthis = this;
        }
        public BigDecimal Add(BigDecimal val)
        {
            if (_sign == 0)
                return val;
            if (val._sign == 0)
                return this;
            sbyte[] op1 = _bits;
            sbyte[] numArray1 = val._bits;
            int scale = Math.Max(_scale, val._scale);
            if (this._scale < scale)
            {
                int length = 1;
                if (length < 0)
                    throw new IndexOutOfRangeException();
                sbyte[] bits = new sbyte[length];
                bits[0] = 10;
                int bitssign = 1;
                int exp = scale - _scale;
                sbyte[] op2 = BigDecimal._pow(bits, bitssign, exp);
                op1 = BigDecimal.Multiply(op1, op2);
            }
            if (val._scale < scale)
            {
                int length = 1;
                if (length < 0)
                    throw new IndexOutOfRangeException();
                sbyte[] bits = new sbyte[length];
                bits[0] = (sbyte)10;
                int bitssign = 1;
                int exp = scale - val._scale;
                sbyte[] op2 = BigDecimal._pow(bits, bitssign, exp);
                numArray1 = BigDecimal.Multiply(numArray1, op2);
            }
            int length1 = Math.Max(op1.Length, numArray1.Length) + 1;
            if (length1 < 0)
                throw new IndexOutOfRangeException();
            sbyte[] numArray2 = new sbyte[length1];
            return new BigDecimal(BigDecimal._subtractBitsArray(numArray2, op1, _sign, numArray1, val._sign), numArray2, scale);
        }
        public BigDecimal Subtract(BigDecimal val)
        {
            if (this._sign == 0)
                return new BigDecimal(val._sign * -1, val._bits, val._scale);
            if (val._sign == 0)
                return this;
            sbyte[] op1 = this._bits;
            sbyte[] numArray1 = val._bits;
            int scale = Math.Max(this._scale, val._scale);
            if (this._scale < scale)
            {
                int length = 1;
                if (length < 0)
                    throw new IndexOutOfRangeException();
                sbyte[] bits = new sbyte[length];
                bits[0] = (sbyte)10;
                int bitssign = 1;
                int exp = scale - this._scale;
                sbyte[] op2 = BigDecimal._pow(bits, bitssign, exp);
                op1 = BigDecimal.Multiply(op1, op2);
            }
            if (val._scale < scale)
            {
                int length = 1;
                if (length < 0)
                    throw new IndexOutOfRangeException();
                sbyte[] bits = new sbyte[length];
                bits[0] = (sbyte)10;
                int bitssign = 1;
                int exp = scale - val._scale;
                sbyte[] op2 = BigDecimal._pow(bits, bitssign, exp);
                numArray1 = BigDecimal.Multiply(numArray1, op2);
            }
            int length1 = Math.Max(op1.Length, numArray1.Length) + 1;
            if (length1 < 0)
                throw new IndexOutOfRangeException();
            sbyte[] numArray2 = new sbyte[length1];
            return new BigDecimal(BigDecimal._subtractBitsArray(numArray2, op1, this._sign, numArray1, -1 * val._sign), numArray2, scale);
        }
        public BigDecimal Multiply(BigDecimal val)
        {
            int num1 = BigDecimal.GetBitLength(val._bits, val._sign);
            int num2 = BigDecimal.GetBitLength(this._bits, this._sign);
            int sign1 = this._sign;
            sbyte[] op1;
            if (num2 % 8 == 0)
            {
                int length = this._bits.Length + 1;
                if (length < 0)
                    throw new IndexOutOfRangeException();
                op1 = new sbyte[length];
                Array.Copy((Array)this._bits, 0, (Array)op1, 1, this._bits.Length);
            }
            else
                op1 = this._bits;
            int sign2 = val._sign;
            sbyte[] op2;
            if (num1 % 8 == 0)
            {
                int length = val._bits.Length + 1;
                if (length < 0)
                    throw new IndexOutOfRangeException();
                op2 = new sbyte[length];
                Array.Copy((Array)val._bits, 0, (Array)op2, 1, val._bits.Length);
            }
            else
                op2 = val._bits;
            int length1 = op1.Length + op2.Length;
            if (length1 < 0)
                throw new IndexOutOfRangeException();
            sbyte[] numArray = new sbyte[length1];
            return new BigDecimal(BigDecimal.Multiply(numArray, op1, sign1, op2, sign2), numArray, this._scale + val._scale);
        }
        public BigDecimal Divide(BigDecimal val)
        {
            return this.Divide(val, this._scale, 0);
        }
        public BigDecimal Divide(BigDecimal val, int roundingMode)
        {
            return this.Divide(val, this._scale, roundingMode);
        }
        public BigDecimal Divide(BigDecimal val, int newScale, int roundingMode)
        {
            if (val._bits.Length == 0)
                throw new ArithmeticException();
            if (roundingMode != 2 && roundingMode != 1 && (roundingMode != 3 && roundingMode != 5) && (roundingMode != 6 && roundingMode != 4 && (roundingMode != 7 && roundingMode != 0)))
                throw new ArgumentException();
            BigDecimal bigDecimal;
            if (this._bits.Length == 0)
            {
                int sign = 0;
                int length = 1;
                if (length < 0)
                    throw new IndexOutOfRangeException();
                sbyte[] magnitude = new sbyte[length];
                magnitude[0] = 0;
                int scale = newScale;
                bigDecimal = new BigDecimal(sign, magnitude, scale);
            }
            else
            {
                int num1 = 2;
                int length1 = 1;
                int length2 = num1;
                if (length1 < 0 || length2 < 0)
                    throw new IndexOutOfRangeException();
                sbyte[][] result = ArrayHelper.InitMultiDArray<sbyte>(length2, length1);
                int num2 = newScale - (_scale - val._scale);
                sbyte[] dividendBits1 = _bits;
                if (num2 > 0)
                {
                    int length3 = 1;
                    if (length3 < 0)
                        throw new IndexOutOfRangeException();
                    sbyte[] bits = new sbyte[length3];
                    bits[0] = 10;
                    int bitssign = 1;
                    int exp = num2;
                    dividendBits1 = BigDecimal.Multiply(_bits, BigDecimal._pow(bits, bitssign, exp));
                }
                sbyte[] divisorBits = val._bits;
                int num3 = BigDecimal.DivideAndRemainder(result, dividendBits1, _sign, divisorBits, val._sign);
                if (num2 < 0)
                {
                    sbyte[] dividendBits2 = result[0];
                    int length3 = 1;
                    if (length3 < 0)
                        throw new IndexOutOfRangeException();
                    sbyte[] bits = new sbyte[length3];
                    bits[0] = 10;
                    int bitssign = 1;
                    int exp = -num2;
                    divisorBits = BigDecimal._pow(bits, bitssign, exp);
                    num3 = BigDecimal.DivideAndRemainder(result, dividendBits2, num3, divisorBits, 1);
                }
                BigDecimal._applyRoundingMode(result, num3, divisorBits, roundingMode);
                bigDecimal = new BigDecimal(num3, result[0], newScale);
            }
            return bigDecimal;
        }
        public BigDecimal DivRemainder(BigDecimal modulo)
        {
            var div = this / modulo;
            var intDiv = new BigDecimal(div.ToBigInteger().ToString());
            return this - (modulo * intDiv);
        }
        public BigDecimal Max(BigDecimal val)
        {
            sbyte[] numArray1 = this._bits;
            sbyte[] numArray2 = val._bits;
            if (this._scale != val._scale)
            {
                if (this._scale > val._scale)
                {
                    int num = this._scale - val._scale;
                    int length = 1;
                    if (length < 0)
                        throw new IndexOutOfRangeException();
                    sbyte[] bits = new sbyte[length];
                    bits[0] = (sbyte)10;
                    int bitssign = 1;
                    int exp = num;
                    sbyte[] op2 = BigDecimal._pow(bits, bitssign, exp);
                    if (numArray2.Length != 0)
                        numArray2 = BigDecimal.Multiply(numArray2, op2);
                }
                else if (this._scale < val._scale)
                {
                    int num = val._scale - this._scale;
                    int length = 1;
                    if (length < 0)
                        throw new IndexOutOfRangeException();
                    sbyte[] bits = new sbyte[length];
                    bits[0] = (sbyte)10;
                    int bitssign = 1;
                    int exp = num;
                    sbyte[] op2 = BigDecimal._pow(bits, bitssign, exp);
                    if (numArray1.Length != 0)
                        numArray1 = BigDecimal.Multiply(numArray1, op2);
                }
            }
            if (BigDecimal.CompareTo(this._sign, numArray1, val._sign, numArray2) < 0)
                return val;
            return this;
        }
        public BigDecimal Min(BigDecimal val)
        {
            sbyte[] numArray1 = this._bits;
            sbyte[] numArray2 = val._bits;
            if (this._scale != val._scale)
            {
                if (this._scale > val._scale)
                {
                    int num = this._scale - val._scale;
                    int length = 1;
                    if (length < 0)
                        throw new IndexOutOfRangeException();
                    sbyte[] bits = new sbyte[length];
                    bits[0] = (sbyte)10;
                    int bitssign = 1;
                    int exp = num;
                    sbyte[] op2 = BigDecimal._pow(bits, bitssign, exp);
                    if (numArray2.Length != 0)
                        numArray2 = BigDecimal.Multiply(numArray2, op2);
                }
                else if (this._scale < val._scale)
                {
                    int num = val._scale - this._scale;
                    int length = 1;
                    if (length < 0)
                        throw new IndexOutOfRangeException();
                    sbyte[] bits = new sbyte[length];
                    bits[0] = (sbyte)10;
                    int bitssign = 1;
                    int exp = num;
                    sbyte[] op2 = BigDecimal._pow(bits, bitssign, exp);
                    if (numArray1.Length != 0)
                        numArray1 = BigDecimal.Multiply(numArray1, op2);
                }
            }
            if (BigDecimal.CompareTo(this._sign, numArray1, val._sign, numArray2) < 0)
                return this;
            return val;
        }
        public BigDecimal Negate()
        {
            return this._negate != null ? this._negate : (this._negate = new BigDecimal(this._sign * -1, this._bits, this._scale));
        }
        public int SigNum()
        {
            return this._sign;
        }

        #endregion

        #region "Bitwise Operations"

        public BigDecimal MovePointLeft(int n)
        {
            if (n < 0)
                return this.MovePointRight(-n);
            return new BigDecimal(this._sign, this._bits, this._scale + n);
        }
        public BigDecimal MovePointRight(int n)
        {
            if (n < 0)
                return this.MovePointLeft(-n);
            int scale = this._scale - n;
            if (scale >= 0)
                return new BigDecimal(this._sign, this._bits, scale);
            sbyte[] magnitude;
            if (this._bits.Length == 0)
            {
                int length = 0;
                if (length < 0)
                    throw new IndexOutOfRangeException();
                magnitude = new sbyte[length];
            }
            else
            {
                int length = 1;
                if (length < 0)
                    throw new IndexOutOfRangeException();
                sbyte[] bits = new sbyte[length];
                bits[0] = (sbyte)10;
                int bitssign = 1;
                int exp = -scale;
                magnitude = BigDecimal.Multiply(this._bits, BigDecimal._pow(bits, bitssign, exp));
            }
            return new BigDecimal(this._sign, magnitude, 0);
        }

        #endregion

        #region "Scale"

        public int GetScale()
        {
            return this._scale;
        }
        public BigDecimal SetScale(int newScale)
        {
            return this.SetScale(newScale, 7);
        }
        public BigDecimal SetScale(int newScale, int roundingMode)
        {
            if (newScale < 0)
                throw new ArithmeticException("Negative scale in SetScale method.");
            if (roundingMode != 2 && roundingMode != 1 && (roundingMode != 3 && roundingMode != 5) && (roundingMode != 6 && roundingMode != 4 && (roundingMode != 7 && roundingMode != 0)))
                throw new ArgumentException("Invalid rounding mode.");
            if (newScale == _scale)
                return this;
            if (newScale > _scale)
            {
                int length = 1;
                if (length < 0)
                    throw new IndexOutOfRangeException();
                sbyte[] bits = new sbyte[length];
                bits[0] = (sbyte)10;
                int bitssign = 1;
                int exp = newScale - this._scale;
                return new BigDecimal(this._sign, BigDecimal.Multiply(this._bits, BigDecimal._pow(bits, bitssign, exp)), newScale);
            }
            int num1 = this._scale - newScale;
            int num2 = 2;
            int length1 = 1;
            int length2 = num2;
            if (length1 < 0 || length2 < 0)
                throw new IndexOutOfRangeException();
            sbyte[][] result = ArrayHelper.InitMultiDArray<sbyte>(length2, length1);
            int length3 = 1;
            if (length3 < 0)
                throw new IndexOutOfRangeException();
            sbyte[] bits1 = new sbyte[length3];
            bits1[0] = (sbyte)10;
            int bitssign1 = 1;
            int exp1 = num1;
            sbyte[] divisorBits = BigDecimal._pow(bits1, bitssign1, exp1);
            int num3 = BigDecimal.DivideAndRemainder(result, this._bits, this._sign, divisorBits, 1);
            BigDecimal._applyRoundingMode(result, num3, divisorBits, roundingMode);
            return new BigDecimal(num3, result[0], newScale);
        }

        #endregion

        #region "Conversions"

        public override double AsDouble()
        {
            if (this._doubleOfthis == null)
                _doubleOfthis = double.Parse(this.ToString());
            return _doubleOfthis.Value;
        }
        public override float AsFloat()
        {
            if (this._floatOfthis == null)
                this._floatOfthis = float.Parse(this.ToString());
            return this._floatOfthis.Value;
        }
        public override int AsInt()
        {
            return (int)this.AsLong();
        }
        public override long AsLong()
        {
            if (this._longOfthis != null)
                return this._longOfthis.Value;

            this._longOfthis = this.ToBigInteger().AsLong();
            return this.ToBigInteger().AsLong();
        }
        public BigInteger ToBigInteger()
        {
            sbyte[] bits1 = this._bits;
            if (this._scale > 0)
            {
                int length1 = 1;
                if (length1 < 0)
                    throw new IndexOutOfRangeException();
                sbyte[] bits2 = new sbyte[length1];
                bits2[0] = (sbyte)10;
                int bitssign = 1;
                int scale = this._scale;
                sbyte[] divisorBits = BigDecimal._pow(bits2, bitssign, scale);
                int num = 2;
                int length2 = 1;
                int length3 = num;
                if (length2 < 0 || length3 < 0)
                    throw new IndexOutOfRangeException();
                sbyte[][] result = ArrayHelper.InitMultiDArray<sbyte>(length3, length2);
                BigDecimal.DivideAndRemainder(result, bits1, this._sign, divisorBits, 1);
                bits1 = result[0];
            }
            return new BigInteger(this._sign, bits1);
        }
        public static BigDecimal FromLong(long val)
        {
            return BigDecimal.FromLong(val, 0);
        }
        public static BigDecimal FromLong(long val, int scale)
        {
            if (scale < 0)
                throw new FormatException("Invalid scale.");
            int sign = 1;
            if ((val & long.MinValue) == long.MinValue)
            {
                sign = -1;
                val = long.MinValue - (val & long.MaxValue);
            }
            int length = 8;
            if (length < 0)
                throw new IndexOutOfRangeException();
            sbyte[] magnitude = new sbyte[length];
            int num1 = 56;
            long num2 = -72057594037927936;
            int index = 0;
            int num3 = 0;
            while (num1 >= 0)
            {
                magnitude[index] = (sbyte)((val & num2) >> num1 & (long)byte.MaxValue);
                ++num3;
                num1 -= 8;
                num2 = (long)((ulong)num2 >> 8);
                ++index;
            }
            return new BigDecimal(sign, magnitude, scale);
        }

        #endregion

        #region "Object Overrides"

        public int CompareTo(object obj)
        {
            return this.CompareTo((object)(BigDecimal)obj);
        }
        public int CompareTo(BigDecimal val)
        {
            int sign1 = _sign;
            int sign2 = val._sign;
            sbyte[] numArray1 = _bits;
            sbyte[] numArray2 = val._bits;
            if (_scale != val._scale)
            {
                if (_scale > val._scale)
                {
                    int num = _scale - val._scale;
                    int length = 1;
                    if (length < 0)
                        throw new IndexOutOfRangeException();
                    sbyte[] bits = new sbyte[length];
                    bits[0] = 10;
                    int bitssign = 1;
                    int exp = num;
                    sbyte[] op2 = BigDecimal._pow(bits, bitssign, exp);
                    if (numArray2.Length != 0)
                        numArray2 = BigDecimal.Multiply(numArray2, op2);
                }
                else if (_scale < val._scale)
                {
                    int num = val._scale - _scale;
                    int length = 1;
                    if (length < 0)
                        throw new IndexOutOfRangeException();
                    sbyte[] bits = new sbyte[length];
                    bits[0] = 10;
                    int bitssign = 1;
                    int exp = num;
                    sbyte[] op2 = BigDecimal._pow(bits, bitssign, exp);
                    if (numArray1.Length != 0)
                        numArray1 = BigDecimal.Multiply(numArray1, op2);
                }
            }
            return BigDecimal.CompareTo(sign1, numArray1, sign2, numArray2);
        }
        public override bool Equals(object val)
        {
            return this == val || val is BigDecimal && this._scale == ((BigDecimal)val)._scale && BigDecimal.CompareTo(this._sign, this._bits, ((BigDecimal)val)._sign, ((BigDecimal)val)._bits) == 0;
        }
        public override int GetHashCode()
        {
            return this.AsInt();
        }
        public override string ToString()
        {
            if (this._toString != null)
                return this._toString;
            StringBuilder strBuilder = new StringBuilder();
            bool flag1 = false;
            if (this._sign == -1)
                flag1 = true;
            BigInteger radix = BigInteger._radixArray[10];
            BigInteger integerBase = new BigInteger(this._sign, this._bits);
            bool flag2 = false;
            while (!flag2)
            {
                BigInteger[] IntegerBaseArray = integerBase.DivideAndRemainder(radix);
                if (IntegerBaseArray[0]._bits.Length == 0)
                    flag2 = true;
                if (flag1)
                    strBuilder.Append(BigInteger._charRepresentation[IntegerBaseArray[1].Negate().AsInt()]);
                else
                    strBuilder.Append(BigInteger._charRepresentation[IntegerBaseArray[1].AsInt()]);
                integerBase = IntegerBaseArray[0];
            }
            if (this._scale > 0)
            {
                if (strBuilder.Length - 1 < _scale)
                {
                    int num = strBuilder.Length;
                    strBuilder.Length = _scale + 1;
                    for (int index = num; index < strBuilder.Length; ++index)
                    {
                        string str = strBuilder.ToString();
                        strBuilder.Replace(str[index], '0', index, 1);
                    }
                }
                strBuilder.Insert(this._scale, ".");
            }
            if (flag1)
                strBuilder.Append("-");
            string beforeReverse = strBuilder.ToString();
            this._toString = new string(beforeReverse.Reverse().ToArray());
            return this._toString;
        }

        #endregion

        #region "Internal Math"

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static sbyte[] _addOneToBitsArray(sbyte[] op)
        {
            sbyte[] numArray1 = op;
            sbyte num1 = 1;
            int index = op.Length - 1;
            while ((int)num1 > 0 && index >= 0)
            {
                int num2 = ((int)op[index] & (int)byte.MaxValue) + ((int)num1 & (int)byte.MaxValue);
                op[index] = (sbyte)(num2 & (int)byte.MaxValue);
                num1 = (sbyte)((num2 & 256) >> 8);
                index += -1;
            }
            if ((int)num1 > 0 && index < 0)
            {
                int length = numArray1.Length + 1;
                if (length < 0)
                    throw new IndexOutOfRangeException();
                sbyte[] numArray2 = new sbyte[length];
                numArray2[0] = num1;
                Array.Copy((Array)numArray1, 0, (Array)numArray2, 1, numArray1.Length);
                numArray1 = numArray2;
            }
            return numArray1;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private static void _applyRoundingMode(sbyte[][] result, int quotientSign, sbyte[] divisorBits, int roundingMode)
        {
            sbyte[] op = result[0];
            sbyte[] numArray1 = result[1];
            bool flag = false;
            for (int index = 0; index < numArray1.Length; ++index)
            {
                if ((int)numArray1[index] != 0)
                {
                    flag = true;
                    break;
                }
            }
            if (roundingMode == 7)
            {
                if (flag)
                    throw new ArithmeticException("Invalid rounding mode and flag is true.");
            }
            else
            {
                if (roundingMode == 1)
                    return;
                sbyte[] bitsArray;
                if (roundingMode == 0)
                {
                    if (!flag)
                        return;
                    bitsArray = BigDecimal._addOneToBitsArray(op);
                }
                else if (roundingMode == 2)
                {
                    if (quotientSign != 1 || !flag)
                        return;
                    bitsArray = BigDecimal._addOneToBitsArray(op);
                }
                else if (roundingMode == 3)
                {
                    if (quotientSign != -1 || !flag)
                        return;
                    bitsArray = BigDecimal._addOneToBitsArray(op);
                }
                else
                {
                    int length = Math.Max(divisorBits.Length, numArray1.Length) + 1;
                    if (length < 0)
                        throw new IndexOutOfRangeException();
                    sbyte[] numArray2 = new sbyte[length];
                    BigDecimal._subtractBitsArray(numArray2, divisorBits, 1, numArray1, -1);
                    int num = BigDecimal.CompareTo(1, numArray2, 1, numArray1);
                    if (roundingMode == 5)
                    {
                        if (num != -1)
                            return;
                        bitsArray = BigDecimal._addOneToBitsArray(op);
                    }
                    else if (roundingMode == 4)
                    {
                        if (num == 1)
                            return;
                        bitsArray = BigDecimal._addOneToBitsArray(op);
                    }
                    else
                    {
                        if (roundingMode != 6)
                            return;
                        if (num == -1)
                        {
                            bitsArray = BigDecimal._addOneToBitsArray(op);
                        }
                        else
                        {
                            if (num != 0 || ((int)op[op.Length - 1] & 1) == 0)
                                return;
                            bitsArray = BigDecimal._addOneToBitsArray(op);
                        }
                    }
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static int GetBitLength(sbyte[] bits, int sign)
        {
            return BigInteger.GetBitLength(bits, sign);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static int CompareTo(int signx, sbyte[] xbits, int signy, sbyte[] ybits)
        {
            return BigInteger.CompareTo(signx, xbits, signy, ybits);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private static int DivideAndRemainder(sbyte[][] result, sbyte[] dividendBits, int dividendSign, sbyte[] divisorBits, int divisorSign)
        {
            int num1 = BigDecimal.GetBitLength(divisorBits, divisorSign);
            sbyte[] dividend;
            if (BigDecimal.GetBitLength(dividendBits, dividendSign) % 8 == 0)
            {
                int length = dividendBits.Length + 1;
                if (length < 0)
                    throw new IndexOutOfRangeException();
                dividend = new sbyte[length];
                Array.Copy((Array)dividendBits, 0, (Array)dividend, 1, dividendBits.Length);
            }
            else
                dividend = dividendBits;
            sbyte[] divisor;
            if (num1 % 8 == 0)
            {
                int length = divisorBits.Length + 1;
                if (length < 0)
                    throw new IndexOutOfRangeException();
                divisor = new sbyte[length];
                Array.Copy((Array)divisorBits, 0, (Array)divisor, 1, divisorBits.Length);
            }
            else
                divisor = divisorBits;
            int length1 = divisor.Length + dividend.Length;
            if (length1 < 0)
                throw new IndexOutOfRangeException();
            sbyte[] res = new sbyte[length1];
            int num2 = BigInteger._divide(res, dividend, dividendSign, divisor, divisorSign);
            sbyte[][] numArray1 = result;
            int index1 = 0;
            int length2 = dividend.Length;
            if (length2 < 0)
                throw new IndexOutOfRangeException();
            sbyte[] numArray2 = new sbyte[length2];
            numArray1[index1] = numArray2;
            sbyte[][] numArray3 = result;
            int index2 = 1;
            int length3 = divisor.Length;
            if (length3 < 0)
                throw new IndexOutOfRangeException();
            sbyte[] numArray4 = new sbyte[length3];
            numArray3[index2] = numArray4;
            Array.Copy((Array)res, divisor.Length, (Array)result[0], 0, dividend.Length);
            Array.Copy((Array)res, 0, (Array)result[1], 0, divisor.Length);
            return num2;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static sbyte[] Multiply(sbyte[] op1, sbyte[] op2)
        {
            return BigInteger._multiply(op1, op2);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static int Multiply(sbyte[] res, sbyte[] op1, int op1sign, sbyte[] op2, int op2sign)
        {
            return BigInteger._multiply(res, op1, op1sign, op2, op2sign);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private static sbyte[] _parseBigDecimal(string str)
        {
            if (str == null || str == string.Empty || (str == BigDecimal.DECIMAL_SEP || str.Length == 0) || str.LastIndexOf(' ') != -1)
                throw new FormatException(str);
            int num1 = 0;
            int num2 = str.Length;
            if ((int)str[0] == 45)
                ++num1;
            int num3 = ((num2 - num1) * 4 + 7) / 8;
            int length1 = num3 + 1;
            if (length1 < 0)
                throw new IndexOutOfRangeException();
            sbyte[] numArray1 = new sbyte[length1];
            int length2 = num3 + 1;
            if (length2 < 0)
                throw new IndexOutOfRangeException();
            sbyte[] op = new sbyte[length2];
            BigInteger radix1 = BigInteger._radixArray[10];
            bool flag = false;
            for (int index = num1; index < num2; ++index)
            {
                if ((int)str[index] == 46)
                {
                    if (flag)
                        throw new FormatException();
                    flag = true;
                }
                else
                {
                    long num4 = Convert.ToInt64(str[index].ToString(), 10);
                    if (num4 < 0L)
                        throw new FormatException(str);
                    BigInteger radix2 = BigInteger._radixArray[(int)num4];
                    if (index > num1)
                    {
                        sbyte[] numArray2 = numArray1;
                        numArray1 = op;
                        op = numArray2;
                        Array.Clear((Array)numArray1, 0, numArray1.Length);
                        int n = radix1.GetBitLength() - 1;
                        while (n >= 0)
                        {
                            if (radix1.TestBit(n))
                                BigInteger.AddBitsArray(numArray1, op);
                            if (n > 0)
                                BigInteger._shiftLeftOneBit(numArray1);
                            n += -1;
                        }
                    }
                    BigInteger.AddBitsArray(numArray1, radix2._bits);
                }
            }
            return numArray1;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static sbyte[] _pow(sbyte[] bits, int bitssign, int exp)
        {
            return BigInteger._pow(bits, bitssign, exp);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static int _subtractBitsArray(sbyte[] res, sbyte[] op1, int op1sign, sbyte[] op2, int op2sign)
        {
            return BigInteger._subtractBitsArray(res, op1, op1sign, op2, op2sign);
        }

        #endregion

        #region "Operators"

        public static bool operator <(BigDecimal d1, BigDecimal d2)
        {
            return d1.CompareTo(d2) < 0;
        }
        public static bool operator >(BigDecimal d1, BigDecimal d2)
        {
            return d1.CompareTo(d2) > 0;
        }
        public static bool operator ==(BigDecimal d1, BigDecimal d2)
        {
            if ((object)(d1) == null)
            {
                return ((object)d2) == null;
            }
            return d1.Equals(d2);
        }
        public static bool operator !=(BigDecimal d1, BigDecimal d2)
        {
            return !(d1 == d2);
        }
        public static BigDecimal operator -(BigDecimal d)
        {
            return d * Numbers.NegOne;
        }
        public static BigDecimal operator +(BigDecimal d1, BigDecimal d2)
        {
            int maxScale = Math.Max(d1.GetScale(), d2.GetScale());
            return d1.SetScale(maxScale).Add(d2.SetScale(maxScale));
        }
        public static BigDecimal operator -(BigDecimal d1, BigDecimal d2)
        {
            int maxScale = Math.Max(d1.GetScale(), d2.GetScale());
            return d1.SetScale(maxScale).Subtract(d2.SetScale(maxScale));
        }
        public static BigDecimal operator *(BigDecimal d1, BigDecimal d2)
        {
            int maxScale = Math.Max(d1.GetScale(), d2.GetScale());
            return d1.SetScale(maxScale).Multiply(d2.SetScale(maxScale));
        }
        public static BigDecimal operator /(BigDecimal d1, BigDecimal d2)
        {
            int maxScale = Math.Max(d1.GetScale(), d2.GetScale());
            return d1.SetScale(maxScale).Divide(d2.SetScale(maxScale));
        }
        public static BigDecimal operator %(BigDecimal d1, BigDecimal d2)
        {
            int maxScale = Math.Max(d1.GetScale(), d2.GetScale());
            return d1.SetScale(maxScale).DivRemainder(d2.SetScale(maxScale));
        }
        public static BigDecimal operator ^(BigDecimal d1, int exp)
        {
            return new BigDecimal(1, BigDecimal._pow( d1._bits, d1._sign, exp), d1.GetScale());
        }

        #endregion
    }
}
