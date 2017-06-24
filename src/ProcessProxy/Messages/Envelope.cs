using Akka.Actor;

namespace ProcessProxy.Messages
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
}
