using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using Akka.Actor;
using Akka.Configuration;
using System;

namespace ProcessProxy
{
    public class Factory 
    {
        private ActorSystem _system;
        private IActorRef _registry;
        private IActorRef _dispatcher;

        public Factory(string config)
        {
            var hocon = ConfigurationFactory.ParseString(config);
            _system = ActorSystem.Create("ProxyFactory", hocon);
            
            _registry = _system.ActorOf(Props.Create(() => new RegistryActor("akka.tcp://ProxyFactory@localhost:33033")), "registry");
            _dispatcher = _system.ActorOf(Props.Create(() => new DispatcherActor(_registry)), "dispatcher");
        }

        public TInterface Create<TInterface, TClass>() 
            where TInterface : class 
            where TClass : class, TInterface, new()
        {
            var guid = Guid.NewGuid();
            var asm = typeof(TClass).Assembly.FullName;
            var type = typeof(TClass).Name;
            var actor = _dispatcher.Ask<IActorRef>(new Create(asm, type)).GetAwaiter().GetResult();

            return new ProxyGenerator()
                .CreateInterfaceProxyWithoutTarget<TInterface>(new SenderInterceptor(actor));
        }
    }
}
