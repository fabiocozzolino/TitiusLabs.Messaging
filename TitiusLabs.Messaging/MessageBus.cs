using System;

namespace TitiusLabs.Messaging
{
    public sealed class MessageBus
    {
        private readonly MessageBusImpl messageBusImpl;

        private static readonly Lazy<MessageBus> instance = new Lazy<MessageBus>(() => new MessageBus(new MessageBusImpl()));

        public static MessageBus Current
        {
            get
            {
                return instance.Value;
            }
        }

        private MessageBus(MessageBusImpl messageBusImpl)
        {
            this.messageBusImpl = messageBusImpl;
        }

        public ISubscriptionToken Subscribe<TMessage>(Action<TMessage> subscriber) where TMessage : IMessage
        {
            return Subscribe(subscriber, null);
        }

        public ISubscriptionToken Subscribe<TMessage>(Action<TMessage> subscriber, Func<TMessage, bool> filter) where TMessage : IMessage
        {
            return messageBusImpl.Subscribe(subscriber, filter);
        }

        public void Unsubscribe<TMessage>(Action<TMessage> subscriber) where TMessage : IMessage
        {
            messageBusImpl.Unsubscribe(subscriber);
        }

        public void Unsubscribe(ISubscriptionToken subscriptionToken)
        {
            messageBusImpl.Unsubscribe(subscriptionToken);
        }

        public void UnsubscribeAll<TMessage>() where TMessage : IMessage
        {
            messageBusImpl.UnsubscribeAll<TMessage>();
        }

        public void Publish<TMessage>(TMessage message) where TMessage : IMessage
        {
            messageBusImpl.Publish(message);
        }
    }
}
