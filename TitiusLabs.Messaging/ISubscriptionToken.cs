using System;

namespace TitiusLabs.Messaging
{
    public interface ISubscriptionToken : IDisposable
    {
        Type MessageType { get; }
    }

    internal class SubscriptionToken : ISubscriptionToken
    {
        private readonly IMessageBus bus;
        
        public SubscriptionToken(IMessageBus bus, Type messageType)
        {
            this.bus = bus;
            MessageType = messageType;
        }

        public Type MessageType { get; }

        public void Dispose()
        {
            if (bus != null)
            {
                bus.Unsubscribe(this);
            }
        }
    }
}
