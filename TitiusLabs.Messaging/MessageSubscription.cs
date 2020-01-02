using System;
using System.Reflection;
using System.Threading.Tasks;

namespace TitiusLabs.Messaging
{
    internal abstract class MessageSubscription
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

        public abstract void Dispatch(IMessage message);

        internal abstract MethodInfo GetSubscriberType();
    }

    internal sealed class MessageSubscription<TMessage> : MessageSubscription where TMessage : IMessage
    {
        public Action<TMessage> Subscriber { get; set; }
        public Func<TMessage, bool> Filter { get; set; }

        public override void Dispatch(IMessage message)
        {
            var typedMessage = (TMessage)message;
            if ((Filter?.Invoke(typedMessage) ?? true))
            {
                Subscriber?.Invoke(typedMessage);
            }
        }

        internal override MethodInfo GetSubscriberType()
        {
            return Subscriber.Method;
        }
    }
}
