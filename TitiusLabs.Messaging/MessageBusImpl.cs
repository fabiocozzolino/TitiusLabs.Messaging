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
        private readonly Queue<IMessage> messageQueue = new Queue<IMessage>();
        private readonly Queue<IMessage> deadMessageQueue = new Queue<IMessage>();
        private readonly IDictionary<Type, List<MessageSubscription>> messageSubscriptions = new Dictionary<Type, List<MessageSubscription>>();

        protected virtual void OnPublishException(PublishExceptionEventArgs e)
        {
            PublishException?.Invoke(this, e);
        }

        public event EventHandler<PublishExceptionEventArgs> PublishException;

        public MessageBusImpl()
        {
            Initialize();
        }

        private void Initialize()
        {
            // start the messaging process
            Task.Run(ProcessMessages);
        }

        public ISubscriptionToken Subscribe<TMessage>(Action<TMessage> subscriber) where TMessage : IMessage
        {
            return Subscribe(subscriber, null);
        }

        public ISubscriptionToken Subscribe<TMessage>(Action<TMessage> subscriber, Func<TMessage, bool> filter) where TMessage : IMessage
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
            var messageSubscription = MessageSubscription.Create(subscriber, filter, scheduler);
            messageSubscription.SubscriptionToken = new SubscriptionToken(this, messageType);
            messageSubscriptions[messageType].Add(messageSubscription);

            return messageSubscription.SubscriptionToken;
        }

        public void Unsubscribe(ISubscriptionToken subscriptionToken)
        {
            Type messageType = subscriptionToken.MessageType;
            if (!messageSubscriptions.ContainsKey(messageType))
            {
                return;
            }

            // remove the single subscriber
            var messageSubscribers = messageSubscriptions[messageType];
            var subscriberItem = messageSubscribers
                .OfType<MessageSubscription>()
                .FirstOrDefault(s => s.SubscriptionToken.Equals(subscriptionToken));
            messageSubscribers.Remove(subscriberItem);

            // check if any then remove the key
            if (!messageSubscribers.Any())
            {
                messageSubscriptions.Remove(messageType);
            }
        }

        public void Unsubscribe<TMessage>(Action<TMessage> subscriber) where TMessage : IMessage
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

        public void UnsubscribeAll<TMessage>() where TMessage : IMessage
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
            messageQueue.Enqueue(message);
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
                    if (!messageQueue.Any())
                    {
                        break;
                    }

                    var message = messageQueue.Dequeue();
                    foreach (var messageSubscription in messageSubscriptions[message.GetType()])
                    {
                        Task.Factory
                            .StartNew(() =>
                            {
                                messageSubscription.Dispatch(message);
                            }, CancellationToken.None, TaskCreationOptions.None, messageSubscription.Scheduler)
                            .ContinueWith((task) =>
                            {
                                OnPublishException(new PublishExceptionEventArgs
                                {
                                    Exception = task.Exception,
                                    Message = message,
                                    SubscriberType = messageSubscription.GetSubscriberType()
                                });
                            }, TaskContinuationOptions.OnlyOnFaulted);
                    }
                }
            }
        }
    }
}
