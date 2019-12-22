﻿using System;

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

        public void Subscribe<TMessage>(Action<TMessage> subscriber) where TMessage : IMessage
        {
            messageBusImpl.Subscribe(subscriber);
        }

        public void UnSubscribeAll<TMessage>() where TMessage : IMessage
        {
            messageBusImpl.UnSubscribeAll<TMessage>();
        }

        public void Post<TMessage>(TMessage message) where TMessage : IMessage
        {
            messageBusImpl.Post(message);
        }
    }
}
