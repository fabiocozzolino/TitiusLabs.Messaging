using System;

namespace TitiusLabs.Messaging
{
    public sealed class MessageBus
    {
        private static readonly object syncObj = new object();
        private static MessageBus current;
        private readonly MessageBusImpl messageBusImpl;

        public static MessageBus Current
        {
            get
            {
                if (current == null)
                {
                    lock(syncObj)
                    {
                        if (current == null)
                        {
                            current = new MessageBus(new MessageBusImpl());
                        }
                    }
                }

                return current;
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

        public void UnSubscribe<TMessage>(Action<TMessage> subscriber) where TMessage : IMessage
        {
            messageBusImpl.UnSubscribe(subscriber);
        }

        public void UnSubscribe(ISubscriptionToken subscriptionToken)
        {
            messageBusImpl.UnSubscribe(subscriptionToken);
        }

        public void UnSubscribeAll<TMessage>() where TMessage : IMessage
        {
            messageBusImpl.UnSubscribeAll<TMessage>();
        }

        public void Post<TMessage>(TMessage message) where TMessage : IMessage
        {
            messageBusImpl.Publish(message);
        }
    }
}
