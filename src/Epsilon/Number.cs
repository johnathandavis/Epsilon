using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsilon
{
    public abstract class Number
    {
        internal static string TrimRadix(string str, int radix)
        {
            int offset = 0;
            if (radix == 8)
                offset = 1;
            else if (radix == 16)
                offset = (int)str[0] != 48 ? 1 : 2;
            return str.Substring(offset);
        }
        internal static int FindRadix(string str)
        {
            if (str == null)
                throw new NullReferenceException();
            if (str == string.Empty)
                throw new FormatException(str);
            if ((int)str[0] == 48)
            {
                if (str.Length <= 1)
                    return 10;
                return (int)str[1] == 120 ? 16 : 8;
            }
            return (int)str[0] == 35 ? 16 : 10;
        }
        internal static long ParseLong(string str, int radix, long MIN_VALUE, long MAX_VALUE)
        {
            if (str == null || str == string.Empty || (radix < 2 || radix > 36))
                throw new FormatException(str);
            int num1 = 0;
            int num2 = str.Length;
            bool flag = false;
            if ((int)str[0] == 45)
            {
                flag = true;
                ++num1;
            }
            if (num1 == num2)
                throw new FormatException(str);
            long num3 = 0;
            long num4 = MAX_VALUE / radix;
            for (int index = num1; index < num2; ++index)
            {
                long num5 = Convert.ToInt64(str[index].ToString(), radix);
                if (num5 < 0L)
                    throw new FormatException(str);
                if (num3 < num4)
                    num3 = num3 * radix + num5;
                else if (num3 == num4)
                {
                    if (index < num2 - 1)
                        throw new FormatException(str);
                    num3 = num3 * radix + num5;
                    if (num3 < 0L || num3 > MAX_VALUE)
                    {
                        if (!flag)
                            throw new FormatException(str);
                        if (num3 != MAX_VALUE + 1L)
                            throw new FormatException(str);
                    }
                }
                else
                {
                    if (!flag || index != num2 - 1 || num5 != 0L)
                        throw new FormatException(str);
                    num3 *= (long)radix;
                    if (num3 != MAX_VALUE + 1L)
                        throw new FormatException(str);
                }
            }
            if (flag)
                return -num3;
            return num3;
        }


        public virtual sbyte AsByte()
        {
            return (sbyte)this.AsInt();
        }
        public virtual short AsShort()
        {
            return (short)this.AsInt();
        }
        public abstract double AsDouble();
        public abstract float AsFloat();
        public abstract int AsInt();
        public abstract long AsLong();

        public new virtual object MemberwiseClone()
        {
            Number number = this;
            return number.MemberwiseClone();
        }

    }
}
