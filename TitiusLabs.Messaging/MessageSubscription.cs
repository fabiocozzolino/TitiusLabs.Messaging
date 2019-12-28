using System;
using System.Threading.Tasks;

namespace TitiusLabs.Messaging
{
    internal class MessageSubscription
    {
        public static MessageSubscription<TMessage> Create<TMessage>(Action<TMessage> subscriber, Func<TMessage, bool> filter, TaskScheduler scheduler) where TMessage : IMessage
        {
            return new MessageSubscription<TMessage>
            {
                Subscriber = subscriber,
                Scheduler = scheduler,
                Filter = filter
            };
        }

        public ISubscriptionToken SubscriptionToken { get; set; }

        public TaskScheduler Scheduler { get; set; }

        public virtual void Invoke(IMessage message)
        {

        }
    }

    internal class MessageSubscription<TMessage> : MessageSubscription where TMessage : IMessage
    {
        public Action<TMessage> Subscriber { get; set; }
        public Func<TMessage, bool> Filter { get; set; }
        public override void Invoke(IMessage message)
        {
            base.Invoke(message);

            var typedMessage = (TMessage)message;
            if (Filter?.Invoke(typedMessage) ?? true)
            {
                Subscriber.Invoke(typedMessage);
            }
        }
    }
}
