using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Host
{
    class Program
    {
        const string config = @"
akka {
    actor {
        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
    }

    remote {
        helios.tcp {
            hostname = localhost
        }
    }
}

akka.suppress-json-serializer-warning = on
";
        private static ActorSystem _system;

        static void Main(string[] args)
        {
            Console.WriteLine(string.Join("\n", args));
            var actorPath = args[0];
            var guid = args[1];
            var asm = args[2];
            var type = args[3];

            var hocon = Akka.Configuration.ConfigurationFactory.ParseString(config);
            _system = ActorSystem.Create("Host", hocon);
            
            var actor = _system.ActorOf(Props.Create(() => new InvocatorActor(asm, type)));
            var proxy = _system.ActorSelection(actorPath).ResolveOne(TimeSpan.FromSeconds(5)).Result;
            proxy.Tell($"register:{guid}", actor);

            _system.WhenTerminated.Wait();
        }
    }
}
