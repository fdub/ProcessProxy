using ProcessProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestClass;

namespace Demo
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
            port = 33033
            hostname = localhost
        }
    }

}


akka.suppress-json-serializer-warning = on
";
        static void Main(string[] args)
        {
            var factory = new Factory(config);
            var proxy = factory.Create<IBadLib, BadLib>();
            proxy.ShowMessage("test");
        }
    }
}
