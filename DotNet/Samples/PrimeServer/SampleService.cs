using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrimeServer
{
    class SampleService : ISampleService
    {
        public bool TestPrime(int x)
        {
            if (x == 1)
                return false;
            if (x == 2)
                return true;
            if ((x & 1) == 0)
                return false;
            for (int divisor=3; divisor<x; divisor+=2)
            {
                if ((x % divisor) == 0)
                    return false;
            }
            return true;
        }
    }
}
