using System;

namespace TitiusLabs.Messaging
{
    public interface IMessageBus
    {
        void Publish<TMessage>(TMessage message) where TMessage : IMessage;
        ISubscriptionToken Subscribe<TMessage>(Action<TMessage> subscriber) where TMessage : IMessage;
        ISubscriptionToken Subscribe<TMessage>(Action<TMessage> subscriber, Func<TMessage, bool> filter) where TMessage : IMessage;
        void UnSubscribe(ISubscriptionToken subscriptionToken);
        void UnSubscribe<TMessage>(Action<TMessage> subscriber) where TMessage : IMessage;
        void UnSubscribeAll<TMessage>() where TMessage : IMessage;
    }
}
