using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TitiusLabs.Messaging
{
    internal class MessageBusImpl : IMessageBus
    {
        private readonly EventWaitHandle waitHandler = new EventWaitHandle(false, EventResetMode.ManualReset);
        private readonly Queue<IMessage> messages = new Queue<IMessage>();
        private readonly IDictionary<Type, List<MessageSubscription>> messageSubscriptions = new Dictionary<Type, List<MessageSubscription>>();

        public MessageBusImpl()
        {
            Initialize();
        }

        private void Initialize()
        {
            // start the messaging process
            Task.Run(ProcessMessages);
        }

        public void Subscribe<TMessage>(Action<TMessage> subscriber) where TMessage : IMessage
        {
            Subscribe(subscriber, null);
        }

        public void Subscribe<TMessage>(Action<TMessage> subscriber, Func<TMessage, bool> filter) where TMessage : IMessage
        {
            var messageType = typeof(TMessage);
            if (!messageSubscriptions.ContainsKey(messageType))
            {
                messageSubscriptions[messageType] = new List<MessageSubscription>();
            }

            // retrieve the synchronizationcontext to invoke subscriber in their context
            TaskScheduler scheduler = SynchronizationContext.Current != null
                ? TaskScheduler.FromCurrentSynchronizationContext()
                : TaskScheduler.Default;

            // add subscriber to list
            messageSubscriptions[messageType].Add(MessageSubscription.Create<TMessage>(subscriber, filter, scheduler));
        }

        public void UnSubscribe<TMessage>(Action<TMessage> subscriber) where TMessage : IMessage
        {
            var messageType = typeof(TMessage);
            if (!messageSubscriptions.ContainsKey(messageType))
            {
                return;
            }

            // remove the single subscriber
            var messageSubscribers = messageSubscriptions[messageType];
            var subscriberItem = messageSubscribers
                .OfType<MessageSubscription<TMessage>>()
                .FirstOrDefault(s => s.Subscriber.Equals(subscriber));
            messageSubscribers.Remove(subscriberItem);

            // check if any then remove the key
            if (!messageSubscribers.Any())
            {
                messageSubscriptions.Remove(messageType);
            }
        }

        public void UnSubscribeAll<TMessage>() where TMessage : IMessage
        {
            var messageType = typeof(TMessage);
            if (!messageSubscriptions.ContainsKey(messageType))
            {
                return;
            }

            messageSubscriptions.Remove(messageType);
        }

        public void Publish<TMessage>(TMessage message) where TMessage : IMessage
        {
            messages.Enqueue(message);
            waitHandler.Set();
        }

        private void ProcessMessages()
        {
            while (true)
            {
                // wait for signals
                waitHandler.WaitOne();
                waitHandler.Reset();

                while (true)
                {
                    // check if any messages to dequeue
                    if (!messages.Any())
                    {
                        break;
                    }

                    var message = messages.Dequeue();
                    foreach (var messageSubscription in messageSubscriptions[message.GetType()])
                    {
                        Task.Factory.StartNew(() =>
                        {
                            messageSubscription.Invoke(message);
                        }, CancellationToken.None, TaskCreationOptions.None, messageSubscription.Scheduler);
                    }
                }
            }
        }
    }
}
