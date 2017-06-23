using System.Collections.Generic;
using Akka.Actor;
using System;
using System.Diagnostics;
using System.IO;

namespace ProcessProxy
{
    public class Envelope<T>
    {
        public T Msg { get; }
        public IActorRef Sender { get; }

        public Envelope(IActorRef sender, T msg)
        {
            Sender = sender;
            Msg = msg;
        }
    }

    public static class Envelope
    {
        public static Envelope<T> Create<T>(IActorRef sender, T msg)
        {
            return new Envelope<T>(sender, msg);
        }
    }

    public class Create
    {
        public string Asm { get; }
        public string Type { get; }

        public Create(string asm, string type)
        {
            Asm = asm;
            Type = type;
        }
    }
    public class RegistryActor : UntypedActor
    {
        private readonly Dictionary<Guid, Envelope<IActorRef>> _senders = new Dictionary<Guid, Envelope<IActorRef>>();
        private readonly string _endpoint;

        public RegistryActor(string endpoint)
        {
            _endpoint = new Uri(new Uri(endpoint), "user/registry").ToString();
        }
        
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case Envelope<Create> c:
                    {
                        var guid = Guid.NewGuid();
                        var asm = c.Msg.Asm;
                        var type = c.Msg.Type;
                        var cmd = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Host.exe");
                        var process = Process.Start(cmd, $"\"{_endpoint}\" {guid} \"{asm}\" \"{type}\"");
                        _senders.Add(guid, Envelope.Create(Sender, c.Sender));
                        break;
                    }
                case string s when s.StartsWith("register:"):
                    {
                        var guid = new Guid(s.Split(':')[1]);
                        var returnTo = _senders[guid];
                        returnTo.Sender.Tell(new Envelope<IActorRef>(returnTo.Msg, Sender));
                        break;
                    }
                default:
                    Unhandled(message);
                    break;
            }
        }
    }
}
