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
        private readonly Dictionary<Guid, (IActorRef dispatcher, IActorRef factory)> _senders_buffer;

        private readonly string _actorPath;

        public RegistryActor(Uri endpoint)
        {
            _senders_buffer = new Dictionary<Guid, (IActorRef, IActorRef)>();
            _actorPath = new Uri(endpoint, string.Join("/", Self.Path.Elements)).ToString();
        }
        
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case Envelope<Create> c:
                    {
                        var guid = Guid.NewGuid();

                        var cmd = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Host.exe");
                        var args = $"\"{_actorPath}\" {guid} \"{c.Msg.Asm}\" \"{c.Msg.Type}\"";

                        Process.Start(cmd, args);
                        _senders_buffer.Add(guid, (Sender, c.Sender));
                        break;
                    }
                case string s when s.StartsWith("register:"):
                    {
                        var guid = new Guid(s.Split(':')[1]);
                        var refs = _senders_buffer[guid];
                        refs.dispatcher.Tell(new Envelope<IActorRef>(refs.factory, Sender));
                        _senders_buffer.Remove(guid);
                        break;
                    }
                default:
                    Unhandled(message);
                    break;
            }
        }
    }
}
