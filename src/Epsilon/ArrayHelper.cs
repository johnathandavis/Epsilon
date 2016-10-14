using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epsilon
{
    internal static class ArrayHelper
    {
        public static T[][] InitMultiDArray<T>(int length, int width)
        {
            T[][] array = new T[length][];
            for (int x = 0; x < length; x++)
            {
                array[x] = new T[width];
            }
            return array;
        }
    }
}
