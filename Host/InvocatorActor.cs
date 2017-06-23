using Akka.Actor;
using Castle.DynamicProxy;
using System;

namespace Host
{
    public class InvocatorActor : UntypedActor
    {
        private object _instance;

        public InvocatorActor(string asm, string type)
        {
            var assembly = AppDomain.CurrentDomain.Load(asm);
            _instance = assembly.CreateInstance(type);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
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
