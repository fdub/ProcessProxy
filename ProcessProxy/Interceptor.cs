using System;
using Castle.DynamicProxy;
using Akka.Actor;

namespace ProcessProxy
{
    public class SenderInterceptor : IInterceptor
    {
        private IActorRef _target;

        public SenderInterceptor(IActorRef target)
        {
            _target = target;
        }

        public void Intercept(IInvocation invocation)
        {
            var result = _target.Ask<object>(/* TODO Invocation */).GetAwaiter().GetResult();
            invocation.ReturnValue = result;
        }
    }
}
