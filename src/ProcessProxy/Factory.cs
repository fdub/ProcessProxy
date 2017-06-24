using Castle.DynamicProxy;
using Akka.Actor;
using Akka.Configuration;
using System;
using ProcessProxy.Messages;

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
            var classType = typeof(TClass);
            var actor = CreateHostActor(classType);

            var scope = CreateModuleScope(classType);
            var proxyGenerator = new ProxyGenerator(new DefaultProxyBuilder(scope));

            var result = proxyGenerator
                .CreateInterfaceProxyWithoutTarget<TInterface>(new SenderInterceptor(actor));

            scope.SaveAssembly();
            actor.Tell($"load|{GetAssemblyPath(classType)}");
            return result;
        }

        private IActorRef CreateHostActor(Type classType)
        {
            var asm = classType.Assembly.FullName;
            var type = classType.Name;
            return _dispatcher
                .Ask<IActorRef>(new Create(asm, type))
                .GetAwaiter()
                .GetResult();
        }

        private static ModuleScope CreateModuleScope(Type classType)
        {
            return new ModuleScope(
                true,
                true,
                ModuleScope.DEFAULT_ASSEMBLY_NAME,
                ModuleScope.DEFAULT_FILE_NAME,
                GetAssemblyName(classType),
                GetAssemblyPath(classType));
        }

        private static string GetAssemblyName(Type type) => $"ProcessProxy.{type.Name}.Gen";
        private static string GetAssemblyPath(Type type) => GetAssemblyName(type) + ".dll";

        public void Dispose()
        {
            _system.Terminate().GetAwaiter().GetResult();
            _system.Dispose();
        }
    }
}
