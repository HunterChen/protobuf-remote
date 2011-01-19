using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrimeClient
{
    public interface ISampleService
    {
        bool TestPrime(int x);

        IAsyncResult BeginTestPrime(int x, AsyncCallback callback, object state);

        bool EndTestPrime(IAsyncResult asyncResult);
    }
}
