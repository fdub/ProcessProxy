using ProcessProxy;
using System.Diagnostics;
using System.Linq;
using DemoLib;

namespace Demo
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
            port = 33033
            hostname = localhost
        }
    }
}
";
        static void Main(string[] args)
        {
            using (var factory = new Factory(config))
            using (var proxy = factory.Create<IDemoClass, DemoClass>())
            {
                var greeting = proxy.GetGreeting();
                proxy.ShowMessage(greeting);
                proxy.ShowMessage(proxy.TypeName);

                var sw = new Stopwatch();
                sw.Start();
                var a = 0;
                foreach (var i in Enumerable.Range(0, 1_000))
                {
                    a = proxy.Add(a, i);
                }
                sw.Stop();

                proxy.ShowMessage($"Added numbers from 0 to 1000 in {sw.ElapsedMilliseconds} ms to: {a}");
            }
        }
    }
}
