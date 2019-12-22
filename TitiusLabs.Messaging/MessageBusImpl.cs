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
        private readonly IDictionary<Type, List<Tuple<Delegate, TaskScheduler>>> subscribers = new Dictionary<Type, List<Tuple<Delegate, TaskScheduler>>>();

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
            var messageType = typeof(TMessage);
            if (!subscribers.ContainsKey(messageType))
            {
                subscribers[messageType] = new List<Tuple<Delegate, TaskScheduler>>();
            }

            // retrieve the synchronizationcontext to invoke subscriber in their context
            TaskScheduler scheduler = SynchronizationContext.Current != null
                ? TaskScheduler.FromCurrentSynchronizationContext()
                : TaskScheduler.Default;

            // add subscriber to list
            subscribers[messageType].Add(Tuple.Create((Delegate)subscriber, scheduler));
        }

        public void UnSubscribe<TMessage>(Action<TMessage> subscriber) where TMessage : IMessage
        {
            var messageType = typeof(TMessage);
            if (!subscribers.ContainsKey(messageType))
            {
                return;
            }

            // remove the single subscriber
            var messageSubscribers = subscribers[messageType];
            var subscriberItem = messageSubscribers.FirstOrDefault(s => s.Item1.Equals(subscriber));
            messageSubscribers.Remove(subscriberItem);

            // check if any then remove the key
            if (!messageSubscribers.Any())
            {
                subscribers.Remove(messageType);
            }
        }

        public void UnSubscribeAll<TMessage>() where TMessage : IMessage
        {
            var messageType = typeof(TMessage);
            if (!subscribers.ContainsKey(messageType))
            {
                return;
            }

            subscribers.Remove(messageType);
        }

        public void Post<TMessage>(TMessage message) where TMessage : IMessage
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

                while (true)
                {
                    if (!messages.Any())
                    {
                        break;
                    }

                    var message = messages.Dequeue();
                    foreach (var subscriber in subscribers[message.GetType()])
                    {
                        Task.Factory.StartNew(() =>
                        {
                            subscriber.Item1.DynamicInvoke(message);
                        }, CancellationToken.None, TaskCreationOptions.None, subscriber.Item2);
                    }
                }
            }
        }
    }
}
