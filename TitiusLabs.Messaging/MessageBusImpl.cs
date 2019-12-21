using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TitiusLabs.Messaging
{
    internal class MessageBusImpl : IMessageBus
    {
        private bool isRunning;
        private readonly Queue<IMessage> messages = new Queue<IMessage>();
        private readonly IDictionary<Type, List<Tuple<Delegate, TaskScheduler>>> subscribers = new Dictionary<Type, List<Tuple<Delegate, TaskScheduler>>>();

        public void Subscribe<TMessage>(Action<TMessage> subscriber) where TMessage : IMessage
        {
            if (!subscribers.ContainsKey(typeof(TMessage)))
            {
                subscribers[typeof(TMessage)] = new List<Tuple<Delegate, TaskScheduler>>();
            }

            TaskScheduler scheduler = null;
            if (SynchronizationContext.Current != null)
            {
                scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            }

            subscribers[typeof(TMessage)].Add(Tuple.Create((Delegate)subscriber, scheduler));
        }

        public void Post<TMessage>(TMessage message) where TMessage : IMessage
        {
            messages.Enqueue(message);
            Task.Run(ProcessQueue);
        }

        private void ProcessQueue()
        {
            if (isRunning)
                return;

            while (true)
            {
                isRunning = true;

                var message = messages.Dequeue();
                foreach (var subscriber in subscribers[message.GetType()])
                {
                    if (subscriber.Item2 != null)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            subscriber.Item1.DynamicInvoke(message);
                        }, CancellationToken.None, TaskCreationOptions.None, subscriber.Item2);
                    }
                    else
                    {
                        subscriber.Item1.DynamicInvoke(message);
                    }
                }

                if (messages.Count == 0)
                {
                    isRunning = false;
                    break;
                }
            }
        }
    }
}
