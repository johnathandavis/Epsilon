using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epsilon
{
    interface INumber<T>
    {
        T Max(T val);
        T Min(T val);
        T Abs();
        T Negate();
        T Add(T val);
        T Subtract(T val);
        T Multiply(T val);
        T Divide(T val);
        int SigNum();
    }
}
