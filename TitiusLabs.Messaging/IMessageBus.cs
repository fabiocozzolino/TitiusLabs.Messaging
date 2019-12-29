using System;

namespace TitiusLabs.Messaging
{
    public interface IMessageBus
    {
        event EventHandler<PublishExceptionEventArgs> PublishException;
        void Publish<TMessage>(TMessage message) where TMessage : IMessage;
        ISubscriptionToken Subscribe<TMessage>(Action<TMessage> subscriber) where TMessage : IMessage;
        ISubscriptionToken Subscribe<TMessage>(Action<TMessage> subscriber, Func<TMessage, bool> filter) where TMessage : IMessage;
        void Unsubscribe(ISubscriptionToken subscriptionToken);
        void Unsubscribe<TMessage>(Action<TMessage> subscriber) where TMessage : IMessage;
        void UnsubscribeAll<TMessage>() where TMessage : IMessage;
    }
}
