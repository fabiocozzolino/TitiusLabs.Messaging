using System;

namespace TitiusLabs.Messaging
{
    public interface IMessageBus
    {
        void Publish<TMessage>(TMessage message) where TMessage : IMessage;
        void Subscribe<TMessage>(Action<TMessage> subscriber) where TMessage : IMessage;
        void Subscribe<TMessage>(Action<TMessage> subscriber, Func<TMessage, bool> filter) where TMessage : IMessage;
        void UnSubscribe<TMessage>(Action<TMessage> subscriber) where TMessage : IMessage;
        void UnSubscribeAll<TMessage>() where TMessage : IMessage;
    }
}
