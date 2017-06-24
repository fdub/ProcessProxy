using System.Collections.Generic;
using Akka.Actor;
using System;
using System.Diagnostics;
using System.IO;
using ProcessProxy.Messages;
using Envelope = ProcessProxy.Messages.Envelope;

namespace ProcessProxy
{
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

                        var cmd = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Host.exe");
                        var args = $"\"{_endpoint}\" {guid} \"{c.Msg.Asm}\" \"{c.Msg.Type}\"";

                        Process.Start(cmd, args);
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
