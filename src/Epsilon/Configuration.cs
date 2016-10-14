using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epsilon
{
    public class Configuration
    {
        internal const int FACTORIAL_CACHE_BOUND = 100;

        private static BigDecimal epsilon;
        internal static BigDecimal Epsilon
        {
            get
            {
                if (epsilon == null)
                {
                    string epsilonStr = "";
                    for (int x = 0; x < EpsilonPrecisionDigits - 1; x++)
                    {
                        epsilonStr += "0";
                    }
                    epsilonStr = "0." + epsilonStr + "1";
                    epsilon = new BigDecimal(epsilonStr);
                }
                return epsilon;
            }
        }

        public static int EpsilonPrecisionDigits { get; set; } = 100;
        public static int TrigTaylorSeriesIterations { get; set; } = 100;
    }
}
