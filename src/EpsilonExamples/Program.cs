using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Epsilon;

namespace EpsilonExamples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DateTime now = DateTime.Now;
            BigMath.Factorial(1000);
            TimeSpan duration = DateTime.Now - now;
            Console.WriteLine("Took " + duration.ToString() + " uncached.");
            now = DateTime.Now;
            BigMath.Factorial(1000);
            duration = DateTime.Now - now;
            Console.WriteLine("Took " + duration.ToString() + " cached.");

            var two = new BigDecimal(2.0);
            Console.WriteLine(two);
            var sqrt = BigMath.Sqrt(two);
            Console.WriteLine(sqrt);
            var sin = BigMath.Sin(two);
            Console.WriteLine(sin);
            var cos = BigMath.Cos(two);
            Console.WriteLine(cos);
            Console.ReadKey();
        }
    }
}
