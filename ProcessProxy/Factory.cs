using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;

namespace ProcessProxy
{
    public static class Factory 
    {
        public static TInterface Create<TInterface, TClass>() 
            where TInterface : class 
            where TClass : class, TInterface, new()
        {
            return new ProxyGenerator()
                .CreateInterfaceProxyWithoutTarget<TInterface>(new Interceptor<TClass>());
        }
    }
}
