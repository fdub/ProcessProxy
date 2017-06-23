using System;
using Castle.DynamicProxy;

namespace ProcessProxy
{
    public class Interceptor<T> : IInterceptor where T : class, new()
    {
        private T _instance;

        public Interceptor()
        {
            _instance = Activator.CreateInstance<T>();
        }

        public void Intercept(IInvocation invocation)
        {
            var result = invocation.Method.Invoke(_instance, invocation.Arguments);

            if (invocation.Method.ReturnType != null)
            {
                invocation.ReturnValue = result;   
            }
        }
    }
}
