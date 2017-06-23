using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using Akka.Actor;
using System;

namespace ProcessProxy
{
    public class Factory 
    {
        private ActorSystem _system;

        public Factory()
        {
            _system = ActorSystem.Create("ProxyFactory");
        }

        public TInterface Create<TInterface, TClass>() 
            where TInterface : class 
            where TClass : class, TInterface, new()
        {
            var actor = _system.ActorOf(Props.Create(() => new InvocatorActor<TClass>()));
            return new ProxyGenerator()
                .CreateInterfaceProxyWithoutTarget<TInterface>(new SenderInterceptor(actor));
        }
    }



    public class InvocatorActor<T> : UntypedActor where T : class, new()
    {
        private T _instance;

        public InvocatorActor()
        {
            _instance = Activator.CreateInstance<T>();
        }

        protected override void OnReceive(object message)
        {
            switch(message)
            {
                case IInvocation i:
                    Intercept(i);
                    break;
                default:
                    Unhandled(message);
                    break;
            }
        }

        private void Intercept(IInvocation invocation)
        {
            var result = invocation.Method.Invoke(_instance, invocation.Arguments);

            if (invocation.Method.ReturnType != null)
            {
                invocation.ReturnValue = result;
            }

            Sender.Tell(invocation);
        }
    }
}
