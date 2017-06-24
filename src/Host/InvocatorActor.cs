using Akka.Actor;
using Castle.DynamicProxy;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Host
{
    public class InvocatorActor : UntypedActor
    {
        private object _instance;

        public InvocatorActor(string assemblyName, string typeName)
        {
            var assembly = AppDomain.CurrentDomain.Load(assemblyName);
            var type = assembly.DefinedTypes.FirstOrDefault(t => t.Name == typeName);
            _instance = Activator.CreateInstance(type);

            Console.WriteLine($"Instance from type {typeName} created: {_instance}");
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case string s when s.StartsWith("load|"):
                    {
                        Assembly.LoadFrom(s.Split('|')[1]);
                        Console.WriteLine($"Loaded assembly: {s}");
                        break;
                    }
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
            
            if (invocation.Method.Name == nameof(IDisposable.Dispose))
            {
                Context.System.Terminate();
            }
        }
    }
}
