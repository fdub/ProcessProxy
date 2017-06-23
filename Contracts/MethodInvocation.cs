using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public class MethodInvocation
    {
        public string Method { get; }
        public object[] Args { get; }

        public MethodInvocation(string method, object[] args)
        {
            Method = method;
            Args = args;
        }
    }
}
