using System;

namespace TitiusLabs.Messaging
{
    public interface IMessageBus
    {
        void Post<TMessage>(TMessage message) where TMessage : IMessage;
        void Subscribe<TMessage>(Action<TMessage> subscriber) where TMessage : IMessage;
        void UnSubscribe<TMessage>(Action<TMessage> subscriber) where TMessage : IMessage;
        void UnSubscribeAll<TMessage>() where TMessage : IMessage;
    }
}
