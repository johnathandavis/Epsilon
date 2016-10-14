using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epsilon.Internal
{
    internal class FactorialCache
    {
        private static Dictionary<int, BigInteger> Cache
        {
            get
            {
                return _factorialCacheLazy.Value;
            }
        }

        private static Lazy<Dictionary<int, BigInteger>> _factorialCacheLazy =
            new Lazy<Dictionary<int, BigInteger>>(() => GenerateFactorialCache(Configuration.FACTORIAL_CACHE_BOUND));

        private static Dictionary<int, BigInteger> GenerateFactorialCache(int upperBound)
        {
            var dict = new Dictionary<int, BigInteger>();
            BigInteger last = null;
            for (int x = 0; x <= upperBound; x++)
            {
                last = PartialFactorial(x, last);
                dict.Add(x, last);
            }

            return dict;
        }

        private static BigInteger PartialFactorial(int x, BigInteger lastProduct)
        {
            if (x == 0) return Numbers.Unity.ToBigInteger();
            if (x == 1) return Numbers.Unity.ToBigInteger();

            BigInteger y = new BigInteger(x.ToString());
            return lastProduct.Multiply(y);
        }

        internal static BigInteger Factorial(int x)
        {
            BigInteger result;
            if (Cache.TryGetValue(x, out result))
                return result;

            result = Cache[Cache.Count - 1];
            for (int y = Cache.Count; y <= x; y++)
            {
                result = PartialFactorial(y, result);
                Cache.Add(y, result);
                System.Diagnostics.Debug.WriteLine(y);
            }
            return result;
        }
    }
}
