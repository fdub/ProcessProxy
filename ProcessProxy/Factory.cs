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
    public class Factory : IDisposable
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

            var scope = new ModuleScope(
                true,
                true, 
                ModuleScope.DEFAULT_ASSEMBLY_NAME, 
                ModuleScope.DEFAULT_FILE_NAME,
                $"ProcessProxy.{typeof(TClass).Name}.Gen",
                $"ProcessProxy.{typeof(TClass).Name}.Gen.dll");
            var builder = new DefaultProxyBuilder(scope);

            var result = new ProxyGenerator(builder)
                .CreateInterfaceProxyWithoutTarget<TInterface>(ProxyGenerationOptions.Default, new SenderInterceptor(actor));
            scope.SaveAssembly();
            actor.Tell($"load|ProcessProxy.{typeof(TClass).Name}.Gen.dll");
            return result;
        }

        public void Dispose()
        {
            _system.Terminate().Wait();
        }
    }
}
