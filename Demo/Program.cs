using ProcessProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestClass;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new Factory();
            var proxy = factory.Create<IBadLib, BadLib>();
            proxy.ShowMessage(proxy.GreetMe());
        }
    }
}
