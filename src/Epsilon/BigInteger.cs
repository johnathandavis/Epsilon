using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Epsilon
{
    public class BigInteger : Number, INumber<BigInteger>
    {
        #region "Fields"

        private const long serialVersionUID = -8336172741129200402;
        internal sbyte[] _bits;
        internal int _sign;
        private int _count;
        private int _length;
        private int _lowestSetBit;
        private int _leadingZeroCount;
        private BigInteger _absOfthis;
        private BigInteger _negate;
        private BigInteger _not;
        private string _toString;
        private double? _doubleOfthis;
        private float? _floatOfthis;
        private long? _longOfthis;
        internal static bool isLocked;
        internal static readonly char[] _charRepresentation;
        internal static readonly BigInteger[] _radixArray;

        #endregion

        #region "Constructors"

        static BigInteger()
        {
            if (isLocked) return;
            isLocked = true;
            int radixSize = 37;
            _radixArray = new BigInteger[radixSize];
            _charRepresentation = new char[radixSize];
            for (int x = 0; x < radixSize; x++)
            {
                if (x < 10)
                {
                    _charRepresentation[x] = x.ToString().First();
                }
                else
                {
                    _charRepresentation[x] = (char)(x + 85);
                }
            }

            for (int x = 0; x < radixSize; x++)
            {
                _radixArray[x] = new BigInteger(new sbyte[1] { (sbyte)x });
            }

            
            isLocked = false;
        }

        public BigInteger(string sval)
          : this(sval, 10)
        {
        }

        public BigInteger(string sval, int rdx)
          : this((int)sval[0] != 45 ? 1 : -1, BigInteger._parseIntegerBase(sval, rdx))
        {
        }

        public BigInteger(sbyte[] bval)
          : this(((int)bval[0] & 128) == 0 ? 1 : -1, ((int)bval[0] & 128) == 0 ? bval : BigInteger._twosComplement(bval))
        {
        }

        public BigInteger(int sign, sbyte[] magnitude)
        {
            this._bits = (sbyte[])null;
            this._count = -1;
            this._length = -1;
            this._lowestSetBit = -1;
            this._leadingZeroCount = -1;
            this._absOfthis = (BigInteger)null;
            this._negate = (BigInteger)null;
            this._not = (BigInteger)null;
            this._toString = (string)null;
            this._doubleOfthis = null;
            this._floatOfthis = null;
            this._longOfthis = null;
            int length1 = magnitude.Length;
            if (length1 == 0 && sign != 0)
                throw new FormatException("Invalid magnitude and sign.");
            if (sign != 1 && sign != 0 && sign != -1)
                throw new FormatException("Invalid sign.");
            int sourceIndex = 0;
            while (sourceIndex < length1 && magnitude[sourceIndex] == 0)
                ++sourceIndex;
            int length2 = length1 - sourceIndex;
            if (length2 < 0)
                throw new IndexOutOfRangeException();
            this._bits = new sbyte[length2];
            if (sign == 0 && this._bits.Length != 0)
                throw new FormatException("Invalid sign and bit length.");
            this._sign = this._bits.Length != 0 ? sign : 0;
            
            if (this._bits.Length != 0)
            {
                Array.Copy(magnitude, sourceIndex, this._bits, 0, this._bits.Length);
            }
        }

        public BigInteger(int numBits, Random rnd)
        {
            this._bits = (sbyte[])null;
            this._count = -1;
            this._length = -1;
            this._lowestSetBit = -1;
            this._leadingZeroCount = -1;
            this._absOfthis = null;
            this._negate = null;
            this._not = null;
            this._toString = (string)null;
            this._doubleOfthis = null;
            this._floatOfthis = null;
            this._longOfthis = null;
            if (numBits < 0)
                throw new ArgumentException($"{nameof(numBits)} < 0.");
            this._sign = 1;
            int length = (numBits + 7) / 8;
            if (length < 0)
                throw new IndexOutOfRangeException();

            var bytes = new byte[length];
            rnd.NextBytes(bytes);
            _bits = (from u in bytes select (sbyte)((int)u - 128)).ToArray();


            sbyte num = (sbyte)((1 << numBits % 8) - 1);
            if ((int)num == 0)
                return;
            this._bits[0] = (sbyte)(_bits[0] & num);
        }

        public BigInteger(int bitLength, int certainty, Random rnd)
        {
            this._bits = null;
            this._count = -1;
            this._length = -1;
            this._lowestSetBit = -1;
            this._leadingZeroCount = -1;
            this._absOfthis = null;
            this._negate = null;
            this._not = null;
            this._toString = null;
            this._doubleOfthis = null;
            this._floatOfthis = null;
            this._longOfthis = null;
            if (bitLength < 2)
                throw new ArithmeticException();
            this._sign = 1;
            int length = (bitLength + 7) / 8;
            if (length < 0)
                throw new IndexOutOfRangeException();
            this._bits = new sbyte[length];
            do
            {
                var bytes = new byte[length];
                rnd.NextBytes(bytes);
                _bits = (from u in bytes select (sbyte)((int)u - 128)).ToArray();

                sbyte num = (sbyte)((1 << bitLength % 8) - 1);
                if ((int)num != 0)
                    this._bits[0] = (sbyte)((int)this._bits[0] & (int)num);
            }
            while (!this.IsProbablyPrime(certainty));
        }

        private BigInteger(int sign, sbyte[] bits, bool needClone)
        {
            this._bits = null;
            this._count = -1;
            this._length = -1;
            this._lowestSetBit = -1;
            this._leadingZeroCount = -1;
            this._absOfthis = null;
            this._negate = null;
            this._not = null;
            this._toString = (string)null;
            this._doubleOfthis = null;
            this._floatOfthis = null;
            this._longOfthis = null;
            int sourceIndex = 0;
            while (sourceIndex < bits.Length && (int)bits[sourceIndex] == 0)
                ++sourceIndex;
            this._sign = sourceIndex >= bits.Length ? 0 : sign;
            if (sourceIndex != 0)
            {
                int length = bits.Length - sourceIndex;
                if (length < 0)
                    throw new IndexOutOfRangeException();
                this._bits = new sbyte[length];
                Array.Copy((Array)bits, sourceIndex, (Array)this._bits, 0, this._bits.Length);
            }
            else
            {
                if (!needClone)
                {
                    _bits = bits;
                }
                else
                {
                    
                    sbyte[] copy = new sbyte[bits.Length];
                    Array.Copy(bits, copy, copy.Length);
                    _bits = copy;
                }
            }

        }

        #endregion

        #region "Math"

        public BigInteger Abs()
        {
            if (_absOfthis != null)
                return _absOfthis;
            if (_sign != 0)
                return _absOfthis = new BigInteger(1, _bits, true);
            return _absOfthis = this;
        }
        public BigInteger Add(BigInteger val)
        {
            if (this._sign == 0)
                return val;
            if (val._sign == 0)
                return this;
            int length = Math.Max(_bits.Length, val._bits.Length) + 1;
            if (length < 0)
                throw new IndexOutOfRangeException();
            sbyte[] numArray = new sbyte[length];
            return new BigInteger(BigInteger._subtractBitsArray(numArray, _bits, _sign, val._bits, val._sign), numArray, false);
        }
        public BigInteger Subtract(BigInteger val)
        {
            if (this._sign == 0)
                return new BigInteger(val._sign * -1, val._bits, true);
            if (val._sign == 0)
                return this;
            int length = Math.Max(this._bits.Length, val._bits.Length) + 1;
            if (length < 0)
                throw new IndexOutOfRangeException();
            sbyte[] numArray = new sbyte[length];
            return new BigInteger(BigInteger._subtractBitsArray(numArray, this._bits, this._sign, val._bits, -1 * val._sign), numArray, false);
        }
        public BigInteger Divide(BigInteger val)
        {
            if (val._bits.Length == 0)
                throw new ArithmeticException("Divisor bit length is 0.");
            BigInteger IntegerBase;
            if (this._bits.Length == 0)
            {
                IntegerBase = BigInteger._radixArray[0];
            }
            else
            {
                int num = 2;
                int length1 = 1;
                int length2 = num;
                if (length1 < 0 || length2 < 0)
                    throw new IndexOutOfRangeException();
                sbyte[][] result = ArrayHelper.InitMultiDArray<sbyte>(length2, length1);
                IntegerBase = new BigInteger(BigInteger._divideAndRemainder(result, this, val), result[0], false);
            }
            return IntegerBase;
        }
        public BigInteger[] DivideAndRemainder(BigInteger divisor)
        {
            if (divisor._bits.Length == 0)
                throw new ArithmeticException("Divisor bit length is 0.");
            int length1 = 2;
            if (length1 < 0)
                throw new IndexOutOfRangeException();
            BigInteger[] IntegerBaseArray = new BigInteger[length1];
            if (this._bits.Length == 0)
            {
                IntegerBaseArray[0] = BigInteger._radixArray[0];
                IntegerBaseArray[1] = BigInteger._radixArray[0];
            }
            else
            {
                int num = 2;
                int length2 = 1;
                int length3 = num;
                if (length2 < 0 || length3 < 0)
                    throw new IndexOutOfRangeException();
                sbyte[][] result = ArrayHelper.InitMultiDArray<sbyte>(length3, length2);
                int sign = BigInteger._divideAndRemainder(result, this, divisor);
                IntegerBaseArray[0] = new BigInteger(sign, result[0], false);
                IntegerBaseArray[1] = new BigInteger(this._sign, result[1], false);
            }
            return IntegerBaseArray;
        }
        public BigInteger Gcd(BigInteger val)
        {
            BigInteger IntegerBase1 = this;
            BigInteger m = val;
            Monitor.Enter(this);
            try
            {
                Monitor.Enter(val);
                try
                {
                    int sign1 = this._sign;
                    int sign2 = val._sign;
                    if (this._sign == -1)
                        this._sign = 1;
                    if (val._sign == -1)
                        val._sign = 1;
                    if (BigInteger.CompareTo(this._sign, this._bits, val._sign, val._bits) == -1)
                    {
                        IntegerBase1 = val;
                        m = this;
                    }
                    BigInteger IntegerBase2;
                    for (; m._bits.Length != 0; m = IntegerBase2)
                    {
                        IntegerBase2 = IntegerBase1.Mod(m);
                        IntegerBase1 = m;
                    }
                    this._sign = sign1;
                    val._sign = sign2;
                }
                finally
                {
                    Monitor.Exit(val);
                }
            }
            finally
            {
                Monitor.Exit(this);
            }
            if (IntegerBase1._sign == -1)
                return new BigInteger(1, IntegerBase1._bits, true);
            return IntegerBase1;
        }
        public BigInteger Max(BigInteger val)
        {
            switch (BigInteger.CompareTo(this._sign, this._bits, val._sign, val._bits))
            {
                case 0:
                case -1:
                    return val;
                default:
                    return this;
            }
        }
        public BigInteger Min(BigInteger val)
        {
            switch (BigInteger.CompareTo(this._sign, this._bits, val._sign, val._bits))
            {
                case 0:
                case 1:
                    return val;
                default:
                    return this;
            }
        }
        public BigInteger Mod(BigInteger m)
        {
            if (m._bits.Length == 0 || m._sign == -1)
                throw new ArithmeticException("Modulus base is either empty or negative.");
            BigInteger IntegerBase;
            if (this._bits.Length == 0)
            {
                IntegerBase = BigInteger._radixArray[0];
            }
            else
            {
                int num = 2;
                int length1 = 1;
                int length2 = num;
                if (length1 < 0 || length2 < 0)
                    throw new IndexOutOfRangeException();
                sbyte[][] result = ArrayHelper.InitMultiDArray<sbyte>(length2, length1);
                BigInteger._divideAndRemainder(result, this, m);
                sbyte[] numArray = result[1];
                if (this._sign == -1)
                {
                    int length3 = Math.Max(m._bits.Length, numArray.Length) + 1;
                    if (length3 < 0)
                        throw new IndexOutOfRangeException();
                    numArray = new sbyte[length3];
                    BigInteger.AddBitsArray(numArray, m._bits, 1, BigInteger._twosComplement(result[1]), -1);
                    int length4 = numArray.Length - m._bits.Length;
                    Array.Clear((Array)numArray, 0, length4);
                }
                IntegerBase = new BigInteger(1, numArray, false);
            }
            return IntegerBase;
        }
        public BigInteger ModInverse(BigInteger m)
        {
            if (m._sign == 0 || m._sign == -1)
                throw new ArithmeticException("Inverse modulus sign is 0 or -1.");
            BigInteger val1 = m;
            BigInteger val2 = this.Mod(m);
            BigInteger IntegerBase1 = val1.Gcd(val2);
            int length1 = 1;
            if (length1 < 0)
                throw new IndexOutOfRangeException();
            sbyte[] bval1 = new sbyte[length1];
            bval1[0] = (sbyte)1;
            BigInteger IntegerBase2 = new BigInteger(bval1);
            if (!IntegerBase1.Equals((object)IntegerBase2))
                throw new ArithmeticException("!IntegerBase1.Equals((object)IntegerBase2)");
            int length2 = 1;
            if (length2 < 0)
                throw new IndexOutOfRangeException();
            sbyte[] bval2 = new sbyte[length2];
            bval2[0] = (sbyte)1;
            BigInteger IntegerBase3 = new BigInteger(bval2);
            int length3 = 1;
            if (length3 < 0)
                throw new IndexOutOfRangeException();
            sbyte[] bval3 = new sbyte[length3];
            bval3[0] = (sbyte)0;
            BigInteger IntegerBase4 = new BigInteger(bval3);
            BigInteger IntegerBase5 = val1;
            int length4 = 1;
            if (length4 < 0)
                throw new IndexOutOfRangeException();
            sbyte[] bval4 = new sbyte[length4];
            bval4[0] = (sbyte)0;
            BigInteger IntegerBase6 = new BigInteger(bval4);
            int length5 = 1;
            if (length5 < 0)
                throw new IndexOutOfRangeException();
            sbyte[] bval5 = new sbyte[length5];
            bval5[0] = (sbyte)1;
            BigInteger IntegerBase7 = new BigInteger(bval5);
            BigInteger IntegerBase8;
            for (BigInteger val3 = val2; val3._bits.Length != 0; val3 = IntegerBase8)
            {
                BigInteger val4 = IntegerBase5.Divide(val3);
                BigInteger IntegerBase9 = IntegerBase3.Subtract(IntegerBase6.Multiply(val4));
                BigInteger IntegerBase10 = IntegerBase4.Subtract(IntegerBase7.Multiply(val4));
                IntegerBase8 = IntegerBase5.Subtract(val3.Multiply(val4));
                IntegerBase3 = IntegerBase6;
                IntegerBase4 = IntegerBase7;
                IntegerBase5 = val3;
                IntegerBase6 = IntegerBase9;
                IntegerBase7 = IntegerBase10;
            }
            while (IntegerBase4._sign == -1)
                IntegerBase4 = IntegerBase4.Add(val1);
            return IntegerBase4;
        }
        public BigInteger ModPow(BigInteger exp, BigInteger m)
        {
            if (m._sign != 1)
                throw new ArithmeticException("Modulus sign isn't -1.");
            if (exp._sign == 0)
                return BigInteger._radixArray[1];
            if (exp.Equals((object)BigInteger._radixArray[1]))
                return this.Mod(m);
            int sign = exp._sign;
            if (sign == -1)
                exp = new BigInteger(1, exp._bits, true);
            BigInteger val = this;
            int n = exp.GetBitLength() - 2;
            while (n >= 0)
            {
                val = val.Multiply(val).Mod(m);
                if (exp.TestBit(n))
                    val = val.Multiply(this).Mod(m);
                n += -1;
            }
            if (sign == -1)
                return val.ModInverse(m);
            return val;
        }
        public BigInteger Multiply(BigInteger val)
        {
            int num1 = val.GetBitLength();
            int num2 = this.GetBitLength();
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
            return new BigInteger(BigInteger._multiply(numArray, op1, sign1, op2, sign2), numArray, false);
        }
        public BigInteger Negate()
        {
            if (_bits.Length == 0) return this;
            if (this._negate == null)
            {
                this._negate = new BigInteger(this._sign * -1, this._bits, true);
            }
            return this._negate;
        }
        public BigInteger Pow(int exp)
        {
            if (exp < 0)
                throw new ArithmeticException("Exp is negative.");
            if (exp == 0)
            {
                int length = 1;
                if (length < 0)
                    throw new IndexOutOfRangeException();
                sbyte[] bval = new sbyte[length];
                bval[0] = (sbyte)1;
                return new BigInteger(bval);
            }
            if (this._bits.Length == 0)
            {
                int length = 1;
                if (length < 0)
                    throw new IndexOutOfRangeException();
                sbyte[] bval = new sbyte[length];
                bval[0] = (sbyte)0;
                return new BigInteger(bval);
            }
            sbyte[] bits = BigInteger._pow(this._bits, this._sign, exp);
            int sign = 1;
            if (this._sign == -1 && (exp & 1) != 0)
                sign = -1;
            return new BigInteger(sign, bits, false);
        }
        public BigInteger Remainder(BigInteger val)
        {
            if (val._bits.Length == 0)
                throw new ArithmeticException($"{nameof(val)}._bits is empty.");
            BigInteger IntegerBase;
            if (this._bits.Length == 0)
            {
                IntegerBase = BigInteger._radixArray[0];
            }
            else
            {
                int num = 2;
                int length1 = 1;
                int length2 = num;
                if (length1 < 0 || length2 < 0)
                    throw new IndexOutOfRangeException();
                sbyte[][] result = ArrayHelper.InitMultiDArray<sbyte>(length2, length1);
                BigInteger._divideAndRemainder(result, this, val);
                IntegerBase = new BigInteger(this._sign, result[1], false);
            }
            return IntegerBase;
        }
        public bool IsProbablyPrime(int certainty)
        {
            if (certainty < 2)
                return true;
            certainty = 3;
            BigInteger val1 = BigInteger.FromLong(0L);
            BigInteger val2 = BigInteger.FromLong(1L);
            BigInteger val3 = BigInteger.FromLong(2L);
            BigInteger m = this.Abs();
            if (m.CompareTo(val3) < 0)
                return false;
            BigInteger IntegerBase1 = m.Subtract(val2);
            int exp = 0;
            while (IntegerBase1.Mod(val3.Pow(exp)).CompareTo(val1) == 0)
                ++exp;
            if (exp > 0)
                exp += -1;
            BigInteger IntegerBase2 = IntegerBase1.Divide(val3.Pow(exp));
            Random random = new Random();
            bool flag = true;
            int num = certainty;
            while (num > 0)
            {
                BigInteger IntegerBase3 = BigInteger.FromLong((long)Math.Abs(random.Next())).Mod(IntegerBase1).Add(val2);
                BigInteger val4 = val2;
                for (BigInteger IntegerBase4 = val1; IntegerBase4.CompareTo(BigInteger.FromLong((long)exp)) <= 0; IntegerBase4 = IntegerBase4.Add(val2))
                {
                    BigInteger IntegerBase5 = IntegerBase3.Pow(Math.Abs(IntegerBase2.Multiply(val4).AsInt())).Mod(m);
                    if (IntegerBase5.CompareTo(val2) == 0 || IntegerBase5.CompareTo(IntegerBase1) == 0)
                    {
                        flag = false;
                        break;
                    }
                    val4 = val4.Multiply(val3);
                }
                if (flag)
                {
                    flag = false;
                    break;
                }
                flag = true;
                num += -1;
            }
            return flag;
        }
        public int SigNum()
        {
            return this._sign;
        }

        #endregion

        #region "Bitwise Operations"

        public BigInteger Not()
        {
            if (this._not != null)
                return this._not;
            if (this._sign == 0)
            {
                int sign = -1;
                int length = 1;
                if (length < 0)
                    throw new IndexOutOfRangeException();
                sbyte[] bits = new sbyte[length];
                bits[0] = (sbyte)1;
                int num = 0;
                return this._not = new BigInteger(sign, bits, num != 0);
            }
            int length1 = this._bits.Length;
            if (length1 < 0)
                throw new IndexOutOfRangeException();
            sbyte[] numArray = new sbyte[length1];
            return this._not = new BigInteger(BigInteger._notBits(numArray, this._bits, this._sign), numArray, false);
        }
        public BigInteger Or(BigInteger val)
        {
            if (this._sign == 0)
                return val;
            if (val._sign == 0)
                return this;
            int length = Math.Max(this._bits.Length, val._bits.Length);
            if (length < 0)
                throw new IndexOutOfRangeException();
            sbyte[] numArray = new sbyte[length];
            return new BigInteger(BigInteger._orBits(numArray, this._bits, this._sign, val._bits, val._sign), numArray, false);
        }
        public BigInteger Xor(BigInteger val)
        {
            int length = Math.Max(this._bits.Length, val._bits.Length);
            if (length < 0)
                throw new IndexOutOfRangeException();
            sbyte[] numArray = new sbyte[length];
            return new BigInteger(BigInteger._xorBits(numArray, this._bits, this._sign, val._bits, val._sign), numArray, false);
        }
        public BigInteger And(BigInteger val)
        {
            if (_sign == 0 || val._sign == 0)
                return BigInteger._radixArray[0];
            int length = Math.Max(_bits.Length, val._bits.Length);
            if (length < 0)
                throw new IndexOutOfRangeException();
            sbyte[] numArray = new sbyte[length];
            return new BigInteger(BigInteger.AndBits(numArray, _bits, _sign, val._bits, val._sign), numArray, false);
        }
        public BigInteger AndNot(BigInteger val)
        {
            if (this._sign == 0)
                return BigInteger._radixArray[0];
            int length1 = val._bits.Length;
            if (length1 < 0)
                throw new IndexOutOfRangeException();
            sbyte[] numArray1 = new sbyte[length1];
            int op2sign = BigInteger._notBits(numArray1, val._bits, val._sign);
            if (op2sign == 0)
                return BigInteger._radixArray[0];
            int length2 = Math.Max(this._bits.Length, numArray1.Length);
            if (length2 < 0)
                throw new IndexOutOfRangeException();
            sbyte[] numArray2 = new sbyte[length2];
            return new BigInteger(BigInteger.AndBits(numArray2, this._bits, this._sign, numArray1, op2sign), numArray2, false);
        }
        public int BitCount()
        {
            return this._count != -1 ? this._count : (this._count = BigInteger.CountBits(this._bits, this._sign));
        }
        public int GetBitLength()
        {
            return this._length != -1 ? this._length : (this._length = BigInteger.GetBitLength(this._bits, this._sign));
        }
        public BigInteger ClearBit(int n)
        {
            return BigInteger.ClearOrSetOrFlipBit(this._bits, this._sign, n, 1);
        }
        public BigInteger FlipBit(int pos)
        {
            return BigInteger.ClearOrSetOrFlipBit(this._bits, this._sign, pos, 3);
        }
        public BigInteger SetBit(int n)
        {
            return BigInteger.ClearOrSetOrFlipBit(this._bits, this._sign, n, 2);
        }
        public BigInteger ShiftLeft(int n)
        {
            if (n < 0)
                return this.ShiftRight(-n);
            if (n == 0)
                return this;
            if (this._bits.Length == 0)
            {
                int sign = 0;
                int length = 0;
                if (length < 0)
                    throw new IndexOutOfRangeException();
                sbyte[] bits = new sbyte[length];
                int num = 0;
                return new BigInteger(sign, bits, num != 0);
            }
            int length1 = (this.GetBitLength() + n + 7) / 8;
            if (length1 < 0)
                throw new IndexOutOfRangeException();
            sbyte[] numArray = new sbyte[length1];
            return new BigInteger(BigInteger._shiftLeftOrRight(numArray, this._bits, this._sign, n, 1), numArray, false);
        }
        public BigInteger ShiftRight(int n)
        {
            if (n < 0)
                return this.ShiftLeft(-n);
            if (n == 0)
                return this;
            if (this._bits.Length == 0)
            {
                int sign = 0;
                int length = 0;
                if (length < 0)
                    throw new IndexOutOfRangeException();
                sbyte[] bits = new sbyte[length];
                int num = 0;
                return new BigInteger(sign, bits, num != 0);
            }
            int length1 = this._bits.Length;
            if (length1 < 0)
                throw new IndexOutOfRangeException();
            sbyte[] numArray = new sbyte[length1];
            return new BigInteger(BigInteger._shiftLeftOrRight(numArray, this._bits, this._sign, n, 2), numArray, false);
        }
        public int GetLowestSetBit()
        {
            if (this._lowestSetBit != -1)
                return this._lowestSetBit;
            int index1 = this._bits.Length - 1;
            while (index1 >= 0)
            {
                int num = 1;
                for (int index2 = 0; index2 < 8; ++index2)
                {
                    if (((int)this._bits[index1] & num) != 0)
                        return this._lowestSetBit = (this._bits.Length - 1 - index1) * 8 + index2;
                    num <<= 1;
                }
                index1 += -1;
            }
            return this._lowestSetBit = -1;
        }
        public sbyte[] ToByteArray()
        {
            int length = (this.GetBitLength() + 8) / 8;
            if (length < 0)
                throw new IndexOutOfRangeException();
            sbyte[] res = new sbyte[length];
            if (this._sign == -1)
            {
                BigInteger._twosComplement(res, this._bits);
            }
            else
            {
                int index1 = this._bits.Length - 1;
                int index2 = res.Length - 1;
                while (index1 >= 0)
                {
                    res[index2] = this._bits[index1];
                    index1 += -1;
                    index2 += -1;
                }
            }
            return res;
        }
        public bool TestBit(int n)
        {
            if (n < 0)
                throw new ArithmeticException($"{nameof(n)} is negative.");
            int num1 = this.GetBitLength();
            if (num1 == 0)
                return this._sign == -1;
            if (n >= num1)
                return this._sign == -1;
            sbyte[] numArray = this._bits;
            if (this._sign == -1)
                numArray = BigInteger._twosComplement(this._bits);
            int num2 = 1 << n % 8;
            int index = numArray.Length - (n + 8) / 8;
            return ((int)numArray[index] & num2) != 0;
        }

        #endregion

        #region "Conversions"

        public override double AsDouble()
        {
            if (_doubleOfthis == null)
                _doubleOfthis = double.Parse(this.ToString(10));
            return _doubleOfthis.Value;
        }
        public override float AsFloat()
        {
            if (_floatOfthis == null)
            {
                _floatOfthis = float.Parse(this.ToString(10));
            }
            return _floatOfthis.Value;
        }
        public override int AsInt()
        {
            return (int)this.AsLong();
        }
        public override long AsLong()
        {
            if (_longOfthis != null)
                return _longOfthis.Value;
            long num1 = 0;
            int num2 = 0;
            int index = this._bits.Length - 1;
            while (num2 < 64 && index >= 0)
            {
                long num3 = ((long)_bits[index] & (long)byte.MaxValue) << num2;
                num1 |= num3;
                num2 += 8;
                index += -1;
            }
            _longOfthis = num1 * this._sign;
            return _longOfthis.Value;
        }
        public static BigInteger FromLong(long val)
        {
            if (val >= 0L && val < (long)BigInteger._radixArray.Length)
                return BigInteger._radixArray[(int)val];
            int sign = 1;
            if ((val & long.MinValue) == long.MinValue)
            {
                sign = -1;
                val = long.MinValue - (val & long.MaxValue);
            }
            int length = 8;
            if (length < 0)
                throw new IndexOutOfRangeException();
            sbyte[] bits = new sbyte[length];
            int num1 = 56;
            long num2 = -72057594037927936;
            int index = 0;
            int num3 = 0;
            while (num1 >= 0)
            {
                bits[index] = (sbyte)((val & num2) >> num1 & (long)byte.MaxValue);
                ++num3;
                num1 -= 8;
                num2 = (long)((ulong)num2 >> 8);
                ++index;
            }
            return new BigInteger(sign, bits, false);
        }

        #endregion

        #region "Object Overrides"

        public virtual int CompareTo(object obj)
        {
            return this.CompareTo((BigInteger)obj);
        }
        public int CompareTo(BigInteger val)
        {
            return BigInteger.CompareTo(this._sign, this._bits, val._sign, val._bits);
        }
        public override bool Equals(object val)
        {
            return this == val || val is BigInteger && BigInteger.CompareTo(this._sign, this._bits, ((BigInteger)val)._sign, ((BigInteger)val)._bits) == 0;
        }
        public override int GetHashCode()
        {
            return this.AsInt();
        }
        public override string ToString()
        {
            return this._toString != null ? this._toString : (this._toString = this.ToString(10));
        }
        public string ToString(int rdx)
        {
            StringBuilder strBuilder = new StringBuilder();
            bool flag1 = false;
            if (this._sign == -1)
                flag1 = true;
            BigInteger radix = BigInteger._radixArray[rdx];
            BigInteger integerBase = this;
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
            if (flag1)
                strBuilder.Append("-");
            return new string(strBuilder.ToString().Reverse().ToArray());
        }

        #endregion
        
        #region "Internal Math"

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static void AddBitsArray(sbyte[] res, sbyte[] op)
        {
            BigInteger.AddBitsArray(res, res.Length - 1, op, op.Length - 1, 1);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private static void AddBitsArray(sbyte[] res, int resIndex, sbyte[] op, int opIndex, int opsign)
        {
            sbyte num1 = 0;
            if (opIndex < 0)
                return;
            while (opIndex >= 0)
            {
                int num2 = ((int)res[resIndex] & (int)byte.MaxValue) + ((int)op[opIndex] & (int)byte.MaxValue) + ((int)num1 & (int)byte.MaxValue);
                res[resIndex] = (sbyte)(num2 & (int)byte.MaxValue);
                num1 = (sbyte)((num2 & 256) >> 8);
                opIndex += -1;
                resIndex += -1;
            }
            int num3 = 0;
            if (opsign == -1)
                num3 = (int)byte.MaxValue;
            while ((int)num1 > 0 && resIndex >= 0)
            {
                int num2 = ((int)res[resIndex] & (int)byte.MaxValue) + num3 + ((int)num1 & (int)byte.MaxValue);
                res[resIndex] = (sbyte)(num2 & (int)byte.MaxValue);
                num1 = (sbyte)((num2 & 256) >> 8);
                resIndex += -1;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private static void AddBitsArray(sbyte[] res, sbyte[] op1, int op1sign, sbyte[] op2, int op2sign)
        {
            int index1 = op1.Length - 1;
            int index2 = op2.Length - 1;
            int index3 = res.Length - 1;
            sbyte num1 = 0;
            while (index1 >= 0 && index2 >= 0)
            {
                int num2 = ((int)op1[index1] & (int)byte.MaxValue) + ((int)op2[index2] & (int)byte.MaxValue) + ((int)num1 & (int)byte.MaxValue);
                res[index3] = (sbyte)(num2 & (int)byte.MaxValue);
                num1 = (sbyte)((num2 & 256) >> 8);
                index1 += -1;
                index2 += -1;
                index3 += -1;
            }
            if (index1 >= 0 && index2 < 0)
            {
                int num2 = 0;
                if (op2sign == -1)
                    num2 = (int)byte.MaxValue;
                while (index1 >= 0)
                {
                    int num3 = ((int)op1[index1] & (int)byte.MaxValue) + num2 + (int)num1;
                    res[index3] = (sbyte)(num3 & (int)byte.MaxValue);
                    num1 = (sbyte)((num3 & 256) >> 8);
                    index1 += -1;
                    index3 += -1;
                }
            }
            else if (index1 < 0 && index2 >= 0)
            {
                int num2 = 0;
                if (op1sign == -1)
                    num2 = (int)byte.MaxValue;
                while (index2 >= 0)
                {
                    int num3 = ((int)op2[index2] & (int)byte.MaxValue) + num2 + (int)num1;
                    res[index3] = (sbyte)(num3 & (int)byte.MaxValue);
                    num1 = (sbyte)((num3 & 256) >> 8);
                    index2 += -1;
                    index3 += -1;
                }
            }
            res[index3] = num1;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private static int AndBits(sbyte[] res, sbyte[] op1, int op1sign, sbyte[] op2, int op2sign)
        {
            int index1 = op1.Length - 1;
            int index2 = op2.Length - 1;
            int index3 = res.Length - 1;
            if (op1sign == -1)
                op1 = BigInteger._twosComplement(op1);
            if (op2sign == -1)
                op2 = BigInteger._twosComplement(op2);
            while (index1 >= 0 && index2 >= 0)
            {
                res[index3] = (sbyte)((int)op1[index1] & (int)op2[index2]);
                index1 += -1;
                index2 += -1;
                index3 += -1;
            }
            if (index1 >= 0 && index2 < 0)
            {
                if (op2sign == -1)
                {
                    while (index1 >= 0)
                    {
                        res[index3] = op1[index1];
                        index1 += -1;
                        index3 += -1;
                    }
                }
            }
            else if (index1 < 0 && index2 >= 0 && op1sign == -1)
            {
                while (index2 >= 0)
                {
                    res[index3] = op2[index2];
                    index2 += -1;
                    index3 += -1;
                }
            }
            if (op1sign != -1 || op2sign != -1)
                return 1;
            BigInteger._twosComplement(res, res);
            return op1sign;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static int GetBitLength(sbyte[] bits, int sign)
        {
            if (sign == 0)
                return 0;
            int index1 = 0;
            int num1 = -1;
            if (sign == -1)
            {
                for (bits = BigInteger._twosComplement(bits); index1 < bits.Length && num1 == -1; ++index1)
                {
                    int num2 = 128;
                    for (int index2 = 0; index2 < 8; ++index2)
                    {
                        if (((int)bits[index1] & num2) == 0)
                        {
                            num1 = index2;
                            break;
                        }
                        num2 = (int)((uint)num2 >> 1);
                    }
                }
            }
            else
            {
                for (; index1 < bits.Length && num1 == -1; ++index1)
                {
                    int num2 = 128;
                    for (int index2 = 0; index2 < 8; ++index2)
                    {
                        if (((int)bits[index1] & num2) != 0)
                        {
                            num1 = index2;
                            break;
                        }
                        num2 = (int)((uint)num2 >> 1);
                    }
                }
            }
            if (num1 == -1)
                num1 = 8;
            return (bits.Length - index1) * 8 + (8 - num1);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private static BigInteger ClearOrSetOrFlipBit(sbyte[] bits, int sign, int pos, int operation)
        {
            if (pos < 0)
                throw new ArithmeticException("Position is negative.");
            int length = Math.Max(bits.Length, (pos + 8) / 8);
            if (length < 0)
                throw new IndexOutOfRangeException();
            sbyte[] numArray = new sbyte[length];
            int index1 = numArray.Length - 1;
            int index2 = bits.Length - 1;
            while (index1 >= 0 && index2 >= 0)
            {
                numArray[index1] = bits[index2];
                index1 += -1;
                index2 += -1;
            }
            if (sign == -1)
                BigInteger._twosComplement(numArray, numArray);
            int num = 1 << pos % 8;
            int index3 = numArray.Length - (pos + 8) / 8;
            switch (operation)
            {
                case 1:
                    numArray[index3] = (sbyte)((int)numArray[index3] & (num ^ -1));
                    break;
                case 2:
                    numArray[index3] = (sbyte)((int)numArray[index3] | num);
                    break;
                case 3:
                    numArray[index3] = ((int)numArray[index3] & num) != 0 ? (sbyte)((int)numArray[index3] & (num ^ -1)) : (sbyte)((int)numArray[index3] | num);
                    break;
            }
            if (sign == -1)
                BigInteger._twosComplement(numArray, numArray);
            if (sign == 0)
                sign = 1;
            return new BigInteger(sign, numArray, false);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static int CompareTo(int signx, sbyte[] xbits, int signy, sbyte[] ybits)
        {
            if (signx == signy)
            {
                if (signx == 0)
                    return 0;
                int length1 = xbits.Length;
                int length2 = ybits.Length;
                int num1 = 0;
                if ((int)xbits[0] == 0)
                {
                    ++num1;
                    for (int index = 1; index < xbits.Length && (int)xbits[index] == 0; ++index)
                        ++num1;
                }
                int num2 = 0;
                if ((int)ybits[0] == 0)
                {
                    ++num2;
                    for (int index = 1; index < ybits.Length && (int)ybits[index] == 0; ++index)
                        ++num2;
                }
                if (length1 - num1 > length2 - num2)
                    return 1 * signx;
                if (length1 - num1 < length2 - num2)
                    return -1 * signy;
                int index1 = num1;
                for (int index2 = num2; index1 < length1 && index2 < length2; ++index2)
                {
                    if ((int)xbits[index1] != (int)ybits[index2])
                    {
                        if (((int)xbits[index1] & (int)byte.MaxValue) > ((int)ybits[index2] & (int)byte.MaxValue))
                            return 1 * signx;
                        return -1 * signy;
                    }
                    ++index1;
                }
                return 0;
            }
            if (signx == 0)
                return -signy;
            if (signy == 0)
                return signx;
            return signx == 1 ? 1 : -1;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private static int CountBits(sbyte[] bval, int sign)
        {
            int num1 = 0;
            if (sign == 1)
            {
                for (int index1 = 0; index1 < bval.Length; ++index1)
                {
                    int num2 = 128;
                    for (int index2 = 0; index2 < 8; ++index2)
                    {
                        if (((int)bval[index1] & num2) != 0)
                            ++num1;
                        num2 = (int)((uint)num2 >> 1);
                    }
                }
            }
            else if (sign == -1)
            {
                sbyte[] numArray = BigInteger._twosComplement(bval);
                for (int index1 = 0; index1 < bval.Length; ++index1)
                {
                    int num2 = 128;
                    for (int index2 = 0; index2 < 8; ++index2)
                    {
                        if (((int)numArray[index1] & num2) == 0)
                            ++num1;
                        num2 = (int)((uint)num2 >> 1);
                    }
                }
            }
            return num1;
        }


        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static int _divide(sbyte[] res, sbyte[] dividend, int dividendsign, sbyte[] divisor, int divisorsign)
        {
            sbyte[] op = BigInteger._twosComplement(divisor);
            BigInteger.AddBitsArray(res, res.Length - 1, dividend, dividend.Length - 1, 1);
            for (int index = 0; index < dividend.Length * 8; ++index)
            {
                BigInteger._shiftLeftOneBit(res);
                BigInteger.AddBitsArray(res, res.Length - 1 - dividend.Length, op, op.Length - 1, -1);
                if (((int)res[0] & 128) != 0)
                {
                    BigInteger.AddBitsArray(res, res.Length - 1 - dividend.Length, divisor, divisor.Length - 1, 1);
                    res[res.Length - 1] = (sbyte)((int)res[res.Length - 1] & 254);
                }
                else
                    res[res.Length - 1] = (sbyte)((int)res[res.Length - 1] | 1);
            }
            return divisorsign == dividendsign ? 1 : -1;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private static int _divideAndRemainder(sbyte[][] result, BigInteger dividend, BigInteger divisor)
        {
            int num1 = divisor.GetBitLength();
            int num2 = dividend.GetBitLength();
            int sign1 = dividend._sign;
            sbyte[] dividend1;
            if (num2 % 8 == 0)
            {
                int length = dividend._bits.Length + 1;
                if (length < 0)
                    throw new IndexOutOfRangeException();
                dividend1 = new sbyte[length];
                Array.Copy((Array)dividend._bits, 0, (Array)dividend1, 1, dividend._bits.Length);
            }
            else
                dividend1 = dividend._bits;
            int sign2 = divisor._sign;
            sbyte[] divisor1;
            if (num1 % 8 == 0)
            {
                int length = divisor._bits.Length + 1;
                if (length < 0)
                    throw new IndexOutOfRangeException();
                divisor1 = new sbyte[length];
                Array.Copy((Array)divisor._bits, 0, (Array)divisor1, 1, divisor._bits.Length);
            }
            else
                divisor1 = divisor._bits;
            int length1 = divisor1.Length + dividend1.Length;
            if (length1 < 0)
                throw new IndexOutOfRangeException();
            sbyte[] res = new sbyte[length1];
            int num3 = BigInteger._divide(res, dividend1, sign1, divisor1, sign2);
            int num4 = 0;
            sbyte[][] numArray1 = result;
            int index1 = 0;
            int length2 = dividend1.Length;
            if (length2 < 0)
                throw new IndexOutOfRangeException();
            sbyte[] numArray2 = new sbyte[length2];
            numArray1[index1] = numArray2;
            sbyte[][] numArray3 = result;
            int index2 = 1;
            int length3 = divisor1.Length;
            if (length3 < 0)
                throw new IndexOutOfRangeException();
            sbyte[] numArray4 = new sbyte[length3];
            numArray3[index2] = numArray4;
            Array.Copy((Array)res, divisor1.Length, (Array)result[0], 0, dividend1.Length);
            Array.Copy((Array)res, 0, (Array)result[1], 0, divisor1.Length);
            for (int index3 = 0; index3 < dividend1.Length; ++index3)
            {
                if ((int)result[0][index3] == 0)
                    ++num4;
            }
            return num4 >= result[0].Length ? 0 : num3;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static sbyte[] _multiply(sbyte[] op1, sbyte[] op2)
        {
            int num1 = BigInteger.GetBitLength(op1, 1);
            int num2 = BigInteger.GetBitLength(op2, 1);
            sbyte[] op1_1;
            if (num1 % 8 == 0 && (num1 + 8) / 8 != op1.Length)
            {
                int length = op1.Length + 1;
                if (length < 0)
                    throw new IndexOutOfRangeException();
                op1_1 = new sbyte[length];
                Array.Copy((Array)op1, 0, (Array)op1_1, 1, op1.Length);
            }
            else
                op1_1 = op1;
            sbyte[] op2_1;
            if (num2 % 8 == 0 && (num1 + 8) / 8 != op1.Length)
            {
                int length = op2.Length + 1;
                if (length < 0)
                    throw new IndexOutOfRangeException();
                op2_1 = new sbyte[length];
                Array.Copy((Array)op2, 0, (Array)op2_1, 1, op2.Length);
            }
            else
                op2_1 = op2;
            int length1 = op1_1.Length + op2_1.Length;
            if (length1 < 0)
                throw new IndexOutOfRangeException();
            sbyte[] res = new sbyte[length1];
            BigInteger._multiply(res, op1_1, 1, op2_1, 1);
            if ((int)res[0] == 0 && ((int)res[1] & 128) == 0)
            {
                int sourceIndex = 1;
                while (sourceIndex < res.Length && (int)res[sourceIndex] == 0)
                    ++sourceIndex;
                if (sourceIndex + 1 < res.Length && ((int)res[sourceIndex + 1] & 128) != 0)
                    sourceIndex += -1;
                int length2 = res.Length - sourceIndex;
                if (length2 < 0)
                    throw new IndexOutOfRangeException();
                sbyte[] numArray = new sbyte[length2];
                Array.Copy((Array)res, sourceIndex, (Array)numArray, 0, numArray.Length);
                res = numArray;
            }
            return res;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static int _multiply(sbyte[] res, sbyte[] op1, int op1sign, sbyte[] op2, int op2sign)
        {
            if (op1sign == 0 || op2sign == 0)
                return 0;
            int resIndex = res.Length - 1;
            int length = op1.Length + 1;
            if (length < 0)
                throw new IndexOutOfRangeException();
            sbyte[] numArray = new sbyte[length];
            int index = op2.Length - 1;
            while (index >= 0)
            {
                BigInteger._bytemultiply(numArray, op1, op2[index]);
                BigInteger.AddBitsArray(res, resIndex, numArray, numArray.Length - 1, 1);
                resIndex += -1;
                index += -1;
            }
            return op1sign != op2sign ? -1 : 1;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static void _bytemultiply(sbyte[] res, sbyte[] op1, sbyte op2)
        {
            int index1 = op1.Length - 1;
            int index2 = res.Length - 1;
            int num1 = 0;
            while (index1 >= 0)
            {
                int num2 = ((int)op2 & (int)byte.MaxValue) * ((int)op1[index1] & (int)byte.MaxValue) + (num1 & (int)byte.MaxValue);
                res[index2] = (sbyte)(num2 & (int)byte.MaxValue);
                num1 = (int)(sbyte)((num2 & 65280) >> 8);
                index1 += -1;
                index2 += -1;
            }
            res[0] = (sbyte)(num1 & (int)byte.MaxValue);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private static int _notBits(sbyte[] res, sbyte[] op, int opsign)
        {
            sbyte[] bval = op;
            if (opsign == -1)
            {
                BigInteger._twosComplement(res, bval);
                bval = res;
            }
            for (int index = 0; index < bval.Length; ++index)
                res[index] = (sbyte)((int)bval[index] ^ -1);
            if (((int)res[0] & 128) == 0)
                return 1;
            BigInteger._twosComplement(res, res);
            return -1;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private static int _orBits(sbyte[] res, sbyte[] op1, int op1sign, sbyte[] op2, int op2sign)
        {
            int index1 = op1.Length - 1;
            int index2 = op2.Length - 1;
            int index3 = res.Length - 1;
            if (op1sign == -1)
                op1 = BigInteger._twosComplement(op1);
            if (op2sign == -1)
                op2 = BigInteger._twosComplement(op2);
            while (index1 >= 0 && index2 >= 0)
            {
                res[index3] = (sbyte)((int)op1[index1] | (int)op2[index2]);
                index1 += -1;
                index2 += -1;
                index3 += -1;
            }
            if (index1 >= 0 && index2 < 0)
            {
                while (index1 >= 0)
                {
                    res[index3] = op2sign != -1 ? op1[index1] : (sbyte)-1;
                    index1 += -1;
                    index3 += -1;
                }
            }
            else if (index1 < 0 && index2 >= 0)
            {
                while (index2 >= 0)
                {
                    res[index3] = op1sign != -1 ? op2[index2] : (sbyte)-1;
                    index2 += -1;
                    index3 += -1;
                }
            }
            if (op1sign != -1 && op2sign != -1)
                return 1;
            BigInteger._twosComplement(res, res);
            return -1;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private static sbyte[] _parseIntegerBase(string str, int radix)
        {
            if (str == null || str == string.Empty || (radix < 2 || radix > 36) || str.LastIndexOf('.') != -1)
                throw new FormatException(str);
            int num1 = 0;
            int num2 = str.Length;
            if (str[0] == '-')
                ++num1;
            int num3 = radix != 2 ? (radix <= 2 || radix > 4 ? (radix <= 4 || radix > 8 ? (radix <= 8 || radix > 16 ? (radix <= 16 || radix > 32 ? ((num2 - num1) * 6 + 7) / 8 : ((num2 - num1) * 5 + 7) / 8) : ((num2 - num1) * 4 + 7) / 8) : ((num2 - num1) * 3 + 7) / 8) : ((num2 - num1) * 2 + 7) / 8) : (num2 - num1 + 7) / 8;
            int length1 = num3 + 1;
            if (length1 < 0)
                throw new IndexOutOfRangeException();
            sbyte[] numArray1 = new sbyte[length1];
            int length2 = num3 + 1;
            if (length2 < 0)
                throw new IndexOutOfRangeException();
            sbyte[] op = new sbyte[length2];
            BigInteger radix1 = BigInteger._radixArray[radix];
            for (int index = num1; index < num2; ++index)
            {
                long num4 = Convert.ToInt64(str[index].ToString(), radix);
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
            return numArray1;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static sbyte[] _pow(sbyte[] bits, int bitssign, int exp)
        {
            int length1 = bits.Length;
            if (length1 < 0)
                throw new IndexOutOfRangeException();
            sbyte[] numArray = new sbyte[length1];
            Array.Copy((Array)bits, 0, (Array)numArray, 0, bits.Length);
            sbyte[] op2;
            if (((exp & 1) == 0 ? 0 : 1) == 1)
            {
                op2 = numArray;
            }
            else
            {
                int length2 = 1;
                if (length2 < 0)
                    throw new IndexOutOfRangeException();
                op2 = new sbyte[length2];
                op2[0] = (sbyte)1;
            }
            exp = (int)((uint)exp >> 1);
            for (int index = 1; index < 31 && exp > 0; ++index)
            {
                int num = (exp & 1) == 0 ? 0 : 1;
                numArray = BigInteger._square(numArray);
                if (num == 1)
                    op2 = BigInteger._multiply(numArray, op2);
                exp = (int)((uint)exp >> 1);
            }
            return op2;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static void _shiftLeftOneBit(sbyte[] bits)
        {
            int num = 128;
            int maxValue = (int)sbyte.MaxValue;
            for (int index = 0; index < bits.Length - 1; ++index)
                bits[index] = (sbyte)(((int)bits[index] & maxValue) << 1 | ((int)bits[index + 1] & num) >> 7);
            bits[bits.Length - 1] = (sbyte)(((int)bits[bits.Length - 1] & maxValue) << 1);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private static int _shiftLeftOrRight(sbyte[] res, sbyte[] bits, int sign, int pos, int operation)
        {
            int num1 = pos / 8;
            int num2 = pos % 8;
            if (sign == -1)
                bits = BigInteger._twosComplement(bits);
            if (operation == 1)
            {
                int index1 = res.Length - 1;
                int num3 = 0;
                while (num3 < num1)
                {
                    res[index1] = (sbyte)0;
                    ++num3;
                    index1 += -1;
                }
                if (num2 != 0)
                {
                    int num4 = (int)(Math.Pow(2.0, 8.0) - Math.Pow(2.0, 8 - num2));
                    int num5 = (int)(Math.Pow(2.0, (double)(8 - num2)) - 1.0);
                    res[index1] = (sbyte)(((int)bits[bits.Length - 1] & num5) << num2);
                    int index2 = index1 - 1;
                    int index3 = bits.Length - 1;
                    while (index3 > 0)
                    {
                        res[index2] = (sbyte)(((int)bits[index3 - 1] & num5) << num2 | ((int)bits[index3] & num4) >> 8 - num2);
                        index3 += -1;
                        index2 += -1;
                    }
                    if (index2 >= 0)
                    {
                        res[index2] = (sbyte)(((int)bits[0] & num4) >> 8 - num2);
                        if (sign == -1)
                            res[index2] = (sbyte)((int)res[index2] | num5 << num2);
                    }
                }
                else
                    Array.Copy((Array)bits, 0, (Array)res, index1 - bits.Length + 1, bits.Length);
            }
            else
            {
                int index1 = 0;
                for (int index2 = 0; index2 < num1 && index1 < res.Length; ++index1)
                {
                    res[index1] = sign != -1 ? (sbyte)0 : (sbyte)-1;
                    ++index2;
                }
                if (num2 != 0 && index1 < res.Length)
                {
                    int num3 = (int)(Math.Pow(2.0, 8.0) - Math.Pow(2.0, (double)num2));
                    int num4 = (int)(Math.Pow(2.0, (double)num2) - 1.0);
                    res[index1] = (sbyte)(((int)bits[0] & num3) >> num2);
                    if (sign == -1)
                        res[index1] = (sbyte)((int)res[index1] | num4 << 8 - num2);
                    int index2 = index1 + 1;
                    int index3 = 1;
                    while (index3 < bits.Length - num1)
                    {
                        res[index2] = (sbyte)(((int)bits[index3 - 1] & num4) << 8 - num2 | ((int)bits[index3] & num3) >> num2);
                        ++index3;
                        ++index2;
                    }
                }
                else
                {
                    int index2 = 0;
                    while (index2 < bits.Length - num1)
                    {
                        res[index1] = bits[index2];
                        ++index2;
                        ++index1;
                    }
                }
            }
            if (sign == -1)
                BigInteger._twosComplement(res, res);
            return sign;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static int _subtractBitsArray(sbyte[] res, sbyte[] op1, int op1sign, sbyte[] op2, int op2sign)
        {
            if (op1sign == op2sign)
            {
                BigInteger.AddBitsArray(res, op1, 1, op2, 1);
                return op1sign;
            }
            int num;
            if (op1sign == -1)
            {
                BigInteger.AddBitsArray(res, BigInteger._twosComplement(op1), op1sign, op2, op2sign);
                num = op2sign;
            }
            else
            {
                BigInteger.AddBitsArray(res, op1, op1sign, BigInteger._twosComplement(op2), op2sign);
                num = op1sign;
            }
            if (((int)res[0] & 1) == 0)
            {
                BigInteger._twosComplement(res, res);
                res[0] = (sbyte)0;
                num *= -1;
            }
            else
                res[0] = (sbyte)0;
            return num;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private static sbyte[] _square(sbyte[] op)
        {
            sbyte[] numArray1 = BigInteger._multiply(op, op);
            if ((int)numArray1[0] == 0 && ((int)numArray1[1] & 128) == 0)
            {
                int sourceIndex = 1;
                while (sourceIndex < numArray1.Length && (int)numArray1[sourceIndex] == 0)
                    ++sourceIndex;
                if (sourceIndex + 1 < numArray1.Length && ((int)numArray1[sourceIndex + 1] & 128) != 0)
                    sourceIndex += -1;
                int length = numArray1.Length - sourceIndex;
                if (length < 0)
                    throw new IndexOutOfRangeException();
                sbyte[] numArray2 = new sbyte[length];
                Array.Copy((Array)numArray1, sourceIndex, (Array)numArray2, 0, numArray2.Length);
                numArray1 = numArray2;
            }
            return numArray1;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private static sbyte[] _twosComplement(sbyte[] bval)
        {
            int length = bval.Length;
            if (length < 0)
                throw new IndexOutOfRangeException();
            sbyte[] res = new sbyte[length];
            BigInteger._twosComplement(res, bval);
            return res;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private static void _twosComplement(sbyte[] res, sbyte[] bval)
        {
            sbyte num1 = 1;
            int index = bval.Length - 1;
            while (index >= 0)
            {
                if ((int)num1 != 0)
                {
                    int num2 = (((int)bval[index] ^ -1) & (int)byte.MaxValue) + (int)num1;
                    res[index] = (sbyte)(num2 & (int)byte.MaxValue);
                    num1 = (sbyte)((num2 & 256) >> 8);
                }
                else
                    res[index] = (sbyte)(((int)bval[index] ^ -1) & (int)byte.MaxValue);
                index += -1;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private static int _xorBits(sbyte[] res, sbyte[] op1, int op1sign, sbyte[] op2, int op2sign)
        {
            int index1 = op1.Length - 1;
            int index2 = op2.Length - 1;
            int index3 = res.Length - 1;
            if (op1sign == -1)
                op1 = BigInteger._twosComplement(op1);
            if (op2sign == -1)
                op2 = BigInteger._twosComplement(op2);
            while (index1 >= 0 && index2 >= 0)
            {
                res[index3] = (sbyte)((int)op1[index1] ^ (int)op2[index2]);
                index1 += -1;
                index2 += -1;
                index3 += -1;
            }
            if (index1 >= 0 && index2 < 0)
            {
                while (index1 >= 0)
                {
                    res[index3] = op2sign != -1 ? op1[index1] : (sbyte)((int)op1[index1] ^ (int)byte.MaxValue);
                    index1 += -1;
                    index3 += -1;
                }
            }
            else if (index1 < 0 && index2 >= 0)
            {
                while (index2 >= 0)
                {
                    res[index3] = op1sign != -1 ? op2[index2] : (sbyte)((int)op1[index1] ^ (int)byte.MaxValue);
                    index2 += -1;
                    index3 += -1;
                }
            }
            if (op1sign != -1 && op2sign != -1)
                return 1;
            BigInteger._twosComplement(res, res);
            return -1;
        }

        #endregion

    }
}
