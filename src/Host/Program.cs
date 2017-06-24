using Akka.Actor;
using System;

namespace Host
{
    class Program
    {
        const string config = @"
akka {
    actor {
        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
        serializers {
            hyperion = ""Akka.Serialization.HyperionSerializer, Akka.Serialization.Hyperion""
        }
        serialization-bindings {
            ""System.Object"" = hyperion
        }
    }

    remote {
        helios.tcp {
            hostname = localhost
        }
    }
}
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
