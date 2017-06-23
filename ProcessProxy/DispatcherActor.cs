using Akka.Actor;

namespace ProcessProxy
{
    public class DispatcherActor : UntypedActor
    {
        private IActorRef _registry;

        public DispatcherActor(IActorRef registry)
        {
            _registry = registry;
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case Envelope<IActorRef> e:
                    e.Sender.Tell(e.Msg);
                    break;
                case Create c:
                    _registry
                        .Ask<Envelope<IActorRef>>(Envelope.Create(Sender, c))
                        .PipeTo(Self);
                    break;
                default:
                    Unhandled(message);
                    break;
            }
        }
    }
}
