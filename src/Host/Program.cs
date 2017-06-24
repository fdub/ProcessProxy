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

        static void Main(string[] args)
        {
            Console.WriteLine(string.Join("\n", args));
            var actorPath = args[0];
            var guid = args[1];
            var asm = args[2];
            var type = args[3];

            var hocon = Akka.Configuration.ConfigurationFactory.ParseString(config);
            using (var system = ActorSystem.Create("Host", hocon))
            {
                var actor = system.ActorOf(Props.Create(() => new InvocatorActor(asm, type)));
                var proxy = system
                    .ActorSelection(actorPath)
                    .ResolveOne(TimeSpan.FromSeconds(5))
                    .GetAwaiter()
                    .GetResult();
                proxy.Tell($"register:{guid}", actor);

                system.WhenTerminated.GetAwaiter().GetResult();
            }
        }
    }
}
